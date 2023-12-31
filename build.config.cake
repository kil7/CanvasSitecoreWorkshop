public class Configuration
{
    public string WebsiteName = "xp0.sc";

    public string SolutionName = "CanvasSitecoreLab.sln";
    public string SolutionFolder = "./src"; 

    public string BuildConfiguration = "Debug";
    public string BuildToolVersion = "VS2022";

    public bool UseLocalNugetPackages = false;

    public string PublishingFolder = "c:/inetpub/wwwroot/xp0.sc";
    public string XConnectPublishFolder = "c:/inetpub/wwwroot/xp0.xconnect";

    public string XConnectJobsPublishFolder = "c:/inetpub/wwwroot/sc.holidays.xconnect/App_Data/jobs/continuous/AutomationEngine";
    public string XConnectModelsPublishFolder = "c:/inetpub/wwwroot/sc.holidays.xconnect/App_Data/Models";
    public string XConnectIndexWorkerPublishFolder = "c:/inetpub/wwwroot/sc.holidays.xconnect/App_Data/jobs/continuous/IndexWorker/App_Data/Models";

    public string TrackerApiXConnectLibsFolderPath = "../tracker/lib";
} 