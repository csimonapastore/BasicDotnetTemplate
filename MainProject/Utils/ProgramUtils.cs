using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using NLog;
using BasicDotnetTemplate.MainProject.Core.Database;
using BasicDotnetTemplate.MainProject.Models.Settings;



namespace BasicDotnetTemplate.MainProject.Utils;

public static class ProgramUtils
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static AppSettings AddConfiguration(ref WebApplicationBuilder builder, string? path = "", string? filename = "")
    {
        Logger.Info("[ProgramUtils][AddConfiguration] Adding configuration");

        string appSettingsPath = String.IsNullOrEmpty(path) ? System.AppDomain.CurrentDomain.BaseDirectory : path;
        var _configuration = new ConfigurationBuilder()
            .SetBasePath(appSettingsPath)
            .AddJsonFile(String.IsNullOrEmpty(filename) ? "appsettings.json" : filename, optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddSingleton<IConfiguration>(_configuration);

        PrivateSettings privateSettings = new PrivateSettings();
        _configuration.GetSection("PrivateSettings").Bind(privateSettings);

        AppSettings appSettings = new AppSettings();
        appSettings.PrivateSettings = privateSettings;
        _configuration.GetSection("AppSettings").Bind(appSettings);

        builder.Services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));
        builder.Services.Configure<PrivateSettings>(_configuration.GetSection("PrivateSettings"));

        Logger.Info("[ProgramUtils][AddConfiguration] Ended configuration");

        return appSettings;
    }

    public static OpenApiInfo CreateOpenApiInfo(AppSettings appSettings)
    {
        OpenApiInfo openApiInfo = new OpenApiInfo
        {
            Version = appSettings.Settings?.Version,
            Title = appSettings.Settings?.Name,
            Description = appSettings.Settings?.Description
        };

        if (!String.IsNullOrEmpty(appSettings.OpenApiSettings?.TermsOfServiceUrl))
        {
            openApiInfo.TermsOfService = new Uri(appSettings.OpenApiSettings.TermsOfServiceUrl);
        }

        if (!String.IsNullOrEmpty(appSettings.OpenApiSettings?.OpenApiContact?.Name) && !String.IsNullOrEmpty(appSettings.OpenApiSettings?.OpenApiContact?.Url))
        {
            openApiInfo.Contact = new OpenApiContact
            {
                Name = appSettings.OpenApiSettings.OpenApiContact.Name,
                Url = new Uri(appSettings.OpenApiSettings.OpenApiContact.Url)
            };
        }

        if (!String.IsNullOrEmpty(appSettings.OpenApiSettings?.OpenApiLicense?.Name) && !String.IsNullOrEmpty(appSettings.OpenApiSettings?.OpenApiLicense?.Url))
        {
            openApiInfo.License = new OpenApiLicense
            {
                Name = appSettings.OpenApiSettings.OpenApiLicense.Name,
                Url = new Uri(appSettings.OpenApiSettings.OpenApiLicense.Url)
            };
        }

        return openApiInfo;
    }
    public static void AddOpenApi(ref WebApplicationBuilder builder, AppSettings appSettings)
    {
        Logger.Info("[ProgramUtils][AddOpenApi] Adding swagger doc");

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", CreateOpenApiInfo(appSettings));
        });

        Logger.Info("[ProgramUtils][AddOpenApi] Ended swagger doc");
    }

    public static void AddServices(ref WebApplicationBuilder builder)
    {
        Logger.Info("[ProgramUtils][AddServices] Adding services");

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        Logger.Info("[ProgramUtils][AddServices] Done services");
    }

    public static void AddMiddlewares(ref WebApplication app)
    {
        Logger.Info("[ProgramUtils][AddMiddlewares] Adding middlewares");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.MapControllers(); // This maps all controllers

        if (app.Environment.IsDevelopment())
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.InjectStylesheet("/swagger-ui/custom.css");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }

        Logger.Info("[ProgramUtils][AddMiddlewares] Done middlewares");
    }

    public static void AddDbContext(ref WebApplicationBuilder builder, AppSettings appSettings)
    {
        Logger.Info("[ProgramUtils][AddDbContext] Adding DbContext");
        var databaseAdded = "";

        var connectionString = appSettings?.DatabaseSettings?.SqlServerConnectionString ?? String.Empty;

        if (!String.IsNullOrEmpty(connectionString))
        {
            connectionString = connectionString.Replace("SQLSERVER_DB_SERVER", Environment.GetEnvironmentVariable("SQLSERVER_DB_SERVER"));
            connectionString = connectionString.Replace("SQLSERVER_DB_DATABASE", Environment.GetEnvironmentVariable("SQLSERVER_DB_DATABASE"));
            connectionString = connectionString.Replace("SQLSERVER_DB_USER", Environment.GetEnvironmentVariable("SQLSERVER_DB_USER"));
            connectionString = connectionString.Replace("SQLSERVER_DB_PASSWORD", Environment.GetEnvironmentVariable("SQLSERVER_DB_PASSWORD"));

            builder.Services.AddDbContext<SqlServerContext>(options =>
                options.UseSqlServer(connectionString));

            databaseAdded += "SqlServer";
        }



        connectionString = appSettings?.DatabaseSettings?.MongoDbSettings?.MongoDbConnectionString ?? String.Empty;

        if (!String.IsNullOrEmpty(connectionString))
        {
            connectionString = connectionString.Replace("MONGODB_DB_SERVER", Environment.GetEnvironmentVariable("MONGODB_DB_SERVER"));
            connectionString = connectionString.Replace("MONGODB_DB_DATABASE", Environment.GetEnvironmentVariable("MONGODB_DB_DATABASE"));
            connectionString = connectionString.Replace("MONGODB_DB_USER", Environment.GetEnvironmentVariable("MONGODB_DB_USER"));
            connectionString = connectionString.Replace("MONGODB_DB_PASSWORD", Environment.GetEnvironmentVariable("MONGODB_DB_PASSWORD"));

            var mongoClient = new MongoClient(connectionString);

            var databaseName = appSettings?.DatabaseSettings?.MongoDbSettings?.DatabaseName ?? Environment.GetEnvironmentVariable("MONGODB_DB_DATABASE") ?? String.Empty;

            if (!String.IsNullOrEmpty(databaseName))
            {
                var dbContextOptions = new DbContextOptionsBuilder<MongoDbContext>()
                    .UseMongoDB(mongoClient, databaseName);
            }

            databaseAdded += (String.IsNullOrEmpty(databaseAdded) ? "" : ", ") + "MongoDB";
        }

        var message = String.IsNullOrEmpty(databaseAdded) ? "No DbContext added" : $"{databaseAdded} added";

        Logger.Info($"[ProgramUtils][AddDbContext] {message}");
    }

}