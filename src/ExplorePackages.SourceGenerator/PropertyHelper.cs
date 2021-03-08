﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Knapcode.ExplorePackages
{
    public static class PropertyHelper
    {
        public static string GetPrettyType(string classNamespacePrefix, ISet<string> propertyNames, IPropertySymbol symbol)
        {
            var propType = symbol.Type.ToString();
            var nonNullPropType = propType.TrimEnd('?');
            var prettyPropType = nonNullPropType;

            // Clean up the type name by removing unnecessary namespaces.
            const string systemPrefix = "System.";
            if (prettyPropType.StartsWith(systemPrefix) && prettyPropType.IndexOf('.', systemPrefix.Length) < 0)
            {
                prettyPropType = prettyPropType.Substring(systemPrefix.Length);
            }
            else if (prettyPropType.StartsWith(classNamespacePrefix))
            {
                prettyPropType = prettyPropType.Substring(classNamespacePrefix.Length);
            }

            // To avoid property name collisions, only use the pretty version if it doesn't match a property name.
            if (propertyNames.Contains(prettyPropType))
            {
                prettyPropType = nonNullPropType;
            }

            return prettyPropType;
        }

        public static bool IsNullableEnum(PropertyVisitorContext context, IPropertySymbol symbol)
        {
            return IsNullable(context, symbol, out var innerType) && innerType.TypeKind == TypeKind.Enum;
        }

        public static bool IsNullable(PropertyVisitorContext context, IPropertySymbol symbol, out ITypeSymbol innerType)
        {
            if (symbol.Type is INamedTypeSymbol typeSymbol
                && SymbolEqualityComparer.Default.Equals(typeSymbol.OriginalDefinition, context.Nullable)
                && typeSymbol.TypeArguments.Length == 1)
            {
                innerType = typeSymbol.TypeArguments[0];
                return true;
            }

            innerType = null;
            return false;
        }

        public static string GetKustoDataType(PropertyVisitorContext context, IPropertySymbol symbol)
        {
            var attributeData = symbol
                .GetAttributes()
                .SingleOrDefault(x => x.AttributeClass.Equals(context.KustoTypeAttribute, SymbolEqualityComparer.Default));
            if (attributeData != null)
            {
                var kustoType = attributeData.ConstructorArguments.Single().Value;
                return kustoType.ToString();
            }

            switch (symbol.Type.ToString())
            {
                case "bool":
                case "bool?":
                    return "bool";
                case "short":
                case "short?":
                case "int":
                case "int?":
                    return "int";
                case "long":
                case "long?":
                    return "long";
                case "System.Guid":
                case "System.Guid?":
                    return "guid";
                case "System.TimeSpan":
                case "System.TimeSpan?":
                    return "timespan";
                case "System.DateTimeOffset":
                case "System.DateTimeOffset?":
                    return "datetime";
                case "System.Version":
                case "string":
                    return "string";
                default:
                    if (symbol.Type.TypeKind == TypeKind.Enum || IsNullableEnum(context, symbol))
                    {
                        return "string";
                    }
                    else
                    {
                        return "dynamic";
                    }
            }
        }
    }
}
