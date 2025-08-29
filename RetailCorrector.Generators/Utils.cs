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
    }
}