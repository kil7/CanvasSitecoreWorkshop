// Loading Cake Add-Ins and External Libraries
#addin nuget:?package=Cake.Powershell&version=2.0.0&loaddependencies=true
#addin nuget:?package=Cake.Http&version=3.0.2&loaddependencies=true
#addin nuget:?package=Cake.Services&version=1.0.1&loaddependencies=true

// Loading Configuration and Helper Methods
#load local:?path=scripts/Cake/utilities.cake

// Loading Local Variables
#load local:?path=build.config.cake

#tool nuget:?package=NuGet.CommandLine&version=5.9.1

// Initializing Local Variables
var configuration = new Configuration();


// Initializing Cake Execution Arguments 
var target = Argument<string>("Target", "_Build");

// Initializing Setup
Setup(context =>
{
    configuration.BuildToolVersion = Argument<string>("BuildToolVersion", configuration.BuildToolVersion);
    configuration.BuildConfiguration = Argument<string>("BuildConfiguration", configuration.BuildConfiguration);
    configuration.PublishingFolder = Argument<string>("PublishingFolder", configuration.PublishingFolder);
    configuration.XConnectPublishFolder = Argument<string>("XConnectPublishFolder", configuration.XConnectPublishFolder);
    configuration.UseLocalNugetPackages = Argument<bool>("UseLocalNugetPackages", configuration.UseLocalNugetPackages);

    Information("Build is running under the following configuration:");
    Information($"Build Tool Version: {configuration.BuildToolVersion}");
    Information($"Build Configuration: {configuration.BuildConfiguration}");
    Information($"Publishing Folder: {configuration.PublishingFolder}");
    Information($"XConnectPublish Folder: {configuration.XConnectPublishFolder}");
    Information($"Use Local Nuget Packages: {configuration.UseLocalNugetPackages}");
});

// Task: Restore-NuGet-Packages
Task("Restore-NuGet-Packages")
.Does(() => 
{
    Information($"Restoring NuGet Packages for solution: {configuration.SolutionName}");

    var nuSettigns = new NuGetRestoreSettings { NoCache = true };
    if (configuration.UseLocalNugetPackages) {
        nuSettigns.Source = new List<string> { "C:\\Windows\\system32\\config\\systemprofile\\.nuget\\packages" };
    }

    NuGetRestore("CanvasSiteCoreLab.sln", nuSettigns);
});


// Task: Clean-Solution
Task("Clean-Solution")
.Does(() => 
{
    Information($"Cleaning solution: {configuration.SolutionName}");
    CleanDirectories($"{configuration.SolutionFolder}/**/obj");
    CleanDirectories($"{configuration.SolutionFolder}/**/bin");

});


// Task: Build Solution
Task("Build-Solution")
.Does(() => 
{
    Information($"Building solution: {configuration.SolutionName}");
    MSBuild(configuration.SolutionName, settings => InitializeMSBuildSettings(settings));
});


// Task: Publish-All-Projects
Task("Publish-All-Projects")
  .IsDependentOn("Publish-Foundation-Projects")
  .IsDependentOn("Publish-Feature-Projects")
  .IsDependentOn("Publish-Project-Projects");


// Task: Publish-Foundation-Projects
Task("Publish-Foundation-Projects")
.Does(() => 
{
    var foundationSolutionFolder = $@"{configuration.SolutionFolder}\Foundation";
    PublishProjects(foundationSolutionFolder);
});


// Task: Publish-Feature-Projects
Task("Publish-Feature-Projects")
.Does(() => 
{
    var featureSolutionFolder = $@"{configuration.SolutionFolder}\Feature";
    PublishProjects(featureSolutionFolder);
});

// Task: Publish-Project-Projects
Task("Publish-Project-Projects")
.Does(() => 
{
    var projectSolutionFolder = $@"{configuration.SolutionFolder}\Project";
    PublishProjects(projectSolutionFolder);
});


// Task: Publish-All-Configs
Task("Publish-All-Configs")
.Does(() => 
{
    var filesFilter = $@"{configuration.SolutionFolder}\**\App_Config\**\*.config";
    var destination = $@"{configuration.PublishingFolder}\App_Config";
    
    CreateFolder(destination);
    CopyFilesToAppConfigFolder(filesFilter, destination);
});

// Task: Cleanup-Artifacts
Task("Cleanup-Projects-Artifacts")
.Does(() => 
{
    var files = GetFiles($@"{configuration.PublishingFolder}\bin\Sitecore.*.dll");
    DeleteFiles(files);
});

Task("Stop-Services").Does(() => {
    StopService("XP0.xconnect-MarketingAutomationService");
});

Task("Start-Services").Does(() => {
    StartService("XP0.xconnect-MarketingAutomationService");
});

// Task: Build - Sequential Tasks
Task("_Build")
.WithCriteria(configuration != null)
.IsDependentOn("Stop-Services")
.IsDependentOn("Restore-NuGet-Packages")
.IsDependentOn("Clean-Solution")
.IsDependentOn("Build-Solution")
.IsDependentOn("Publish-All-Projects")
.IsDependentOn("Start-Services");


Task("_Release")
.WithCriteria(configuration != null)
.IsDependentOn("Restore-NuGet-Packages")
.IsDependentOn("Clean-Solution")
.IsDependentOn("Build-Solution")
.IsDependentOn("Publish-All-Projects")
.IsDependentOn("Cleanup-Projects-Artifacts");

// Run Target Task
RunTarget(target);