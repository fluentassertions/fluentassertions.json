#tool "nuget:?package=xunit.runner.console&version=2.2.0"
#tool "nuget:?package=GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var toolpath = Argument("toolpath", @"");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./Artifacts") + Directory(configuration);
GitVersion gitVersion = null; 

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("GitVersion").Does(() => {
    gitVersion = GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true
	});
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
	DotNetCoreRestore();

    NuGetRestore("./FluentAssertions.Json.sln", new NuGetRestoreSettings 
	{ 
		NoCache = true,
		Verbosity = NuGetVerbosity.Detailed,
		ToolPath = "./build/nuget.exe"
	});
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
  // Use MSBuild
  MSBuild("./FluentAssertions.Json.sln", settings => {
	settings.ToolPath = String.IsNullOrEmpty(toolpath) ? settings.ToolPath : toolpath;
	settings.ToolVersion = MSBuildToolVersion.VS2017;
	settings.PlatformTarget = PlatformTarget.MSIL;
	settings.SetConfiguration(configuration);
  });
});

Task("Run-Unit-Tests")
    .Does(() =>
{
    XUnit2("./Tests/FluentAssertions.Json.Net45.Specs/bin/" + configuration + "/*.Specs.dll", new XUnit2Settings { });
});

Task("Pack")
    .IsDependentOn("GitVersion")
    .Does(() => 
    {
      NuGetPack("./src/FluentAssertions.nuspec", new NuGetPackSettings {
        OutputDirectory = "./Artifacts",
        Version = gitVersion.NuGetVersionV2
      });  
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
	.IsDependentOn("GitVersion")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
