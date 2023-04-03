using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MediatrNavigator.Helpers.TypeSymbolHandlers
{
    internal class ParameterHandler : BaseTypeSymbolHandler
    {
        public ParameterHandler(SemanticModel semanticModel, SyntaxNode syntaxNode) : base(semanticModel, syntaxNode)
        {
        }

        public override INamedTypeSymbol GetTypeSymbol()
        {
            if (_syntaxNode is ParameterSyntax parameterSyntax)
            {
                ITypeSymbol typeInfo = _semanticModel.GetDeclaredSymbol(parameterSyntax).Type;
                return typeInfo as INamedTypeSymbol;
            }

            if (_next != null)
            {
                return _next.GetTypeSymbol();
            }
            return null;
        }
    }
}
