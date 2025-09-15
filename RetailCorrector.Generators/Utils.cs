using Microsoft.CodeAnalysis;
using System;

namespace RetailCorrector.Generators
{
    internal static class Utils
    {
        public static string GetAccessModifier(this ITypeSymbol symbol) => symbol.DeclaredAccessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.Internal => "internal",
            Accessibility.Public => "public",
            _ => throw new NotSupportedException(),
        };

        public static bool IsCommonClass(this ITypeSymbol symbol) =>
            !symbol.IsStatic && !symbol.IsAbstract;

        public static bool InheritedFrom(this ITypeSymbol type, string path) =>
            type.BaseType?.ToDisplayString().StartsWith(path) ?? false;
    }
}