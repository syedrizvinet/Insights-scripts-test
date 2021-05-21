// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

namespace NuGet.Insights
{
    [Generator]
    public class CsvRecordGenerator : ISourceGenerator
    {
        internal const string Category = "NuGet.Insights.SourceGenerator";
        private const string InterfaceNamePrefix = "ICsvRecord";
        private const string FullInterfaceName = "NuGet.Insights.ICsvRecord";
        private const string FullKustoTypeAttributeName = "NuGet.Insights.KustoTypeAttribute";

        private static readonly string AutoGenerated = "// <auto-generated />" + Environment.NewLine + Environment.NewLine;
        private static readonly string CsvRecordTemplate = AutoGenerated + @"// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NuGet.Insights;

namespace {0}
{{
    /* Kusto DDL:

    .drop table {1} ifexists;

    .create table {1} (
{2}
    );

    .alter-merge table {1} policy retention softdelete = 30d;

    .alter table {1} policy partitioning {3};

    .create table {1} ingestion csv mapping '{13}'
    '['
{4}
    ']'

    */
    partial {5} {6}
    {{
        public int FieldCount => {7};

        public void WriteHeader(TextWriter writer)
        {{
{8}
        }}

        public void Write(List<string> fields)
        {{
{9}
        }}

        public void Write(TextWriter writer)
        {{
{10}
        }}

        public async Task WriteAsync(TextWriter writer)
        {{
{11}
        }}

        public ICsvRecord ReadNew(Func<string> getNextField)
        {{
            return new {6}
            {{
{12}
            }};
        }}
    }}
}}
";

