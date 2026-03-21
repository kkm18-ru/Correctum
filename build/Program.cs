using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core;
using Cake.Frosting;

public static class Constants
{
    public const string PresentationPath = "../src/Correctum.Presentation/Correctum.Presentation.csproj";
    public const string SolutionPath = "../Correctum.slnx";
}

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string Version { get; set; }
    public BuildContext(ICakeContext context) : base(context) 
    { 
        Version = context.Arguments.GetArgument("Version");
    }
}

[TaskName("Build")]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(Constants.SolutionPath);
    }
}

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTest(Constants.SolutionPath, new DotNetTestSettings
        {
            NoBuild = true
        });
    }
}

[TaskName("Publish")]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var version = context.Configuration.GetValue("version");
        context.DotNetPublish(Constants.PresentationPath, new DotNetPublishSettings
        {
            SelfContained = true,
            Runtime = "win-x86",
            Configuration = "Release",
            NoLogo = true,
            OutputDirectory = "../publish",
            MSBuildSettings = new DotNetMSBuildSettings
            {
                Version = version
            }
        });
        context.DeleteFiles(new Cake.Core.IO.GlobPattern("../publish/*.xml"));
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(TestTask))]
public sealed class DefaultTask : FrostingTask<BuildContext>;