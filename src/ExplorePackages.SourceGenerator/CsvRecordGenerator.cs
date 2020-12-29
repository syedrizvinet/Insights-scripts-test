﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Knapcode.ExplorePackages
{
    [Generator]
    public class CsvRecordGenerator : ISourceGenerator
    {
        private const string Category = "Knapcode.ExplorePackages.SourceGenerator";
        private const string InterfaceNamePrefix = "ICsvRecord";
        private const string FullInterfaceName = "Knapcode.ExplorePackages.ICsvRecord`1";

        private static readonly string AutoGenerated = "// <auto-generated />" + Environment.NewLine + Environment.NewLine;
        private static readonly string Template = AutoGenerated + @"using System;
using System.IO;
using Knapcode.ExplorePackages;

namespace {0}
{{
    /* Kusto DDL:

    .drop table {1};

    .create table {1} (
{2}
    );

    .create table {1} ingestion csv mapping '{1}_mapping'
    '['
{3}
    ']'

    */
    partial {4} {5}
    {{
        public void Write(TextWriter writer)
        {{
{6}
        }}

        public {5} Read(Func<string> getNextField)
        {{
            return new {5}
            {{
{7}
            }};
        }}
    }}
}}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
            {
                return;
            }

            // System.Diagnostics.Debugger.Launch();

            var nullable = context.Compilation.GetTypeByMetadataName("System.Nullable`1");
            var type = context.Compilation.GetTypeByMetadataName(FullInterfaceName);
            if (type == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "EXP0001",
                        title: $"{InterfaceNamePrefix} interface could not be found",
                        messageFormat: $"The {FullInterfaceName} interface could not be found.",
                        Category,
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    Location.None));
            }

            var infos = receiver
                .CandidateClasses
                .Select(x => new { Keyword = "class", Candidate = (TypeDeclarationSyntax)x, x.Identifier, x.Modifiers })
                .Concat(receiver
                    .CandidateRecords
                    .Select(x => new { Keyword = "record", Candidate = (TypeDeclarationSyntax)x, x.Identifier, x.Modifiers }));

            var sourceAdded = false;
            foreach (var info in infos)
            {
                var model = context.Compilation.GetSemanticModel(info.Candidate.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(info.Candidate);
                var typeSymbol = (ITypeSymbol)symbol;
                if (!typeSymbol.Interfaces.OfType<INamedTypeSymbol>().Any(x => SymbolEqualityComparer.Default.Equals(type, x.OriginalDefinition)))
                {
                    continue;
                }

                if (!info.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "EXP0002",
                            title: $"{InterfaceNamePrefix} implementor is not partial",
                            messageFormat: $"The type {{0}} implements {InterfaceNamePrefix} but is not declared as partial.",
                            Category,
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        Location.Create(info.Candidate.SyntaxTree, info.Candidate.Span),
                        info.Identifier.Text));
                    continue;
                }

                var typeNamespacePrefix = symbol.ContainingNamespace.ToString() + ".";

                var kustoTableBuilder = new KustoTableBuilder(indent: 8);
                var kustoMappingBuilder = new KustoMappingBuilder(8);
                var writerBuilder = new WriteBuilder(indent: 12);
                var readerBuilder = new ReadBuilder(indent: 16);

                var visitors = new IPropertyVisitor[]
                {
                    kustoTableBuilder,
                    kustoMappingBuilder,
                    writerBuilder,
                    readerBuilder,
                };

                var sortedProperties = new List<IPropertySymbol>();
                var currentType = symbol;
                while (currentType != null)
                {
                    sortedProperties.AddRange(currentType
                        .GetMembers()
                        .Where(x => !x.IsImplicitlyDeclared)
                        .OfType<IPropertySymbol>()
                        .OrderByDescending(x => x.Locations.First().SourceSpan.Start));
                    currentType = currentType.BaseType;
                }

                sortedProperties.Reverse();
                var propertyNames = sortedProperties.Select(x => x.Name).ToImmutableHashSet();

                foreach (var propertySymbol in sortedProperties)
                {
                    var propType = propertySymbol.Type.ToString();
                    var propName = propertySymbol.Name;

                    var prettyType = PropertyHelper.GetPrettyType(typeNamespacePrefix, propertyNames, propertySymbol);
                    foreach (var visitor in visitors)
                    {
                        visitor.OnProperty(nullable, propertySymbol, prettyType);
                    }
                }

                foreach (var visitor in visitors)
                {
                    visitor.Finish();
                }

                var typeName = info.Identifier.Text;
                var kustoTableName = "Jver" + typeName.Pluralize();

                context.AddSource(
                    $"{typeName}.{InterfaceNamePrefix}.cs",
                    SourceText.From(
                        string.Format(
                            Template,
                            symbol.ContainingNamespace,
                            kustoTableName,
                            kustoTableBuilder.GetResult(),
                            kustoMappingBuilder.GetResult(),
                            info.Keyword,
                            typeName,
                            writerBuilder.GetResult(),
                            readerBuilder.GetResult()),
                        Encoding.UTF8));
                sourceAdded = true;
            }

            if (sourceAdded)
            {
                using (var stream = typeof(CsvRecordGenerator)
                    .Assembly
                    .GetManifestResourceStream($"{typeof(CsvUtility).Namespace}.{nameof(CsvUtility)}.cs"))
                using (var reader = new StreamReader(stream))
                {
                    var csvReader = AutoGenerated + reader.ReadToEnd();
                    context.AddSource($"{nameof(CsvUtility)}.cs", SourceText.From(csvReader, Encoding.UTF8));
                }
            }
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
            public List<RecordDeclarationSyntax> CandidateRecords { get; } = new List<RecordDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax && HasInterface(classDeclarationSyntax.BaseList))
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
                else if (syntaxNode is RecordDeclarationSyntax recordDeclarationSyntax && HasInterface(recordDeclarationSyntax.BaseList))
                {
                    CandidateRecords.Add(recordDeclarationSyntax);
                }
            }

            private static bool HasInterface(BaseListSyntax baseList)
            {
                return baseList?
                    .Types
                    .Select(x => x.Type)
                    .OfType<GenericNameSyntax>()
                    .Select(x => x.Identifier.Text)
                    .Any(x => x.IndexOf(InterfaceNamePrefix, StringComparison.OrdinalIgnoreCase) > -1) ?? false;
            }
        }
    }
}
