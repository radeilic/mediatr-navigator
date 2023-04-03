using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MediatrNavigator.Helpers.TypeSymbolHandlers
{
    internal class DefaultTypeSymbolHandler : BaseTypeSymbolHandler
    {
        public DefaultTypeSymbolHandler(SemanticModel semanticModel, SyntaxNode syntaxNode) : base(semanticModel, syntaxNode)
        {
        }

        public override INamedTypeSymbol GetTypeSymbol()
        {
            ClassDeclarationSyntax classDeclaration = GetAnyParentClassDeclaration();
            if (classDeclaration != null)
            {
                return _semanticModel.GetDeclaredSymbol(classDeclaration);
            }

            if (_next != null)
            {
                return _next.GetTypeSymbol();
            }
            return null;
        }

        private ClassDeclarationSyntax GetAnyParentClassDeclaration()
        {
            return _syntaxNode.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        }
    }
}