        private static readonly string KustoDDLTemplate = AutoGenerated + @"// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace NuGet.Insights
{{
    static partial class KustoDDL
    {{
        public const string {3}DefaultTableName = ""{1}"";

        public static readonly IReadOnlyList<string> {3}DDL = new[]
        {{
            "".drop table __TABLENAME__ ifexists"",

            @"".create table __TABLENAME__ (
{2}
)"",

            "".alter-merge table __TABLENAME__ policy retention softdelete = 30d"",

            @"".create table __TABLENAME__ ingestion csv mapping '{6}'
'['
{5}
']'"",
        }};

        public const string {3}PartitioningPolicy = @"".alter table __TABLENAME__ policy partitioning {4}"";

        private static readonly bool {3}AddTypeToDefaultTableName = AddTypeToDefaultTableName(typeof({0}.{3}), {3}DefaultTableName);

        private static readonly bool {3}AddTypeToDDL = AddTypeToDDL(typeof({0}.{3}), {3}DDL);

        private static readonly bool {3}AddTypeToPartitioningPolicy = AddTypeToPartitioningPolicy(typeof({0}.{3}), {3}PartitioningPolicy);
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
            var interfaceType = context.Compilation.GetTypeByMetadataName(FullInterfaceName);
            if (interfaceType == null)
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

            var kustoTypeAttributeType = context.Compilation.GetTypeByMetadataName(FullKustoTypeAttributeName);
            if (kustoTypeAttributeType == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "EXP0006",
                        title: $"{FullKustoTypeAttributeName.Split('.').Last()} interface could not be found",
                        messageFormat: $"The {FullKustoTypeAttributeName} interface could not be found.",
                        Category,
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    Location.None));
            }

            var propertyVisitorContext = new PropertyVisitorContext(
                context,
                nullable,
                kustoTypeAttributeType);

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
                ITypeSymbol typeSymbol = symbol;
                if (!typeSymbol.Interfaces.OfType<INamedTypeSymbol>().Any(x => SymbolEqualityComparer.Default.Equals(interfaceType, x.OriginalDefinition)))
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

                var kustoTableCommentBuilder = new KustoTableBuilder(indent: 8);
                var kustoPartitioningPolicyCommentBuilder = new KustoPartitioningPolicyBuilder(indent: 4, escapeQuotes: false);
                var kustoMappingCommentBuilder = new KustoMappingBuilder(indent: 8, escapeQuotes: false);
                var writeHeaderBuilder = new WriteHeaderBuilder(indent: 12);
                var writeListBuilder = new WriteListBuilder(indent: 12);
                var writeTextWriterBuilder = new WriteTextWriterBuilder(indent: 12);
                var writeAsyncTextWriterBuilder = new WriteAsyncTextWriterBuilder(indent: 12);
                var readerBuilder = new ReadBuilder(indent: 16);
                var kustoTableConstantBuilder = new KustoTableBuilder(indent: 4);
                var kustoPartitioningPolicyConstantBuilder = new KustoPartitioningPolicyBuilder(indent: 0, escapeQuotes: true);
                var kustoMappingConstantBuilder = new KustoMappingBuilder(indent: 4, escapeQuotes: true);

                var visitors = new IPropertyVisitor[]
                {
                    kustoTableCommentBuilder,
                    kustoPartitioningPolicyCommentBuilder,
                    kustoMappingCommentBuilder,
                    writeHeaderBuilder,
                    writeListBuilder,
                    writeTextWriterBuilder,
                    writeAsyncTextWriterBuilder,
                    readerBuilder,
                    kustoTableConstantBuilder,
                    kustoPartitioningPolicyConstantBuilder,
                    kustoMappingConstantBuilder,
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
                        visitor.OnProperty(propertyVisitorContext, propertySymbol, prettyType);
                    }
                }

                foreach (var visitor in visitors)
                {
                    visitor.Finish(propertyVisitorContext);
                }

                var typeName = info.Identifier.Text;
                var kustoTableName = typeName;
                var suffixesToRemove = new[]
                {
                    "CsvRecord",
                    "Record",
                }.OrderByDescending(x => x.Length).ThenBy(x => x);

                foreach (var suffix in suffixesToRemove)
                {
                    if (kustoTableName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        kustoTableName = kustoTableName.Substring(0, kustoTableName.Length - suffix.Length);
                    }
                }
                kustoTableName = kustoTableName.Pluralize();

                context.AddSource(
                    $"{typeName}.{InterfaceNamePrefix}.cs",
                    SourceText.From(
                        string.Format(
                            CsvRecordTemplate,
                            symbol.ContainingNamespace,
                            kustoTableName,
                            kustoTableCommentBuilder.GetResult(),
                            kustoPartitioningPolicyCommentBuilder.GetResult(),
                            kustoMappingCommentBuilder.GetResult(),
                            info.Keyword,
                            typeName,
                            propertyNames.Count,
                            writeHeaderBuilder.GetResult(),
                            writeListBuilder.GetResult(),
                            writeTextWriterBuilder.GetResult(),
                            writeAsyncTextWriterBuilder.GetResult(),
                            readerBuilder.GetResult(),
                            KustoDDL.CsvMappingName),
                        Encoding.UTF8));

                context.AddSource(
                    $"{typeName}.KustoDDL.cs",
                    SourceText.From(
                        string.Format(
                            KustoDDLTemplate,
                            symbol.ContainingNamespace,
                            kustoTableName,
                            kustoTableConstantBuilder.GetResult(),
                            typeName,
                            kustoPartitioningPolicyConstantBuilder.GetResult(),
                            kustoMappingConstantBuilder.GetResult(),
                            KustoDDL.CsvMappingName),
                        Encoding.UTF8));

                sourceAdded = true;
            }

            if (sourceAdded)
            {
                AddType(context, typeof(CsvUtility));
                AddType(context, typeof(KustoDDL));
            }
        }

        private static void AddType(GeneratorExecutionContext context, Type type)
        {
            var typeName = type.Name;

            using (var stream = typeof(CsvRecordGenerator)
                .Assembly
                .GetManifestResourceStream($"{type.Namespace}.{type.Name}.cs"))
            using (var reader = new StreamReader(stream))
            {
                var source = AutoGenerated + reader.ReadToEnd();
                context.AddSource($"{typeName}.cs", SourceText.From(source, Encoding.UTF8));
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
                    .OfType<SimpleNameSyntax>()
                    .Select(x => x.Identifier.Text)
                    .Any(x => x.IndexOf(InterfaceNamePrefix, StringComparison.OrdinalIgnoreCase) > -1) ?? false;
            }
        }
    }
}
