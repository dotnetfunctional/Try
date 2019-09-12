#module nuget:?package=Cake.DotNetTool.Module&version=0.3.0

#tool nuget:?package=xunit.runner.console&version=2.4.1
#tool dotnet:?package=GitVersion.Tool&version=5.0.0-beta3-4

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var assemblyVersion = "0.0.0";
var packageVersion = "0.0.0";

// Define globs
var srcProjectGlob = "./src/*/*.csproj";
var testProjectGlob = "./test/*/*.Test.csproj";

// Define directories.
var solutionFile = File("./DotNetFunctional.Try.sln");
var artifactsDir = MakeAbsolute(Directory("artifacts"));
var srcDir = MakeAbsolute(Directory("src"));
var testDir = MakeAbsolute(Directory("test"));
var coverageDir = MakeAbsolute(Directory("tests-results"));

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);
        var settings = new DotNetCoreCleanSettings
        {
            Configuration = configuration
        };

        DotNetCoreClean(solutionFile, settings);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore(solutionFile);
    });

Task("SemVer")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var settings = new GitVersionSettings
        {
            NoFetch = true
        };

        var gitVersion = GitVersion(settings);

        assemblyVersion = gitVersion.AssemblySemVer;
        packageVersion = gitVersion.NuGetVersion;

        Information($"AssemblySemVer: {assemblyVersion}");
        Information($"NuGetVersion: {packageVersion}");
    });

Task("SetAppVeyorVersion")
    .IsDependentOn("SemVer")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
    {
        AppVeyor.UpdateBuildVersion(packageVersion);
    });

Task("Build")
    .IsDependentOn("SetAppVeyorVersion")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoIncremental = true,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .SetVersion(assemblyVersion)
                .WithProperty("FileVersion", packageVersion)
                .WithProperty("InformationalVersion", packageVersion)
                .WithProperty("nowarn", "7035")
        };

        if (IsRunningOnLinuxOrDarwin())
        {
            settings.Framework = "netstandard2.0";

            GetFiles(srcProjectGlob)
                .ToList()
                .ForEach(file => DotNetCoreBuild(file.FullPath, settings));

            settings.Framework = "netcoreapp2.2";

            GetFiles(testProjectGlob)
                .ToList()
                .ForEach(file => DotNetCoreBuild(file.FullPath, settings));
        }
        else
        {
            DotNetCoreBuild(solutionFile, settings);
        }
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            TestAdapterPath = Directory(".").Path
        };

        if (IsRunningOnLinuxOrDarwin())
        {
            settings.Framework = "netcoreapp2.2";
        }

        GetFiles(testProjectGlob)
            .ToList()
            .ForEach(projectFile => {
                // Based on https://stackoverflow.com/a/55285729/5394220
                var testResultsFile = coverageDir.Combine($"{projectFile.GetFilenameWithoutExtension()}.xml");
                settings.Logger = $"\"xunit;LogFilePath={testResultsFile}\"";

                DotNetCoreTest(projectFile.FullPath, settings);
            });
    });

Task("Pack")
    .IsDependentOn("Test")
    .WithCriteria(() => HasArgument("pack"))
    .Does(() =>
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            OutputDirectory = artifactsDir,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .WithProperty("PackageVersion", packageVersion)
        };

        if (IsRunningOnLinuxOrDarwin())
        {
            settings.MSBuildSettings.WithProperty("TargetFrameworks", "netstandard2.0");
        }

        GetFiles(srcProjectGlob)
            .ToList()
            .ForEach(f => DotNetCorePack(f.FullPath, settings));
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

private bool IsRunningOnLinuxOrDarwin()
{
    return Context.Environment.Platform.IsUnix();
}