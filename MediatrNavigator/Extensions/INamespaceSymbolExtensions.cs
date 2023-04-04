using Microsoft.CodeAnalysis;

namespace MediatrNavigator.Extensions
{
    public static class INamespaceSymbolExtensions
    {
        public static string GetFullyQualifiedName(this INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace)
            {
                return string.Empty;
            }

            string fullyQualifiedName = namespaceSymbol.Name;
            INamespaceSymbol currentNamespace = namespaceSymbol.ContainingNamespace;

            while (currentNamespace != null && !currentNamespace.IsGlobalNamespace)
            {
                fullyQualifiedName = $"{currentNamespace.Name}.{fullyQualifiedName}";
                currentNamespace = currentNamespace.ContainingNamespace;
            }

            return fullyQualifiedName;
        }
    }
}
