using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MediatrNavigator.Helpers.TypeSymbolHandlers
{
    public class VariableDeclarationHandler : BaseTypeSymbolHandler
    {
        public VariableDeclarationHandler(SemanticModel semanticModel, SyntaxNode syntaxNode) : base(semanticModel, syntaxNode)
        {
        }

        public override INamedTypeSymbol GetTypeSymbol()
        {
            var variableDeclarationSyntax = _syntaxNode.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().FirstOrDefault();
            if (variableDeclarationSyntax != null)
            {
                ITypeSymbol typeInfo = _semanticModel.GetTypeInfo(variableDeclarationSyntax.Type).Type;
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
