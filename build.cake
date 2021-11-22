var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var solutionFile = "DockerService.sln";
var outputDirectory = "./output/";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("CleanUp")
    .Does(() => {
    if(DirectoryExists(outputDirectory))
    {
        DeleteDirectory(outputDirectory, new DeleteDirectorySettings {
            Recursive = true,
            Force = true
        });

        Information("Output directory cleared");
    }

    DotNetCoreClean(solutionFile);
});

Task("NuGetRestore")
    .Description("Restore NuGet packages")
    .Does(() => {
    DotNetCoreRestore(solutionFile);
});

Task("Build")
    .IsDependentOn("NuGetRestore")
    .Does(() => {
    DotNetCoreBuild(solutionFile);
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() => {
    var settings = new DotNetCorePublishSettings
    {
        Configuration = "Release",
        OutputDirectory = outputDirectory
    };
    
    DotNetCorePublish(solutionFile, settings);
});

Task("Default")
    .IsDependentOn("CleanUp")
    .IsDependentOn("Build")
    .IsDependentOn("Publish");



//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);