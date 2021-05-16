﻿using Microsoft.CodeAnalysis;

namespace NuGet.Insights
{
    public class PropertyVisitorContext
    {
        public PropertyVisitorContext(
            GeneratorExecutionContext generatorExecutionContext,
            INamedTypeSymbol nullable,
            INamedTypeSymbol kustoTypeAttribute)
        {
            GeneratorExecutionContext = generatorExecutionContext;
            Nullable = nullable;
            KustoTypeAttribute = kustoTypeAttribute;
        }

        public GeneratorExecutionContext GeneratorExecutionContext { get; }
        public INamedTypeSymbol Nullable { get; }
        public INamedTypeSymbol KustoTypeAttribute { get; }
    }
}
