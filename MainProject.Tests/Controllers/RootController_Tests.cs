using System;
using System.Reflection;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicDotnetTemplate.MainProject;
using BasicDotnetTemplate.MainProject.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using BasicDotnetTemplate.MainProject.Models.Settings;
using Microsoft.AspNetCore.Http;


namespace BasicDotnetTemplate.MainProject.Tests;

[TestClass]
public class RootController_Test
{
    [TestMethod]
    public void GetRoot_Valid()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        try
        {
            RootController rootController = new RootController();
            var result = rootController.GetRoot();
            if (result != null)
            {
                var data = (OkResult)result;
                Assert.IsTrue(data.StatusCode == StatusCodes.Status200OK);
            }
            else
            {
                Assert.Fail($"Data is null");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }
}
