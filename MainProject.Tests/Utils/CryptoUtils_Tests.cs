using System;
using BasicDotnetTemplate.MainProject.Models.Settings;
using Microsoft.AspNetCore.Builder;
using BasicDotnetTemplate.MainProject.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace BasicDotnetTemplate.MainProject.Tests;

[TestClass]
public class CryptoUtils_Tests
{
    [TestMethod]
    public void Decrypt_Success()
    {
        try
        {
            string encryptedData = "d2ejdI1f4GYpq2kTB1nmeQkZXqR3QSxH8Yqkl7iv7zgfQ13qG/0dUUsreG/WGHWRBE5mVWaV43A=";
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            CryptUtils cryptoUtils = new CryptUtils(appSettings);
            var decryptedData = cryptoUtils.Decrypt(encryptedData);
            var isEqual = decryptedData == "ThisIsASuccessfullTest";
            Assert.IsTrue(isEqual);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void Decrypt_Error()
    {
        try
        {
            string encryptedData = "d1ejdI1f4GYpq2kTB1nmeQkZXqR3QSxH8Yqkl7iv7zgfQ13qG/0dUUsreG/WGHWRBE5mVWaV43A=";
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            CryptUtils cryptoUtils = new CryptUtils(appSettings);
            var decryptedData = cryptoUtils.Decrypt(encryptedData);
            var isEqual = decryptedData == "ThisIsASuccessfullTest";
            Assert.IsFalse(isEqual);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void Decrypt_ArgumentException()
    {
        try
        {
            string encryptedData = "d1ejdI1f4GYpq2kTB1nmeQkZXqR3QSxH8Yqkl7iv7zgfQ13qG/0dUUsreG/WGHWRBE5mVWaV43A=";
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData", "invalidCryptAppsettings.json");
            CryptUtils cryptoUtils = new CryptUtils(appSettings);
            try
            {
                var decryptedData = cryptoUtils.Decrypt(encryptedData);
                Assert.Fail($"Expected exception instead of response: {decryptedData}");
            }
            catch (ArgumentException argumentException)
            {
                Assert.IsInstanceOfType(argumentException, typeof(ArgumentException));
            }
            catch (Exception exception)
            {
                Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void Decrypt_Empty()
    {
        try
        {
            string encryptedData = "WGHWRBE5mVWaV=";
            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            CryptUtils cryptoUtils = new CryptUtils(appSettings);
            var decryptedData = cryptoUtils.Decrypt(encryptedData);
            var isEqual = decryptedData == String.Empty;
            Assert.IsTrue(isEqual);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void GenerateSalt()
    {
        try
        {
            var salt = CryptUtils.GenerateSalt();
            Assert.IsTrue(!String.IsNullOrEmpty(salt));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void ComputeHash_Hashed()
    {
        try
        {
            var password = "P4ssw0rd@1!";
            var salt = CryptUtils.GenerateSalt();
            Assert.IsTrue(!String.IsNullOrEmpty(salt));

            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            CryptUtils cryptoUtils = new CryptUtils(appSettings);
            var encryptedPassword = cryptoUtils.GeneratePassword(password, salt, 0);
            Assert.IsTrue(password != encryptedPassword);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

    [TestMethod]
    public void VerifyPassword_True()
    {
        try
        {
            var password = "P4ssw0rd@1!";
            var salt = "Afi7PQYgEL2sPbNyVzduvg==";
            var hashedPassword = "2lMeySZ9ciH1KtSg1Z7oSJRmJEjHMeDvdaNRcJcGutM=";

            WebApplicationBuilder builder = WebApplication.CreateBuilder(Array.Empty<string>());
            AppSettings appSettings = ProgramUtils.AddConfiguration(ref builder, System.AppDomain.CurrentDomain.BaseDirectory + "/JsonData");
            CryptUtils cryptoUtils = new CryptUtils(appSettings);
            var verified = cryptoUtils.VerifyPassword(password, salt, 0, hashedPassword);
            Assert.IsTrue(verified);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

}




