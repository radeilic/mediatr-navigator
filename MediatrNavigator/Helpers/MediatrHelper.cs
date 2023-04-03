using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Shell;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using MediatrNavigator.Helpers.TypeSymbolHandlers;

namespace MediatrNavigator.Helpers
{
    public static class MediatrHelper
    {
        public static async Task<INamedTypeSymbol> GetCommandClassSymbolAtPositionAsync(Workspace workspace)
        {
            var typeSymbol = await GetTypeSymbolAtPositionAsync(workspace).ConfigureAwait(false);
            if (IsMediatrCommand(typeSymbol))
            {
                return typeSymbol;
            }
            return null;
        }

        private static async Task<INamedTypeSymbol> GetTypeSymbolAtPositionAsync(Workspace workspace)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var activeDocumentAnalyzer = new ActiveDocumentAnalyzer(workspace);
            var activeDocumentInfo = activeDocumentAnalyzer.GetActiveDocumentInfo();
            if (activeDocumentInfo.DocumentId == null)
            {
                return null;
            }

            var document = workspace.CurrentSolution.GetDocument(activeDocumentInfo.DocumentId);
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var semanticModel = await document.GetSemanticModelAsync();

            var syntaxRoot = await document.GetSyntaxRootAsync();

            var position = (await syntaxTree.GetTextAsync()).Lines[activeDocumentInfo.Line - 1].Start + activeDocumentInfo.Column - 1;

            var span = new TextSpan(position, 0);
            var syntaxNode = syntaxRoot.FindNode(span, true, true);
            return GetTypeSymbolFromSyntaxNode(semanticModel, syntaxNode);
        }

        private static INamedTypeSymbol GetTypeSymbolFromSyntaxNode(SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            var typeSymbolHandlerChain = new ParameterHandler(semanticModel, syntaxNode);
            typeSymbolHandlerChain
                .SetNext(new VariableDeclarationHandler(semanticModel, syntaxNode))
                .SetNext(new InvocationExpressionHandler(semanticModel, syntaxNode))
                .SetNext(new IdentifierNameHandler(semanticModel, syntaxNode))
                .SetNext(new DefaultTypeSymbolHandler(semanticModel, syntaxNode));

            return typeSymbolHandlerChain.GetTypeSymbol();
        }

        public static async Task<INamedTypeSymbol> FindCommandHandlerSymbolAsync(INamedTypeSymbol commandClassSymbol, Workspace workspace)
        {
            var commandAssembly = commandClassSymbol.ContainingAssembly;
            var commandProject = workspace.CurrentSolution.Projects.FirstOrDefault(p => p.AssemblyName == commandAssembly.Name);
            Compilation compilation = await commandProject.GetCompilationAsync(CancellationToken.None);
            var baseHandlerInterfaceSymbol = compilation.GetTypeByMetadataName("MediatR.IRequestHandler`2");

            IEnumerable<ReferencedSymbol> references = await SymbolFinder.FindReferencesAsync(commandClassSymbol, workspace.CurrentSolution);

            return await FindSymbolsForReferencesAsync(references, compilation, commandClassSymbol, baseHandlerInterfaceSymbol);
        }

        public static async Task<INamedTypeSymbol> FindSymbolsForReferencesAsync(
            IEnumerable<ReferencedSymbol> references,
            Compilation compilation,
            INamedTypeSymbol commandClassSymbol,
            INamedTypeSymbol baseHandlerInterfaceSymbol)
        {
            foreach (ReferenceLocation location in references.SelectMany(r => r.Locations))
            {
                SyntaxTree syntaxTree = await location.Document.GetSyntaxTreeAsync();
                var classDeclarations = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<ClassDeclarationSyntax>();
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                foreach (var classDeclaration in classDeclarations)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                    var handlerInterfaceSymbol = classSymbol
                        .AllInterfaces
                        .Where(i => IsImplementingInterface(i, baseHandlerInterfaceSymbol))
                        .FirstOrDefault();

                    if (IsHandlerOfCommand(commandClassSymbol, handlerInterfaceSymbol))
                    {
                        return classSymbol;
                    }
                }
            }

            return null;
        }

        private static bool IsHandlerOfCommand(INamedTypeSymbol commandClassSymbol, INamedTypeSymbol implementedInterface)
        {
            var typeArguments = implementedInterface?.TypeArguments;
            return typeArguments != null && IsCommandInTypeArguments(commandClassSymbol, typeArguments.Value);
        }

        private static bool IsImplementingInterface(INamedTypeSymbol implementedInterface, INamedTypeSymbol requestHandlerType)
        {
            return implementedInterface.OriginalDefinition.Equals(requestHandlerType, SymbolEqualityComparer.Default);
        }

        private static bool IsCommandInTypeArguments(INamedTypeSymbol commandClassSymbol, ImmutableArray<ITypeSymbol> typeArguments)
        {
            return typeArguments.Length == 2 && typeArguments[0].Equals(commandClassSymbol, SymbolEqualityComparer.Default);
        }

        public static bool IsMediatrCommand(INamedTypeSymbol classSymbol)
        {
            return classSymbol?.AllInterfaces.Any(i => i.Name.Equals("IRequest") && i.ContainingNamespace.Name == "MediatR") ?? false;
        }
    }
}
