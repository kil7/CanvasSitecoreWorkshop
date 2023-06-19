using System.Text.RegularExpressions;

// Get Valid MSBuildToolVersion based on Configuration BuildToolVersion
public MSBuildToolVersion GetValidMSBuildToolVersion()
{
    var msBuildToolVersion = MSBuildToolVersion.Default;
    if(Enum.TryParse(configuration.BuildToolVersion, out msBuildToolVersion)) 
    {
        return msBuildToolVersion;
    }
    else
    {
        return MSBuildToolVersion.Default;
    }
}

// Initialize MSBuild Settings from Configuration Object
public MSBuildSettings InitializeMSBuildSettings(MSBuildSettings settings)
{
    settings.SetConfiguration(configuration.BuildConfiguration)
            .SetVerbosity(Verbosity.Minimal)
            .SetMSBuildPlatform(MSBuildPlatform.Automatic)
            .SetPlatformTarget(PlatformTarget.MSIL)
            .UseToolVersion(GetValidMSBuildToolVersion())
            .WithRestore();

    return settings;
}

// Initialize Globber Settings with Exclude Predicate
public GlobberSettings InitializeGlobberSettings()
{
    Func<IFileSystemInfo, bool> filesToExclude = x => !x.Path.FullPath.Contains("/obj/") && !x.Path.FullPath.Contains("/bin/");
    return new GlobberSettings { Predicate = filesToExclude };
}

// Publish Projects from specific Root Folder
public void PublishProjects(string layerFolder)
{
    var projects = GetFiles($@"{layerFolder}\**\code\*.csproj");
    foreach (var project in projects)
    {
        Information($"Publishing project: {project.GetFilename()}");
        MSBuild(project, settings => InitializeMSBuildSettings(settings)
        .WithTarget("Build")
        .WithProperty("PublishUrl", configuration.PublishingFolder)
        .WithProperty("DeployOnBuild", "true")
        .WithProperty("DeployDefaultTarget", "WebPublish")
        .WithProperty("WebPublishMethod", "FileSystem")
        .WithProperty("DeleteExistingFiles", "false")
        .WithProperty("BuildProjectReferences", "true"));
    }
}


// Publish XConnect Projects from specific Root Folder
public void PublishXConnectProjects()
{
    var projects = GetFiles($@"{configuration.SolutionFolder}\Foundation\xconnect\code\*.csproj");
   
    foreach (var project in projects)
    {
        Information($"Publishing XConnect project: {project.GetFilename()}");
        MSBuild(project, settings => InitializeMSBuildSettings(settings)
        .WithTarget("Build")
        .WithProperty("PublishUrl", configuration.XConnectPublishFolder)
        .WithProperty("DeployOnBuild", "true")
        .WithProperty("DeployDefaultTarget", "WebPublish")
        .WithProperty("WebPublishMethod", "FileSystem")
        .WithProperty("DeleteExistingFiles", "false")
        .WithProperty("BuildProjectReferences", "true"));
    }
}

public void PublishXConnectModels(){
    var projectsModelsFilter = $@"{configuration.SolutionFolder}\Foundation\XConnect\code\App_Data\Models\*.json";
  
    
    Information($"Publishing XConnect models: {configuration.XConnectModelsPublishFolder}");
    CreateFolder(configuration.XConnectModelsPublishFolder);
    CopyFilesToFolder(projectsModelsFilter, configuration.XConnectModelsPublishFolder);

    Information($"Publishing XConnect index worker models: {configuration.XConnectIndexWorkerPublishFolder}");    
    CreateFolder(configuration.XConnectIndexWorkerPublishFolder);
    CopyFilesToFolder(projectsModelsFilter, configuration.XConnectIndexWorkerPublishFolder);
}

public void PublishXConnectJobs(){
    var projectsDllFilter = $@"{configuration.SolutionFolder}\Foundation\XConnect\code\bin\*.dll";
  
    Information($"Publishing XConnect job: {configuration.XConnectJobsPublishFolder}");
    CreateFolder(configuration.XConnectJobsPublishFolder);
    CopyFilesToFolder(projectsDllFilter, configuration.XConnectJobsPublishFolder);
}

public void PublishXConnectJobsToTrackerApi() {    
    var xConnectDllsFilter = $@"{configuration.SolutionFolder}\Foundation\XConnect\code\bin\easyJet.*.dll";

    var directoryPath = MakeAbsolute(new DirectoryPath(configuration.TrackerApiXConnectLibsFolderPath)).FullPath;
    Information($"Publishing XConnectJobs To TrackerApi: {directoryPath}");
    CreateFolder(directoryPath);
    CopyFilesToFolder(xConnectDllsFilter, directoryPath);
}

// Check if Exists and Create Directory
public void CreateFolder(string folderPath)
{
    if (!DirectoryExists(folderPath))
    {
        CreateDirectory(folderPath);
    }
}

// Copy Files to Folder
public void CopyFilesToFolder(string filesFilter, string destination)
{
    var files = GetFiles(filesFilter, InitializeGlobberSettings()).ToList();

    if(files.Any()){
        files.ForEach(x => Information($"Copying file: {x.FullPath}"));
        CopyFiles(files , destination, preserveFolderStructure: true);
    }
}

// Copy Files to Folder
public void CopyFilesToAppConfigFolder(string filesFilter, string destination)
{
    var files = GetFiles(filesFilter, InitializeGlobberSettings()).ToList();

    foreach(var file in files)
    {
        Information($"Copying file: {file.FullPath}");

        var destinationFileRegex = Regex.Replace(file.FullPath, $".+App_Config/(.+)/*", "$1");
        FilePath destinationFile = $@"{destination}\{destinationFileRegex.Replace("/", "\\")}";

        CreateFolder(destinationFile.GetDirectory().FullPath);
        CopyFile(file.FullPath, destinationFile);  
    }
}

//Rebuild provided Index
public void RebuildIndex(string indexName)
{
    var url = $"https://{configuration.WebsiteName}/api/utilities/indexrebuild?indexName={indexName}";
    Console.WriteLine("URL: " + url);
    string responseBody = HttpGet(url);
}
