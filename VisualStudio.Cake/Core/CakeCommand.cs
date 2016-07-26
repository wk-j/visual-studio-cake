using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VisualStudio.Cake.Helpers;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace VisualStudio.Cake.Core
{
    public class Guids
    {
        public static readonly Guid CommandSet = new Guid("ac5c2a80-6a6f-41af-8aff-3c3627ecdae7");
        //public static readonly uint InitId = 0x0100;
        public static readonly int DynamicStartButton = 0x0104;
    }

    internal sealed class CakeCommand
    {
        private readonly Package _package;
        private static List<OleMenuCommand> _commands;

        static Action<string> Output = CakeHelper.OutputWindow();

        private CakeCommand(Package package)
        {
            if (package == null) throw new ArgumentNullException("package");

            _package = package;

            var commandService = GetService();
            if (commandService != null)
            {
                /*
                var initId = new CommandID(Guids.CommandSet, Guids.InitId);
                var initCommand = new MenuCommand(this.InitCallback, initId);
                commandService.AddCommand(initCommand);
                */

                Output("create task command");

                var tasksId = new CommandID(Guids.CommandSet, Guids.DynamicStartButton);
                var tasksCommand = new OleMenuCommand(BuildCallback, tasksId);
                tasksCommand.Visible = false;
                tasksCommand.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(tasksCommand);
            }
        }

        private OleMenuCommandService GetService()
        {
            return this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

        }


        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            Output("before query status");

            var currentCommand = sender as OleMenuCommand;
            currentCommand.Visible = true;
            currentCommand.Text = "Init";
            currentCommand.Enabled = true;

            CreateCommands();
        }

        private void CreateCommands()
        {
            Output("create command");

            var mcs = GetService();
            if (_commands == null)
                _commands = new List<OleMenuCommand>();

            foreach (var cmd in _commands)
            {
                mcs.RemoveCommand(cmd);
            }

            var path = SolutionHelper.GetSolutionDir().FullName;
            var cake = Path.Combine(path, "build.cake");
            var list = CakeParser.ParseFile(new FileInfo(cake)).Select(x => x.Name).ToList();

            Output("task length = " + list.Count);

            var index = 1;
            foreach (var ele in list)
            {
                Output(ele);
                var menuCommandID = new CommandID(Guids.CommandSet, Guids.DynamicStartButton + index++);
                var command = new OleMenuCommand(this.BuildCallback, menuCommandID);
                command.Text = "Cake: " + ele;
                command.BeforeQueryStatus += (x, y) => { (x as OleMenuCommand).Visible = true; };
                _commands.Add(command);
                mcs.AddCommand(command);
            }
        }

        public static CakeCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider => this._package;

        public static void Initialize(Package package)
        {
            Instance = new CakeCommand(package);
        }

        private void BuildCallback(object sender, EventArgs e)
        {
            var cmd = (OleMenuCommand)sender;
            var text = cmd.Text;
            var task = text.Substring(text.IndexOf(':') + 1).Trim();

            System.Threading.Tasks.Task.Run(() =>
            {
                var dir = SolutionHelper.GetSolutionDir();
                CakeHelper.ExecuteCmd(task, dir.FullName);
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
