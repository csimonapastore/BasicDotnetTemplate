using System;
using System.Reflection;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicDotnetTemplate.MainProject;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using Microsoft.Extensions.DependencyModel.Resolution;
using BasicDotnetTemplate.MainProject.Models.Settings;
using Microsoft.AspNetCore.Builder;
using BasicDotnetTemplate.MainProject.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using BasicDotnetTemplate.MainProject.Core.Database;
using BasicDotnetTemplate.MainProject.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace BasicDotnetTemplate.MainProject.Tests;

public static class TestUtils
{
    public static IConfiguration CreateConfiguration()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
        AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
        ProgramUtils.AddOpenApi(ref builder, appSettings);
        AppSettings _appSettings = new AppSettings();
        builder.Configuration.GetSection("AppSettings").Bind(_appSettings);
        return builder.Configuration;
    }

    public static IConfiguration CreateEmptyConfiguration(string? path = "", string? filename = "")
    {
        string appSettingsPath = String.IsNullOrEmpty(path) ? System.AppDomain.CurrentDomain.BaseDirectory : path;
        return new ConfigurationBuilder()
            .SetBasePath(appSettingsPath)
            .AddJsonFile(String.IsNullOrEmpty(filename) ? "appsettings.json" : filename, optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static string GetSqlConnectionString(IConfiguration configuration)
    {
        AppSettings _appSettings = new AppSettings();
        configuration.GetSection("AppSettings").Bind(_appSettings);
        return _appSettings.DatabaseSettings?.SqlServerConnectionString ?? String.Empty;
    }

    public static SqlServerContext CreateInMemorySqlContext()
    {
        var options = new DbContextOptionsBuilder<SqlServerContext>()
            .UseSqlite("DataSource=:memory:") // Database in-memory
            .Options;

        var context = new SqlServerContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    public static AuthService CreateAuthService()
    {
        IConfiguration configuration = CreateConfiguration();
        var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
        optionsBuilder.UseSqlServer(GetSqlConnectionString(configuration));
        SqlServerContext sqlServerContext = CreateInMemorySqlContext();
        var userServiceMock = new Mock<IUserService>();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        return new AuthService(httpContextAccessor.Object, configuration, sqlServerContext, userServiceMock.Object);
    }

    public static UserService CreateUserService()
    {
        IConfiguration configuration = CreateConfiguration();
        SqlServerContext sqlServerContext = CreateInMemorySqlContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        return new UserService(httpContextAccessor.Object, configuration, sqlServerContext);
    }

    public static JwtService CreateJwtService()
    {
        IConfiguration configuration = CreateConfiguration();
        var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
        optionsBuilder.UseSqlServer(GetSqlConnectionString(configuration));
        SqlServerContext sqlServerContext = CreateInMemorySqlContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        return new JwtService(httpContextAccessor.Object, configuration, sqlServerContext);
    }

    public static RoleService CreateRoleService()
    {
        IConfiguration configuration = CreateConfiguration();
        var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
        optionsBuilder.UseSqlServer(GetSqlConnectionString(configuration));
        SqlServerContext sqlServerContext = CreateInMemorySqlContext();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        return new RoleService(httpContextAccessor.Object, configuration, sqlServerContext);
    }
}




