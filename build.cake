#tool "nuget:?package=Fixie"
#addin "nuget:?package=Cake.Watch"

var solution = "VisualStudio.Cake.sln";
var testProj = "VisualStudio.Cake.Tests/VisualStudio.Cake.Tests.csproj";
var testDll = "VisualStudio.Cake.Tests/bin/Debug/VisualStudio.Cake.Tests.dll";

Task("build")
    .Does(() => {
        DotNetBuild(solution, settings => {
            settings.SetConfiguration("Debug")
                .WithTarget("Build");
        });
    });
Task("hello")
    .Does(() => {
            Console.WriteLine("Hello, world!");
    });

Task("test")
    .Does(() => {
        DotNetBuild(testProj);
        Fixie(testDll);
    });

Task("ts")
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