using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System.Linq;

namespace MediatrNavigator.Helpers
{
    internal class ActiveDocumentAnalyzer
    {
        private readonly Workspace workspace;

        public ActiveDocumentAnalyzer(Workspace workspace)
        {
            this.workspace = workspace;
        }

        public ActiveDocumentInfo GetActiveDocumentInfo()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            var activeDocument = dte.ActiveDocument;

            // Get the TextDocument from the active document
            EnvDTE.TextDocument textDocument = (EnvDTE.TextDocument)activeDocument.Object("TextDocument");
            EditPoint activePoint = textDocument.Selection.ActivePoint.CreateEditPoint();

            // Get the file path and cursor position
            string filePath = activeDocument.FullName;
            int line = activePoint.Line;
            int column = activePoint.LineCharOffset;

            var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();

            return new ActiveDocumentInfo()
            {
                ActiveDocument = textDocument,
                DocumentId = documentId,
                Line = line,
                Column = column,
            };
        }
    }
}
