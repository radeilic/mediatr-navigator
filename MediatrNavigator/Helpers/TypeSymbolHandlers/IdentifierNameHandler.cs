using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MediatrNavigator.Helpers.TypeSymbolHandlers
{
    internal class IdentifierNameHandler : BaseTypeSymbolHandler
    {
        public IdentifierNameHandler(SemanticModel semanticModel, SyntaxNode syntaxNode) : base(semanticModel, syntaxNode)
        {
        }

        public override INamedTypeSymbol GetTypeSymbol()
        {
            if (_syntaxNode is IdentifierNameSyntax identifierNameSyntax)
            {
                return _semanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            }

            if (_next != null)
            {
                return _next.GetTypeSymbol();
            }
            return null;
        }
    }
}
