using System;
using System.Reflection;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using BasicDotnetTemplate.MainProject;
using BasicDotnetTemplate.MainProject.Controllers;
using BasicDotnetTemplate.MainProject.Services;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using BasicDotnetTemplate.MainProject.Models.Settings;
using BasicDotnetTemplate.MainProject.Models.Api.Request.Auth;
using BasicDotnetTemplate.MainProject.Models.Api.Data.Auth;
using BasicDotnetTemplate.MainProject.Models.Api.Common.User;
using BasicDotnetTemplate.MainProject.Models.Api.Common.Role;
using DatabaseSqlServer = BasicDotnetTemplate.MainProject.Models.Database.SqlServer;
using BasicDotnetTemplate.MainProject.Models.Api.Response.Auth;
using AutoMapper;
using BasicDotnetTemplate.MainProject.Core.Middlewares;
using Microsoft.AspNetCore.Http;
using BasicDotnetTemplate.MainProject.Models.Api.Request.User;
using BasicDotnetTemplate.MainProject.Models.Api.Data.User;
using BasicDotnetTemplate.MainProject.Models.Database.SqlServer;


namespace BasicDotnetTemplate.MainProject.Tests;

[TestClass]
public class UserController_Tests
{
    private Mock<IUserService>? _userServiceMock;
    private Mock<IRoleService>? _roleServiceMock;
    private UserController? _userController;

    [TestInitialize]
    public void Setup()
    {
        IConfiguration configuration = TestUtils.CreateConfiguration();
        _userServiceMock = new Mock<IUserService>();
        _roleServiceMock = new Mock<IRoleService>();
        _userController = new UserController(configuration, _userServiceMock.Object, _roleServiceMock.Object);
    }

