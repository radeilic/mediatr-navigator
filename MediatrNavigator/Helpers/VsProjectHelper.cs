using EnvDTE;
using EnvDTE80;
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

                ProjectItem projectItem = FindProjectItemInProject(project, filePath);
                if (projectItem != null)
                {
                    return projectItem;
                }
            }

            return null;
        }

        private static ProjectItem FindProjectItemInProject(EnvDTE.Project project, string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (project == null)
            {
                return null;
            }

            foreach (ProjectItem item in project.ProjectItems)
            {
                ProjectItem foundItem = FindProjectItemInProjectItem(item, filePath);
                if (foundItem != null)
                {
                    return foundItem;
                }
            }
            
            if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    ProjectItem subProjectItem = FindProjectItemInProjectItem(item, filePath);
                    if (subProjectItem != null)
                    {
                        return subProjectItem;
                    }
                }
            }

            return null;
        }

        private static ProjectItem FindProjectItemInProjectItem(ProjectItem item, string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (item.FileCount > 0
                    && item.Kind == ProjectItemKinds.vsPhysicalFile
                    && item.FileNames[0].Equals(filePath, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }

            if (item.ProjectItems != null)
            {
                foreach (ProjectItem subItem in item.ProjectItems)
                {
                    ProjectItem foundSubItem = FindProjectItemInProjectItem(subItem, filePath);
                    if (foundSubItem != null)
                    {
                        return foundSubItem;
                    }
                }
            }

            if (item.SubProject != null)
            {
                ProjectItem projectItem = FindProjectItemInProject(item.SubProject, filePath);
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
    }
}
