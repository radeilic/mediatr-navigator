using EnvDTE;
using MediatrNavigator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace MediatrNavigator
{
    internal sealed class GoToHandlerForSymbolCommand
    {
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("5b6caf8e-6b44-4c80-9d93-eb5201a84b3d");

        private GoToHandlerForSymbolCommand(OleMenuCommandService commandService)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.Visible = true;

            menuItem.BeforeQueryStatus += (s, e) =>
            {
                INamedTypeSymbol commandClassSymbol = null;
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    commandClassSymbol = await MediatrHelper.GetCommandClassSymbolAtPositionAsync(Workspace);
                });

                menuItem.Visible = commandClassSymbol != null;
            };

            commandService.AddCommand(menuItem);
        }

        public static GoToHandlerForSymbolCommand Instance
        {
            get;
            private set;
        }

        public static VisualStudioWorkspace Workspace { get; set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GoToHandlerForSymbolCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            Workspace = componentModel.GetService<VisualStudioWorkspace>();

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GoToHandlerForSymbolCommand(commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            INamedTypeSymbol commandClassSymbol = null;
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                commandClassSymbol = await MediatrHelper.GetCommandClassSymbolAtPositionAsync(Workspace);
            });
            if (commandClassSymbol == null)
            {
                return;
            }

            INamedTypeSymbol handlerSymbol = null;
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                handlerSymbol = await MediatrHelper.FindCommandHandlerSymbolAsync(commandClassSymbol, Workspace);
            });
            if (handlerSymbol == null)
            {
                return;
            }

            ProjectItem projectItem = VsProjectHelper.GetProjectItem(handlerSymbol);
            if (projectItem == null)
            {
                return;
            }

            VsProjectHelper.ActivateItem(projectItem);
        }
    }
}
