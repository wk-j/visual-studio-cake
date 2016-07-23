using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio.Cake.Helpers
{
    public class CakeHelper
    {
        public static void Init(string solutionPath)
        {
            Func<string, string> full = name => Path.Combine(solutionPath, name);

            Download("http://cakebuild.net/download/bootstrapper/osx", full("build.sh"));
            Download("http://cakebuild.net/download/bootstrapper/windows", full("build.ps1"));
            Download("https://raw.githubusercontent.com/wk-j/cake-init/master/files/build.cake", full("build.cake"));
            Download("https://raw.githubusercontent.com/wk-j/cake-init/master/files/build.cmd", full("build.cmd"));
        }

        private static void Download(string source, string target)
        {
            if (File.Exists(target)) return;
            using (var client = new WebClient())
            {
                var uri = new Uri(source);
                client.DownloadFileAsync(uri, target);
            }
        }

        public static void ExecuteCmd(string task, string workingDir)
        {
            var process = new Process();
            process.StartInfo.FileName = "build.cmd";
            process.StartInfo.Arguments = $"-target {task}";
            process.StartInfo.WorkingDirectory = workingDir;
            process.Start();
        }

        public static void ExecutePs(string task, string workingDir)
        {
            var param = new CommandParameter("-target", task);
            var psFile = Path.Combine(workingDir, "build.ps1");
            ExecutePs(psFile, param);
        }

        private static Runspace CreateRunspace(string dir)
        {
            var runspaceConfiguration = RunspaceConfiguration.Create();
            var runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();
            runspace.SessionStateProxy.Path.SetLocation(dir);
            return runspace;
        }

        private static void ExecutePs(string scriptFile, params CommandParameter[] parameters)
        {
            var dir = new FileInfo(scriptFile).Directory.FullName;
            var runspace = CreateRunspace(dir);
            var scriptInvoker = new RunspaceInvoke(runspace);
            var pipeline = runspace.CreatePipeline();
            var command = new Command(scriptFile);

            parameters.ToList().ForEach(command.Parameters.Add);
            pipeline.Commands.Add(command);

            var rs = pipeline.Invoke();
            var output = OutputWindow();

            rs.ToList().ForEach(x =>
            {
                output(x.ToString());
            });

            pipeline.Output.ReadToEnd().ToList().ForEach(x =>
            {
                output(x.ToString());
            });

            pipeline.Error.ReadToEnd().ToList().ForEach(x => {
                output(x.ToString());
            });
        }

        private static Action<string> OutputWindow()
        {
            var outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var customGuid = new Guid("1ABDD7FB-F095-427A-B188-59CD35520C5A"); 

            IVsOutputWindowPane outputPane;
            outWindow.CreatePane(ref customGuid, "Cake" , 1, 1);
            outWindow.GetPane(ref customGuid, out outputPane);
            outputPane.Activate(); 

            return (message) => outputPane.OutputString(message);
        }
    }
}