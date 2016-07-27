#tool "nuget:?package=Fixie"
#addin "nuget:?package=Cake.Watch"

var user = EnvironmentVariable("ghu");
var pass = EnvironmentVariable("ghp");

var solution = "VisualStudio.Cake.sln";
var testProj = "VisualStudio.Cake.Tests/VisualStudio.Cake.Tests.csproj";
var testDll = "VisualStudio.Cake.Tests/bin/Debug/VisualStudio.Cake.Tests.dll";
var package = "VisualStudio.Cake/bin/Debug/VisualStudio.Cake.vsix";

Task("build")
    .Does(() => {
        MSBuild(solution, new MSBuildSettings {
            Verbosity = Verbosity.Minimal,
            ToolVersion = MSBuildToolVersion.VS2015,
            Configuration = "Debug",
            ToolPath = @"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe",
            PlatformTarget = PlatformTarget.MSIL
        });
    });

Task(".net-build")
    .Does(() => {
        DotNetBuild(solution, settings => {
            settings.SetConfiguration("Debug")
                .WithTarget("Build");
        });
    });

Task("create-github-release")
    .IsDependentOn("build")
    .Does(() => {
        var asm = ParseAssemblyInfo("./Visualstudio.Cake/Properties/AssemblyInfo.cs");
        var version = asm.AssemblyVersion;
        var tag = string.Format("v{0}", version);
        var args = string.Format("tag -a {0} -m \"{0}\"", tag);
        var owner = "wk-j";
        var repo = "visual-studio-cake";

        StartProcess("git", new ProcessSettings {
            Arguments = args
        });

        StartProcess("git", new ProcessSettings {
            Arguments = string.Format("push https://{0}:{1}@github.com/wk-j/{2}.git {3}", user, pass, repo, tag)
        });

        GitReleaseManagerCreate(user, pass, owner , repo, new GitReleaseManagerCreateSettings {
            Name              = tag,
            InputFilePath = "RELEASE.md",
            Prerelease        = false,
            TargetCommitish   = "master",
        });
        GitReleaseManagerAddAssets(user, pass, owner, repo, tag, package);
        GitReleaseManagerPublish(user, pass, owner , repo, tag);
    });


var target = Argument("target", "default");
RunTarget(target);