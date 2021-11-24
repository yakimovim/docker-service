#addin nuget:?package=Cake.Docker&version=1.0.0

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var solutionFile = "DockerService.sln";
var outputDirectory = "./output/";
var dockerImageName = "iakimov/testservice:latest";

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

Task("BuildDockerImage")
    .Does(() => {
    var settings = new DockerImageBuildSettings 
    {
        Tag = new string[] { dockerImageName }
    };

    DockerBuild(settings, ".");
});

Task("RunDockerImage")
    .Does(() => {
    var settings = new DockerContainerRunSettings  
    {
        Detach = true,
        Rm = true,
        Publish = new string[] { "5000:80" }
    };

    DockerRun(settings, dockerImageName, null);

    Information("The service is started on port 5000");
});

Task("PushDockerImage")
    .Does(() => {
    DockerPush(dockerImageName);
});

Task("Default")
    .IsDependentOn("CleanUp")
    .IsDependentOn("Build")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);