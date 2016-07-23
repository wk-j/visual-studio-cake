using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VisualStudio.Cake.Helpers;
using System.Net;
using System.IO;

namespace VisualStudio.Cake.Core
{
    internal sealed class CakeCommand
    {
        public static readonly Guid CommandSet = new Guid("ac5c2a80-6a6f-41af-8aff-3c3627ecdae7");
        private readonly Package package;

        private CakeCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var initId = new CommandID(CommandSet, 0x0100);
                var initCommand = new MenuCommand(this.InitCallback, initId);
                commandService.AddCommand(initCommand);

                var buildId = new CommandID(CommandSet, 0x0101);
                var buildCommand = new MenuCommand(this.BuildCallback, buildId);
                commandService.AddCommand(buildCommand);
            }
        }

        public static CakeCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        public static void Initialize(Package package)
        {
            Instance = new CakeCommand(package);
        }

        private void BuildCallback(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                var dir = SolutionHelper.GetSolutionDir();
                CakeHelper.ExecutePs("build", dir.FullName);
            });
        }

        private void InitCallback(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                var dir = SolutionHelper.GetSolutionDir();
                CakeHelper.Init(dir.FullName);
            });
        }


        private void ShowMessage(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
