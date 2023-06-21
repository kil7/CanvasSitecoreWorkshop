public class Configuration
{
    public string WebsiteName = "xp0.sc";

    public string SolutionName = "CanvasSitecoreWorkshop.sln";
    public string SolutionFolder = "./src"; 

    public string BuildConfiguration = "Debug";
    public string BuildToolVersion = "VS2022";

    public bool UseLocalNugetPackages = false;

    public string PublishingFolder = "c:/inetpub/wwwroot/xp0.sc";
    public string XConnectPublishFolder = "c:/inetpub/wwwroot/xp0.xconnect";

    public string XConnectJobsPublishFolder = "";
    public string XConnectModelsPublishFolder = "";
    public string XConnectIndexWorkerPublishFolder = "";

    public string TrackerApiXConnectLibsFolderPath = "";
} 