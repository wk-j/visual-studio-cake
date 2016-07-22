#tool "nuget:?package=Fixie"
#addin "nuget:?package=Cake.Watch"

var solution = "VisualStudio.Cake.sln";
var testDll = "TrySelectMany.Tests/bin/Debug/TrySelectMany.Tests.dll";

Task("build")
    .Does(() => {
        DotNetBuild(solution, settings => {
            settings.SetConfiguration("Debug")
                .WithTarget("Build");
        //MSBuild(solution);
        });
    });

Task("test")
    .Does(() => {
        Fixie(testDll);
    });

Task("watch")
    .Does(() => {
        var settings = new WatchSettings {
            Recursive = true,
            Path = "./",
            Pattern = "*Tests.cs"
        };
        Watch(settings, (changed) => {
            RunTarget("test");
        });
    });

var target = Argument("target", "default");
RunTarget(target);