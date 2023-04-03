using Microsoft.CodeAnalysis;

namespace MediatrNavigator.Helpers
{
    public class ActiveDocumentInfo
    {
        public EnvDTE.TextDocument ActiveDocument { get; set; }
        public DocumentId DocumentId { get; set; }
        public int Line { get; set; }

        public int Column { get; set; }

        public int Position { get; set; }
    }
}
