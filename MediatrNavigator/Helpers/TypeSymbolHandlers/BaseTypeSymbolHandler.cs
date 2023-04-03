using Microsoft.CodeAnalysis;

namespace MediatrNavigator.Helpers.TypeSymbolHandlers
{
    public abstract class BaseTypeSymbolHandler : ITypeSymbolHandler
    {
        protected ITypeSymbolHandler _next;

        protected SemanticModel _semanticModel;
        protected SyntaxNode _syntaxNode;

        public BaseTypeSymbolHandler(SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            _semanticModel = semanticModel;
            _syntaxNode = syntaxNode;
        }

        public ITypeSymbolHandler SetNext(ITypeSymbolHandler next)
        {
            _next = next;
            return next;
        }

        public abstract INamedTypeSymbol GetTypeSymbol();
    }
}
