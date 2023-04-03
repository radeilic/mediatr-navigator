using Microsoft.CodeAnalysis;

namespace MediatrNavigator.Helpers.TypeSymbolHandlers
{
    public interface ITypeSymbolHandler
    {
        ITypeSymbolHandler SetNext(ITypeSymbolHandler next);

        INamedTypeSymbol GetTypeSymbol();
    }
}
