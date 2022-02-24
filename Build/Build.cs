using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Tools.Xunit;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /* Support plugins are available for:
       - JetBrains ReSharper        https://nuke.build/resharper
       - JetBrains Rider            https://nuke.build/rider
       - Microsoft VisualStudio     https://nuke.build/visualstudio
       - Microsoft VSCode           https://nuke.build/vscode
    */
    public static int Main() => Execute<Build>(x => x.Push);

    [Parameter("A branch specification such as develop or refs/pull/1775/merge")]
    readonly string BranchSpec;

    [Parameter("An incrementing build number as provided by the build engine")]
    readonly string BuildNumber;

    [Parameter("The key to push to Nuget")]
    readonly string ApiKey;

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [GitVersion(Framework = "net5.0")]
    readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath TestsDirectory => RootDirectory / "tests";

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    string SemVer;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target CalculateNugetVersion => _ => _
        .Executes(() =>
        {
            SemVer = GitVersion.SemVer;
            if (IsPullRequest)
            {
                Serilog.Log.Information(
                    "Branch spec {branchspec} is a pull request. Adding build number {buildnumber}",
                    BranchSpec, BuildNumber);

                SemVer = string.Join('.', GitVersion.SemVer.Split('.').Take(3).Union(new[] { BuildNumber }));
            }

            Serilog.Log.Information("SemVer = {semver}", SemVer);
        });

    bool IsPullRequest => BranchSpec != null && BranchSpec.Contains("pull", StringComparison.InvariantCultureIgnoreCase);

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration("CI")
                .EnableNoLogo()
                .EnableNoRestore());
        });

    Target ApiChecks => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetConfiguration("Release")
                .EnableNoBuild()
                .CombineWith(
                    cc => cc.SetProjectFile(Solution.Approval_Tests)));
        });

    Target UnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution.FluentAssertions_Json_Specs)
                .SetConfiguration("Debug")
                .EnableNoBuild()
                .SetResultsDirectory(RootDirectory / "TestResults")
                .CombineWith(
                    cc => cc.SetFramework("net47"),
                    cc => cc.SetFramework("netcoreapp3.0").SetDataCollector("XPlat Code Coverage")));
        });

    Target CodeCoverage => _ => _
        .DependsOn(UnitTests)
        .Executes(() =>
        {
            ReportGenerator(s => s
                .SetProcessToolPath(ToolPathResolver.GetPackageExecutable("ReportGenerator", "ReportGenerator.dll", framework: "net5.0"))
                .SetTargetDirectory(RootDirectory / "TestResults" / "reports")
                .AddReports(RootDirectory / "TestResults/**/coverage.cobertura.xml")
                .AddReportTypes("HtmlInline_AzurePipelines_Dark", "lcov")
                .SetClassFilters("-System.Diagnostics.CodeAnalysis.StringSyntaxAttribute")
                .SetAssemblyFilters("+FluentAssertions.Json"));

            string link = RootDirectory / "TestResults" / "reports" / "index.html";

            Serilog.Log.Information($"Code coverage report: \x1b]8;;file://{link.Replace('\\', '/')}\x1b\\{link}\x1b]8;;\x1b\\");
        });

    Target Pack => _ => _
        .DependsOn(ApiChecks)
        .DependsOn(UnitTests)
        .DependsOn(CodeCoverage)
        .DependsOn(CalculateNugetVersion)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution.FluentAssertions_Json)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration("Release")
                .EnableNoLogo()
                .EnableNoRestore()
                .EnableContinuousIntegrationBuild() // Necessary for deterministic builds
                .SetVersion(SemVer));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .OnlyWhenDynamic(() => IsTag)
        .Executes(() =>
        {
            IReadOnlyCollection<string> packages = GlobFiles(ArtifactsDirectory, "*.nupkg");

            Assert.NotEmpty(packages.ToList());

            DotNetNuGetPush(s => s
                .SetApiKey(ApiKey)
                .EnableSkipDuplicate()
                .SetSource("https://api.nuget.org/v3/index.json")
                .EnableNoSymbols()
                .CombineWith(packages,
                    (v, path) => v.SetTargetPath(path)));
        });

    bool IsTag => BranchSpec != null && BranchSpec.Contains("refs/tags", StringComparison.InvariantCultureIgnoreCase);
}
