using BasicDotnetTemplate.MainProject.Services;
using BasicDotnetTemplate.MainProject.Models.Api.Data.User;
using BasicDotnetTemplate.MainProject.Models.Database.SqlServer;
using Newtonsoft.Json;



namespace BasicDotnetTemplate.MainProject.Tests;

[TestClass]
public class UserService_Tests
{
    private static User _user = ModelsInit.CreateUser();
    private static UserService _userService = TestUtils.CreateUserService();

    [TestMethod]
    public void Inizialize()
    {
        try
        {
            var userService = TestUtils.CreateUserService();
            if (userService != null)
            {
                Assert.IsInstanceOfType(userService, typeof(UserService));
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task GetUserByUsernameAndPassword_Null()
    {
        try
        {
            var testString = "test";
            if (_userService != null)
            {
                var user = await _userService.GetUserByUsernameAndPassword(testString, testString);
                Assert.IsTrue(user == null);
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task CheckIfEmailIsValid_EmailNotExists()
    {
        try
        {
            if (_userService != null)
            {
                var valid = await _userService.CheckIfEmailIsValid(_user.Email ?? String.Empty);
                Assert.IsTrue(valid);
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_Success()
    {
        try
        {

            var expectedUser = ModelsInit.CreateUser();

            CreateUserRequestData data = new CreateUserRequestData()
            {
                FirstName = expectedUser.FirstName ?? String.Empty,
                LastName = expectedUser.LastName ?? String.Empty,
                Email = expectedUser.Email ?? String.Empty
            };

            Role role = new()
            {
                Name = expectedUser.Role?.Name ?? String.Empty,
                IsNotEditable = expectedUser.Role?.IsNotEditable ?? false,
                Guid = expectedUser.Role?.Guid ?? String.Empty
            };

            var user = await _userService.CreateUserAsync(data, role);
            Assert.IsInstanceOfType(user, typeof(User));
            Assert.IsNotNull(user);
            Assert.IsTrue(expectedUser.FirstName == user.FirstName);
            Assert.IsTrue(expectedUser.LastName == user.LastName);
            Assert.IsTrue(expectedUser.Email == user.Email);
            Assert.IsTrue(expectedUser.Role?.Name == user.Role?.Name);
            _user = user;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_Exception()
    {
        try
        {
            var expectedUser = ModelsInit.CreateUser();

            CreateUserRequestData data = new CreateUserRequestData()
            {
                FirstName = expectedUser.FirstName ?? String.Empty,
                LastName = expectedUser.LastName ?? String.Empty,
                Email = expectedUser.Email ?? String.Empty
            };

            Role role = new()
            {
                Name = expectedUser.Role?.Name ?? String.Empty,
                IsNotEditable = expectedUser.Role?.IsNotEditable ?? false,
                Guid = expectedUser.Role?.Guid ?? String.Empty
            };

            var exceptionUserService = TestUtils.CreateUserServiceException();

            if (exceptionUserService != null)
            {
                try
                {
                    var user = await exceptionUserService.CreateUserAsync(data, role);
                    Assert.Fail($"Expected exception instead of response: {user?.Guid}");
                }
                catch (Exception exception)
                {
                    Assert.IsInstanceOfType(exception, typeof(Exception));
                }
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task CheckIfEmailIsValid_EmailCurrentUser()
    {
        try
        {

            if (_userService != null)
            {
                var valid = await _userService.CheckIfEmailIsValid(_user.Email ?? String.Empty, _user.Guid ?? String.Empty);
                Assert.IsTrue(valid);
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task CheckIfEmailIsValid_EmailAlreadyExists()
    {
        try
        {

            if (_userService != null)
            {
                var valid = await _userService.CheckIfEmailIsValid(_user.Email ?? String.Empty);
                Assert.IsFalse(valid);
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task GetUserByIdAsync()
    {
        try
        {

            if (_userService != null)
            {
                var user = await _userService.GetUserByIdAsync(_user.Id);
                Assert.IsNotNull(user);
                Assert.IsTrue(user.Id == _user?.Id);
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task GetUserByGuidAsync()
    {
        try
        {

            if (_userService != null)
            {
                var user = await _userService.GetUserByGuidAsync(_user.Guid ?? String.Empty);
                Assert.IsNotNull(user);
                Assert.IsTrue(user.Guid == _user?.Guid);
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public async Task DeleteUser()
    {
        try
        {

            if (_userService != null)
            {
                var user = await _userService.GetUserByGuidAsync(_user.Guid ?? String.Empty);
                Assert.IsNotNull(user);
                var deleted = await _userService.DeleteUserAsync(user);
                Assert.IsTrue(deleted);
            }
            else
            {
                Assert.Fail($"UserService is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }


}



