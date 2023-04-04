using Microsoft.CodeAnalysis;

namespace MediatrNavigator.Extensions
{
    public static class ITypeSymbolExtensions
    {
        public static bool AreEquivalent(this ITypeSymbol thisSymbol, ITypeSymbol otherSymbol)
        {
            if (thisSymbol == null || otherSymbol == null)
                return false;

            if (thisSymbol.Name != otherSymbol.Name)
                return false;

            if (thisSymbol.ContainingNamespace.GetFullyQualifiedName() != otherSymbol.ContainingNamespace.GetFullyQualifiedName())
                return false;

            if (!thisSymbol.ContainingAssembly.IsEquivalentTo(otherSymbol.ContainingAssembly))
                return false;

            return true;
        }
    }
}