    [TestMethod]
    public void UserController_NullConfiguration()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        var exception = true;
        try
        {
            var userServiceMock = new Mock<IUserService>();
            var roleServiceMock = new Mock<IRoleService>();
            _ = new UserController(null, userServiceMock.Object, roleServiceMock.Object);
            exception = false;
            Assert.Fail($"This test should not pass as configuration is null");
        }
        catch (Exception)
        {
            Assert.IsTrue(exception);
        }
    }


    [TestMethod]
    public async Task GetUserByGuidAsync_Should_Return_200_When_Successful()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }
        var guid = Guid.NewGuid().ToString();
        DatabaseSqlServer.User user = ModelsInit.CreateUser();

        _userServiceMock?.Setup(s => s.GetUserByGuidAsync(It.IsAny<string>())).ReturnsAsync(user);
        ObjectResult response = (ObjectResult)(await _userController.GetUserByGuidAsync(guid));
        if (response != null && response.Value != null)
        {
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);

            var result = (BaseResponse<object>)response.Value;
            if (result != null)
            {
                Assert.AreEqual(StatusCodes.Status200OK, result.Status);
                Assert.IsInstanceOfType(result.Data, typeof(UserDto));
            }
            else
            {
                Assert.Fail($"Result value is null");
            }
        }
        else
        {
            Assert.Fail($"Response value is null");
        }
    }

    [TestMethod]
    public async Task GetUserByGuidAsync_NotFound()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }

        var guid = Guid.NewGuid().ToString();
        DatabaseSqlServer.User? user = null;
        _userServiceMock?.Setup(s => s.GetUserByGuidAsync(It.IsAny<string>())).ReturnsAsync(user);
        NotFoundResult response = (NotFoundResult)(await _userController.GetUserByGuidAsync(guid));

        Assert.IsInstanceOfType(response, typeof(NotFoundResult));

        if (response != null)
        {
            Assert.AreEqual(StatusCodes.Status404NotFound, response.StatusCode);
        }
        else
        {
            Assert.Fail($"Response is null");
        }
    }

    [TestMethod]
    public async Task GetUserByGuidAsync_Exception()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }

        var guid = Guid.NewGuid().ToString();
        _userServiceMock?.Setup(s => s.GetUserByGuidAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Unexpected error"));
        ObjectResult response = (ObjectResult)(await _userController.GetUserByGuidAsync(guid));

        Assert.IsInstanceOfType(response, typeof(ObjectResult));

        if (response != null && response.Value != null)
        {
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);

            var result = (BaseResponse<object>)response.Value;
            if (result != null)
            {
                Assert.AreEqual(StatusCodes.Status500InternalServerError, result.Status);
                Assert.AreEqual("Something went wrong. Unexpected error", result.Message);
            }
            else
            {
                Assert.Fail($"Result value is null");
            }
        }
        else
        {
            Assert.Fail($"Response is null");
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_Success()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }

        if (_roleServiceMock == null)
        {
            Assert.Fail($"_roleServiceMock is null");
        }

        DatabaseSqlServer.User user = ModelsInit.CreateUser();
        DatabaseSqlServer.Role role = ModelsInit.CreateRole();

        CreateUserRequest request = new CreateUserRequest()
        {
            Data = new CreateUserRequestData()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            }
        };

        _userServiceMock?.Setup(s => s.CheckIfEmailIsValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _roleServiceMock.Setup(s => s.GetRoleForUser(null)).ReturnsAsync(role);
        _userServiceMock?.Setup(s => s.CreateUserAsync(request.Data, role)).ReturnsAsync(user);

        ObjectResult response = (ObjectResult)(await _userController.CreateUserAsync(request));
        if (response != null && response.Value != null)
        {
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);

            var result = (BaseResponse<object>)response.Value;
            if (result != null)
            {
                Assert.AreEqual(StatusCodes.Status200OK, result.Status);
                Assert.IsInstanceOfType(result.Data, typeof(UserDto));
            }
            else
            {
                Assert.Fail($"Result value is null");
            }
        }
        else
        {
            Assert.Fail($"Response value is null");
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_InvalidEmail()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }

        DatabaseSqlServer.User user = ModelsInit.CreateUser();

        CreateUserRequest request = new CreateUserRequest()
        {
            Data = new CreateUserRequestData()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            }
        };

        _userServiceMock?.Setup(s => s.CheckIfEmailIsValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        ObjectResult response = (ObjectResult)(await _userController.CreateUserAsync(request));

        if (response != null && response.Value != null)
        {
            Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);

            var result = (BaseResponse<object>)response.Value;
            if (result != null)
            {
                Assert.AreEqual(StatusCodes.Status400BadRequest, result.Status);
                Assert.AreEqual("Invalid email", result.Message);
            }
            else
            {
                Assert.Fail($"Result value is null");
            }
        }
        else
        {
            Assert.Fail($"Response value is null");
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_RoleNull()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }

        DatabaseSqlServer.User user = ModelsInit.CreateUser();

        CreateUserRequest request = new CreateUserRequest()
        {
            Data = new CreateUserRequestData()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            }
        };

        _userServiceMock?.Setup(s => s.CheckIfEmailIsValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _userServiceMock?.Setup(s => s.CreateUserAsync(
            It.IsAny<CreateUserRequestData>(),
            It.IsAny<Role>()
        )).ReturnsAsync(user);

        ObjectResult response = (ObjectResult)(await _userController.CreateUserAsync(request));

        if (response != null && response.Value != null)
        {
            Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);

            var result = (BaseResponse<object>)response.Value;
            if (result != null)
            {
                Assert.AreEqual(StatusCodes.Status400BadRequest, result.Status);
                Assert.AreEqual("Role not found", result.Message);
            }
            else
            {
                Assert.Fail($"Result value is null");
            }
        }
        else
        {
            Assert.Fail($"Response value is null");
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_NotCreated()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }

        if (_roleServiceMock == null)
        {
            Assert.Fail($"_roleServiceMock is null");
        }

        DatabaseSqlServer.User user = ModelsInit.CreateUser();
        DatabaseSqlServer.Role role = ModelsInit.CreateRole();
        DatabaseSqlServer.User? expectedUser = null;

        CreateUserRequest request = new CreateUserRequest()
        {
            Data = new CreateUserRequestData()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            }
        };

        _userServiceMock?.Setup(s => s.CheckIfEmailIsValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _roleServiceMock.Setup(s => s.GetRoleForUser(null)).ReturnsAsync(role);
        _userServiceMock?.Setup(s => s.CreateUserAsync(request.Data, role)).ReturnsAsync(expectedUser);

        ObjectResult response = (ObjectResult)(await _userController.CreateUserAsync(request));
        if (response != null && response.Value != null)
        {
            Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);

            var result = (BaseResponse<object>)response.Value;
            if (result != null)
            {
                Assert.AreEqual(StatusCodes.Status400BadRequest, result.Status);
                Assert.AreEqual("Not created", result.Message);
            }
            else
            {
                Assert.Fail($"Result value is null");
            }
        }
        else
        {
            Assert.Fail($"Response value is null");
        }
    }


    [TestMethod]
    public async Task CreateUserAsync_Exception()
    {
        if (_userController == null)
        {
            Assert.Fail($"_userController is null");
        }

        if (_roleServiceMock == null)
        {
            Assert.Fail($"_roleServiceMock is null");
        }

        DatabaseSqlServer.User user = ModelsInit.CreateUser();
        DatabaseSqlServer.Role role = ModelsInit.CreateRole();

        CreateUserRequest request = new CreateUserRequest()
        {
            Data = new CreateUserRequestData()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password
            }
        };

        _userServiceMock?.Setup(s => s.CheckIfEmailIsValid(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _roleServiceMock.Setup(s => s.GetRoleForUser(null)).ReturnsAsync(role);
        _userServiceMock?.Setup(s => s.CreateUserAsync(request.Data, role)).ReturnsAsync(user);

        _userServiceMock?.Setup(s => s.CreateUserAsync(
            It.IsAny<CreateUserRequestData>(),
            It.IsAny<Role>()
        )).ThrowsAsync(new Exception("Unexpected error"));

        ObjectResult response = (ObjectResult)(await _userController.CreateUserAsync(request));
        Assert.IsInstanceOfType(response, typeof(ObjectResult));

        if (response != null && response.Value != null)
        {
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);

            var result = (BaseResponse<object>)response.Value;
            if (result != null)
            {
                Assert.AreEqual(StatusCodes.Status500InternalServerError, result.Status);
                Assert.AreEqual("Something went wrong. Unexpected error", result.Message);
            }
            else
            {
                Assert.Fail($"Result value is null");
            }
        }
        else
        {
            Assert.Fail($"Response is null");
        }
    }
}
