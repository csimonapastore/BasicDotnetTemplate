using System;
using System.Reflection;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicDotnetTemplate.MainProject;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using Microsoft.Extensions.DependencyModel.Resolution;
using BasicDotnetTemplate.MainProject.Models.Api.Common.Role;
using BasicDotnetTemplate.MainProject.Models.Api.Response.Role;
using DatabaseSqlServer = BasicDotnetTemplate.MainProject.Models.Database.SqlServer;
using BasicDotnetTemplate.MainProject.Models.Api.Response.Auth;
using BasicDotnetTemplate.MainProject.Core.Middlewares;
using AutoMapper;
using Microsoft.AspNetCore.Http;


namespace BasicDotnetTemplate.MainProject.Tests;

[TestClass]
public class GetRoleResponse_Tests
{
    private IMapper? _mapper;

    [TestInitialize]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperConfiguration>();
        });

        _mapper = config.CreateMapper();
    }

    [TestMethod]
    public void IstantiateGetRoleResponse_OnlyStatus_Valid()
    {
        try
        {
            var getRoleResponse = new GetRoleResponse(200, null, null);
            Assert.IsTrue(getRoleResponse.Status == StatusCodes.Status200OK && String.IsNullOrEmpty(getRoleResponse.Message) && getRoleResponse.Data == null);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void IstantiateGetRoleResponse_OnlyStatus_IsInvalid()
    {
        try
        {
            var getRoleResponse = new GetRoleResponse(201, null, null);
            Assert.IsFalse(getRoleResponse.Status == StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void IstantiateGetRoleResponse_StatusAndMessage_Valid()
    {
        try
        {
            var getRoleResponse = new GetRoleResponse(200, "This is a test message", null);
            Assert.IsTrue(getRoleResponse.Status == StatusCodes.Status200OK && getRoleResponse.Message == "This is a test message" && getRoleResponse.Data == null);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void IstantiateGetRoleResponse_AllFields_Valid()
    {
        try
        {
            DatabaseSqlServer.Role role = ModelsInit.CreateRole();
            RoleDto? data = _mapper?.Map<RoleDto>(role);
            var getRoleResponse = new GetRoleResponse(200, "This is a test message", data);
            Assert.IsTrue(getRoleResponse.Status == StatusCodes.Status200OK && getRoleResponse.Message == "This is a test message" && getRoleResponse.Data == data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

}




