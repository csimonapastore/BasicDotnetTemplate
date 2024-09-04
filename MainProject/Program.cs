using NLog;
using BasicDotnetTemplate.MainProject.Models.Settings;
using System.Reflection;
using BasicDotnetTemplate.MainProject.Utils;



namespace BasicDotnetTemplate.MainProject;

public static class ReflectionProgram
{
    public static MethodInfo LaunchConfiguration()
    {
        var a = typeof(Program);

        MethodInfo[] methods = a.GetMethods(); //Using BindingFlags.NonPublic does not show any results
        MethodInfo? initialize = null;

        foreach (MethodInfo m in methods)
        {
            if (m.Name == "Initialize")
            {
                initialize = m;
            }
        }
        return initialize!;
    }
}



internal static class Program
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public static WebApplication Initialize(string[] args)
    {
        Logger.Info("[Program][Initialize] Start building");
        var builder = WebApplication.CreateBuilder(args);

        AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder);
        ProgramUtils.AddServices(ref builder);
        ProgramUtils.AddOpenApi(ref builder, appSettings);
        ProgramUtils.AddDbContext(ref builder, appSettings);
        WebApplication app = builder.Build();
        ProgramUtils.AddMiddlewares(ref app);

        Logger.Info("[Program][Initialize] End building");
        return app;
    }


    public static void Main(string[] args)
    {
        WebApplication app = Initialize(args);
        Logger.Info("[Program][Main] Launching app");
        app.Run();
        NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
    }

}