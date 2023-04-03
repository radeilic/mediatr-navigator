using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System;
using System.Linq;

namespace MediatrNavigator.Helpers
{
    public static class VsProjectHelper
    {
        public static ProjectItem FindProjectItemByFilePath(DTE dte, string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (dte == null || string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            foreach (EnvDTE.Project project in dte.Solution.Projects)
            {
                ProjectItem projectItem = FindProjectItemByFilePath(project.ProjectItems, filePath);
                if (projectItem != null)
                {
                    return projectItem;
                }
            }

            return null;
        }

        public static ProjectItem GetProjectItem(INamedTypeSymbol symbol)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            string filePath = symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            ProjectItem projectItem = FindProjectItemByFilePath(dte, filePath);
            return projectItem;
        }

        public static void ActivateItem(ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Window window = projectItem.Open(Constants.vsViewKindCode);
            window.Visible = true;
            window.Activate();
        }

        private static ProjectItem FindProjectItemByFilePath(ProjectItems projectItems, string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (projectItems == null)
            {
                return null;
            }

            foreach (ProjectItem projectItem in projectItems)
            {
                if (projectItem.FileCount > 0 && string.Equals(projectItem.FileNames[0], filePath, StringComparison.OrdinalIgnoreCase))
                {
                    return projectItem;
                }

                ProjectItem foundProjectItem = FindProjectItemByFilePath(projectItem.ProjectItems, filePath);
                if (foundProjectItem != null)
                {
                    return foundProjectItem;
                }
            }

            return null;
        }
    }
}
