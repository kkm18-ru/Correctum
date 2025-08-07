#addin nuget:?package=Cake.VersionReader&version=5.1.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var path = Argument("path", "TEMP/publish");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Teardown(ctx =>
{
	DeleteDirectory("TEMP", new DeleteDirectorySettings
	{
		Force = true, 
		Recursive = true
	});
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Publish")
.Does(() => {
	DotNetPublish("RetailCorrector", new DotNetPublishSettings
	{
		Configuration = configuration,
		OutputDirectory = path,
		SelfContained = true,
		Runtime = "win-x86"
	});
	string version = GetVersionNumber($"{path}/RetailCorrector.exe");
	InnoSetup("RetailCorrector.iss", new InnoSetupSettings 
	{
		Defines = new Dictionary<string, string>
		{
			{ "AppVersion", version }
		}
	});
	MoveFileToDirectory($"TEMP/RetailCorrector-{version}.exe", "\\\\192.168.2.89\\share_egais\\Distr\\RetailCorrector");
});

Task

RunTarget(target);