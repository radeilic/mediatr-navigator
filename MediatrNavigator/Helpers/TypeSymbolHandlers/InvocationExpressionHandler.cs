using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MediatrNavigator.Helpers.TypeSymbolHandlers
{
    public class InvocationExpressionHandler : BaseTypeSymbolHandler
    {
        public InvocationExpressionHandler(SemanticModel semanticModel, SyntaxNode syntaxNode) : base(semanticModel, syntaxNode)
        {
        }

        public override INamedTypeSymbol GetTypeSymbol()
        {
            var invocationExpressionSyntax = _syntaxNode.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocationExpressionSyntax != null)
            {
                var argumentList = invocationExpressionSyntax.ChildNodes().Where(c => c is ArgumentListSyntax).FirstOrDefault();
                var argument = argumentList.ChildNodes().Where(c => c is ArgumentSyntax).FirstOrDefault();
                if (argument is ArgumentSyntax argumentSyntax)
                {
                    var argumentType = _semanticModel.GetTypeInfo(argumentSyntax.Expression).Type;
                    return argumentType as INamedTypeSymbol;
                }
            }

            if (_next != null)
            {
                return _next.GetTypeSymbol();
            }
            return null;
        }
    }
}
