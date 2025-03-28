using BasicDotnetTemplate.MainProject.Models.Settings;
using Microsoft.AspNetCore.Builder;
using BasicDotnetTemplate.MainProject.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;


namespace BasicDotnetTemplate.MainProject.Tests;

[TestClass]
public class ProgramUtils_Tests
{
    [TestMethod]
    public void NoOpenApiConfig_Valid()
    {
        try
        {
            OpenApiInfo expectedOpenApiInfo = new OpenApiInfo()
            {
                Title = "MainProject",
                Description = "This template contains basic configuration for a .Net 8 backend",
                Version = "v1.0",
                Contact = null,
                TermsOfService = null,
                License = null,
                Extensions = new Dictionary<string, IOpenApiExtension>()
            };

            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings realAppSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData", "noApiConfigurationAppSettings.json");
            OpenApiInfo realOpenApiInfo = ProgramUtils.CreateOpenApiInfo(realAppSettings);

            var areEquals = expectedOpenApiInfo.Title == realOpenApiInfo.Title &&
                expectedOpenApiInfo.Description == realOpenApiInfo.Description &&
                expectedOpenApiInfo.Version == realOpenApiInfo.Version &&
                expectedOpenApiInfo.Contact == realOpenApiInfo.Contact &&
                expectedOpenApiInfo.TermsOfService == realOpenApiInfo.TermsOfService &&
                expectedOpenApiInfo.License == realOpenApiInfo.License;

            Assert.IsTrue(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void CreateOpenApiInfo_EmptyAppSettings()
    {
        try
        {
            OpenApiInfo expectedOpenApiInfo = new OpenApiInfo()
            {
                Title = null,
                Description = null,
                Version = null,
                Contact = null,
                TermsOfService = null,
                License = null,
                Extensions = new Dictionary<string, IOpenApiExtension>()
            };

            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings realAppSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData", "emptyAppsettings.json");
            OpenApiInfo realOpenApiInfo = ProgramUtils.CreateOpenApiInfo(realAppSettings);

            var areEquals = expectedOpenApiInfo.Title == realOpenApiInfo.Title &&
                expectedOpenApiInfo.Description == realOpenApiInfo.Description &&
                expectedOpenApiInfo.Version == realOpenApiInfo.Version &&
                expectedOpenApiInfo.Contact == realOpenApiInfo.Contact &&
                expectedOpenApiInfo.TermsOfService == realOpenApiInfo.TermsOfService &&
                expectedOpenApiInfo.License == realOpenApiInfo.License;

            Assert.IsTrue(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void CreateOpenApiInfo_NullSettings()
    {
        try
        {

            AppSettings appSettings = new AppSettings();
            OpenApiInfo realOpenApiInfo = ProgramUtils.CreateOpenApiInfo(appSettings);
            Assert.IsTrue(realOpenApiInfo != null);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void CreateOpenApiInfo_NullOpenApiSettings()
    {
        try
        {

            AppSettings appSettings = new AppSettings()
            {
                Settings = new Settings
                {
                    Name = "Test",
                    Description = "This is a test description",
                    Version = "v1"
                },
                OpenApiSettings = null
            };
            OpenApiInfo realOpenApiInfo = ProgramUtils.CreateOpenApiInfo(appSettings);
            Assert.IsTrue(realOpenApiInfo != null);
            Assert.IsTrue(realOpenApiInfo.Title == appSettings.Settings.Name);
            Assert.IsTrue(realOpenApiInfo.Description == appSettings.Settings.Description);
            Assert.IsTrue(realOpenApiInfo.Version == appSettings.Settings.Version);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void CreateOpenApiInfo_NullTermsOfServiceUrl()
    {
        try
        {
            AppSettings appSettings = new AppSettings()
            {
                Settings = new Settings
                {
                    Name = "Test",
                    Description = "This is a test description",
                    Version = "v1"
                },
                OpenApiSettings = new OpenApiSettings
                {
                    TermsOfServiceUrl = null,
                    OpenApiContact = null,
                    OpenApiLicense = null
                }
            };
            OpenApiInfo realOpenApiInfo = ProgramUtils.CreateOpenApiInfo(appSettings);
            Assert.IsTrue(realOpenApiInfo != null);
            Assert.IsTrue(realOpenApiInfo.Title == appSettings.Settings.Name);
            Assert.IsTrue(realOpenApiInfo.Description == appSettings.Settings.Description);
            Assert.IsTrue(realOpenApiInfo.Version == appSettings.Settings.Version);
            Assert.IsTrue(realOpenApiInfo.TermsOfService == null);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void OpenApiConfig_NotNull()
    {
        try
        {
            OpenApiInfo expectedOpenApiInfo = new OpenApiInfo()
            {
                Title = "MainProject",
                Description = "This template contains basic configuration for a .Net 8 backend",
                Version = "v1.0",
                Contact = null,
                TermsOfService = null,
                License = null,
                Extensions = new Dictionary<string, IOpenApiExtension>()
            };

            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData", "completeAppSettings.json");
            OpenApiInfo realOpenApiInfo = ProgramUtils.CreateOpenApiInfo(appSettings);

            var areEquals = expectedOpenApiInfo.Title == realOpenApiInfo.Title &&
                expectedOpenApiInfo.Description == realOpenApiInfo.Description &&
                expectedOpenApiInfo.Version == realOpenApiInfo.Version &&
                expectedOpenApiInfo.Contact == realOpenApiInfo.Contact &&
                expectedOpenApiInfo.TermsOfService == realOpenApiInfo.TermsOfService &&
                expectedOpenApiInfo.License == realOpenApiInfo.License;

            Assert.IsFalse(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void AddOpenApi_Valid()
    {
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            ProgramUtils.AddOpenApi(ref builder, appSettings);
            AppSettings _appSettings = new AppSettings();
            builder.Configuration.GetSection("AppSettings").Bind(_appSettings);
            var areEquals = appSettings.OpenApiSettings?.OpenApiContact?.Name == _appSettings.OpenApiSettings?.OpenApiContact?.Name &&
                appSettings.OpenApiSettings?.OpenApiContact?.Url == _appSettings.OpenApiSettings?.OpenApiContact?.Url &&
                appSettings.OpenApiSettings?.OpenApiLicense?.Name == _appSettings.OpenApiSettings?.OpenApiLicense?.Name &&
                appSettings.OpenApiSettings?.OpenApiLicense?.Url == _appSettings.OpenApiSettings?.OpenApiLicense?.Url &&
                appSettings.OpenApiSettings?.TermsOfServiceUrl == _appSettings.OpenApiSettings?.TermsOfServiceUrl;
            Assert.IsTrue(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void AddSqlServerContext_Valid()
    {
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            ProgramUtils.AddDbContext(ref builder, appSettings);
            AppSettings _appSettings = new AppSettings();
            builder.Configuration.GetSection("AppSettings").Bind(_appSettings);
            var areEquals = appSettings.DatabaseSettings?.SqlServerConnectionString == _appSettings.DatabaseSettings?.SqlServerConnectionString;
            Assert.IsTrue(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void AddMongoDbContext_Valid()
    {
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            ProgramUtils.AddDbContext(ref builder, appSettings);
            AppSettings _appSettings = new AppSettings();
            builder.Configuration.GetSection("AppSettings").Bind(_appSettings);
            var areEquals = appSettings.DatabaseSettings?.MongoDbConnectionString == _appSettings.DatabaseSettings?.MongoDbConnectionString;
            Assert.IsTrue(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void AddPostgreSqlDbContext_Valid()
    {
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            ProgramUtils.AddDbContext(ref builder, appSettings);
            AppSettings _appSettings = new AppSettings();
            builder.Configuration.GetSection("AppSettings").Bind(_appSettings);
            var areEquals = appSettings.DatabaseSettings?.PostgreSQLConnectionString == _appSettings.DatabaseSettings?.PostgreSQLConnectionString;
            Assert.IsTrue(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void NoSqlServerContext_Empty()
    {
        try
        {
            DatabaseSettings expectedDbSettings = new DatabaseSettings()
            {
                SqlServerConnectionString = null
            };

            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings realAppSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData", "noApiConfigurationAppSettings.json");
            ProgramUtils.AddDbContext(ref builder, realAppSettings);

            var areEquals = expectedDbSettings.SqlServerConnectionString == realAppSettings.DatabaseSettings?.SqlServerConnectionString;
            Assert.IsTrue(areEquals);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }


}




