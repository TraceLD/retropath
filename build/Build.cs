using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.PublishCli);

    [Solution] readonly Solution Solution;
    readonly Guid RetroPathCliProject = new("D2A39843-338C-4BD4-BCE2-AA80283AD1EC");

    static readonly AbsolutePath DIST_DIR = RootDirectory / "dist";
    static readonly AbsolutePath EXAMPLE_DATA_DIR = RootDirectory / "example_data";
    static readonly AbsolutePath CLI_DIR = DIST_DIR / "cli";

    const string RID_WIN_X64 = "win-x64";
    const string RID_LINUX_X64 = "linux-x64";
    const string RID_MACOS_ARM64 = "osx-arm64";
    
    static readonly Platform CLI_WINDOWS_X64 = new(CLI_DIR, RID_WIN_X64);
    static readonly Platform CLI_LINUX_X64 = new(CLI_DIR, RID_LINUX_X64);
    static readonly Platform CLI_MACOS_ARM64 = new(CLI_DIR, RID_MACOS_ARM64);

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(_ => _
                .SetProject(Solution));
            
            EnsureCleanDirectory(DIST_DIR);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target PublishCli => _ => _
        .DependsOn(Restore, PublishCliWindows64, PublishCliLinux64, PublishCliMacOsArm64)
        .Executes(() => { });

    Target PublishCliWindows64 => _ => _
        .After(Restore)
        .Executes(() =>
        {
            PublishCliForPlatform(CLI_WINDOWS_X64);
        });
    
    Target PublishCliLinux64 => _ => _
        .After(Restore)
        .Executes(() =>
        {
            PublishCliForPlatform(CLI_LINUX_X64);
        });

    Target PublishCliMacOsArm64 => _ => _
        .After(Restore)
        .Executes(() =>
        {
            PublishCliForPlatform(CLI_MACOS_ARM64);
        });
    
    void PublishCliForPlatform(Platform platform)
    {
        DotNetTasks.DotNetPublish(_ => _.SetConfiguration(Configuration.Release)
            .SetProject(Solution.GetProject(RetroPathCliProject))
            .SetRuntime(platform.Rid)
            .SetOutput(platform.TargetDir)
            .SetProperties(new Dictionary<string, object>
            {
                {"DebugType", "None"},
                {"DebugSymbols", false}
            }));
        
        CopyDirectoryRecursively(EXAMPLE_DATA_DIR, platform.TargetDir / "example_data");
    }
}