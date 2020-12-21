﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private const string InterfaceName = "ICsvRecord";
        private const string FullInterfaceName = "Knapcode.ExplorePackages." + InterfaceName;

        private static readonly string Template = @"// <auto-generated />
using System;
using System.IO;
using Knapcode.ExplorePackages;

namespace {0}
{{
    partial class {1}
    {{
        public void Write(TextWriter writer)
        {{
{2}
        }}

        public void Read(Func<string> getNextField)
        {{
{3}
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

            var type = context.Compilation.GetTypeByMetadataName(FullInterfaceName);
            if (type == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "EXP0001",
                        title: $"{InterfaceName} interface could not be found",
                        messageFormat: $"The {FullInterfaceName} interface could not be found.",
                        Category,
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    Location.None));
            }

            using (var stream = typeof(CsvRecordGenerator)
                .Assembly
                .GetManifestResourceStream($"{typeof(CsvUtility).Namespace}.{nameof(CsvUtility)}.cs"))
            using (var reader = new StreamReader(stream))
            {
                var csvReader = reader.ReadToEnd();
                context.AddSource($"{nameof(CsvUtility)}.cs", SourceText.From(csvReader, Encoding.UTF8));
            }

            foreach (var declaredClass in receiver.CandidateClasses)
            {
                var model = context.Compilation.GetSemanticModel(declaredClass.SyntaxTree);
                var classModel = model.GetDeclaredSymbol(declaredClass);
                var classTypeSymbol = (ITypeSymbol)classModel;
                if (!classTypeSymbol.Interfaces.Contains(type))
                {
                    continue;
                }

                if (!declaredClass.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "EXP0002",
                            title: $"{InterfaceName} implementor is not partial",
                            messageFormat: $"The class {{0}} implements {InterfaceName} but is not declared as partial.",
                            Category,
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        Location.Create(declaredClass.SyntaxTree, declaredClass.Span),
                        declaredClass.Identifier.Text));
                    continue;
                }

                var classNamespacePrefix = classModel.ContainingNamespace.ToString() + ".";
                var writerBuilder = new StringBuilder();
                var readerBuilder = new StringBuilder();
                const int indent = 12;

                var sortedMembers = declaredClass.Members.OrderBy(x => x.SpanStart);
                foreach (var member in sortedMembers)
                {
                    var memberSymbol = model.GetDeclaredSymbol(member);
                    if (memberSymbol is IPropertySymbol propertySymbol)
                    {
                        if (writerBuilder.Length > 0)
                        {
                            writerBuilder.AppendLine();
                            writerBuilder.Append(' ', indent);
                            writerBuilder.AppendLine("writer.Write(',');");
                        }

                        if (readerBuilder.Length > 0)
                        {
                            readerBuilder.AppendLine();
                        }

                        var propType = propertySymbol.Type.ToString();
                        var propName = propertySymbol.Name;

                        var nonNullPropType = propType.TrimEnd('?');

                        // Clean up the type name by removing unnecessary namespaces.
                        const string systemPrefix = "System.";
                        if (nonNullPropType.StartsWith(systemPrefix))
                        {
                            nonNullPropType = nonNullPropType.Substring(systemPrefix.Length);
                        }

                        if (nonNullPropType.StartsWith(classNamespacePrefix))
                        {
                            nonNullPropType = nonNullPropType.Substring(classNamespacePrefix.Length);
                        }

                        writerBuilder.Append(' ', indent);

                        readerBuilder.Append(' ', indent);

                        switch (propType)
                        {
                            case "bool":
                            case "bool?":
                            case "short":
                            case "short?":
                            case "ushort":
                            case "ushort?":
                            case "int":
                            case "int?":
                            case "uint":
                            case "uint?":
                            case "long":
                            case "long?":
                            case "ulong":
                            case "ulong?":
                            case "System.Guid":
                            case "System.Guid?":
                            case "System.TimeSpan":
                            case "System.TimeSpan?":
                                writerBuilder.AppendFormat("writer.Write({0});", propName);
                                if (propType.EndsWith("?"))
                                {
                                    readerBuilder.AppendFormat("{0} = CsvUtility.ParseNullable(getNextField(), {1}.Parse);", propName, nonNullPropType);
                                }
                                else
                                {
                                    readerBuilder.AppendFormat("{0} = {1}.Parse(getNextField());", propName, nonNullPropType);
                                }
                                break;
                            case "string":
                                writerBuilder.AppendFormat("CsvUtility.WriteWithQuotes(writer, {0});", propName);
                                readerBuilder.AppendFormat("{0} = getNextField();", propName);
                                break;
                            case "System.DateTimeOffset":
                                writerBuilder.AppendFormat("writer.Write(CsvUtility.FormatDateTimeOffset({0}));", propName);
                                readerBuilder.AppendFormat("{0} = CsvUtility.ParseDateTimeOffset(getNextField());", propName);
                                break;
                            case "System.DateTimeOffset?":
                                writerBuilder.AppendFormat("writer.Write(CsvUtility.FormatDateTimeOffset({0}));", propName);
                                readerBuilder.AppendFormat("{0} = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset);", propName);
                                break;
                            default:
                                if (propertySymbol.Type.TypeKind == TypeKind.Enum)
                                {
                                    writerBuilder.AppendFormat("writer.Write({0});", propName);
                                    if (propType.EndsWith("?"))
                                    {
                                        readerBuilder.AppendFormat("{0} = CsvUtility.ParseNullable(getNextField(), Enum.Parse<{1}>);", propName, nonNullPropType);
                                    }
                                    else
                                    {
                                        readerBuilder.AppendFormat("{0} = Enum.Parse<{1}>(getNextField());", propName, nonNullPropType);
                                    }
                                }
                                else
                                {
                                    writerBuilder.AppendFormat("CsvUtility.WriteWithQuotes(writer, {0}?.ToString());", propName);
                                    readerBuilder.AppendFormat("{0} = Parse{0}(getNextField());", propName);
                                }
                                break;
                        }
                    }
                }

                writerBuilder.AppendLine();
                writerBuilder.Append(' ', indent);
                writerBuilder.Append("writer.WriteLine();");

                var className = declaredClass.Identifier.Text;
                context.AddSource(
                    $"{className}.{InterfaceName}.cs",
                    SourceText.From(
                        string.Format(
                            Template,
                            classModel.ContainingNamespace,
                            className,
                            writerBuilder,
                            readerBuilder),
                        Encoding.UTF8));
            }
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (!(syntaxNode is ClassDeclarationSyntax classDeclarationSyntax))
                {
                    return;
                }

                var hasInterface = classDeclarationSyntax
                    .BaseList?
                    .Types
                    .Select(x => x.Type)
                    .OfType<IdentifierNameSyntax>()
                    .Select(x => x.Identifier.Text)
                    .Any(x => x.IndexOf(InterfaceName, StringComparison.OrdinalIgnoreCase) > -1);

                if (hasInterface == true)
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
