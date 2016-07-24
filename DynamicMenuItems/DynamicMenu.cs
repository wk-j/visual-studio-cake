using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE80;
using EnvDTE;

namespace DynamicMenuItems
{
    class DynamicCommandGuid
    {
        public const string guidDynamicMenuPackageCmdSet = "ec11b265-47d0-44a2-8f7e-930e5ae29806";  // get the GUID from the .vsct file
        public const uint cmdidMyCommand = 0x104;
    }

    internal sealed class DynamicMenu
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("ec11b265-47d0-44a2-8f7e-930e5ae29806");

        private readonly Package _package;

        private DTE2 _dte2;
        private int _rootItemId = 0;


        private DynamicMenu(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this._package = package;

            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var dynamicItemRootId = new CommandID(new Guid(DynamicCommandGuid.guidDynamicMenuPackageCmdSet), (int) DynamicCommandGuid.cmdidMyCommand);
                var dynamicMenuCommand = new DynamicItemMenuCommand(
                    dynamicItemRootId,
                    IsValidDynamicItem,
                    OnInvokeDynamicItem,
                    OnBeforeQueryStatusDynamicItem);

                commandService.AddCommand(dynamicMenuCommand);
            }
        }

        private bool IsValidDynamicItem(int commandId)
        {
            return (commandId >= (int)DynamicCommandGuid.cmdidMyCommand) && ((commandId - (int)DynamicCommandGuid.cmdidMyCommand) < _dte2.Solution.Projects.Count); 
        }

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs e)
        {
            var matchedCommand = (DynamicItemMenuCommand)sender;
            matchedCommand.Enabled = true;
            matchedCommand.Visible = true;

            var isRootItem = (matchedCommand.MatchedCommandId == 0);
            var indexForDisplay = (isRootItem ? 1 : (matchedCommand.MatchedCommandId - (int)DynamicCommandGuid.cmdidMyCommand));
            matchedCommand.Text = _dte2.Solution.Projects.Item(indexForDisplay).Name;
            var startupProjects = (Array)_dte2.Solution.SolutionBuild.StartupProjects;
            var startupProject = System.IO.Path.GetFileNameWithoutExtension((string)startupProjects.GetValue(0));
            matchedCommand.Checked = (matchedCommand.Text == startupProject);
            matchedCommand.MatchedCommandId = 0;
        }

        private void OnInvokeDynamicItem(object sender, EventArgs e)
        {
            var invokeCommand = (DynamicItemMenuCommand)sender;
            if (invokeCommand.Checked) return;

            var projects = _dte2.Solution.Projects;
            foreach (Project proj in projects)
            {
                if(invokeCommand.Text.Equals(proj.Name))
                {
                    _dte2.Solution.SolutionBuild.StartupProjects = proj.FullName;
                    return;
                }
            }
        }

        public static DynamicMenu Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return this._package;
            }
        }

        public static void Initialize(Package package)
        {
            Instance = new DynamicMenu(package);
        }
    }
}
