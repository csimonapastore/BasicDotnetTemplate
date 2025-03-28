using System;
using System.Reflection;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicDotnetTemplate.MainProject;


namespace BasicDotnetTemplate.MainProject.Tests;

[TestClass]
public class Program_Tests
{
    [TestMethod]
    public void Program_Configuration_IsValid()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        try
        {
            var reflectionType = typeof(ReflectionProgram);

            if (reflectionType != null)
            {
                MethodInfo[] methods = reflectionType.GetMethods(); //Using BindingFlags.NonPublic does not show any results

                MethodInfo? execute = null;

                foreach (MethodInfo m in methods)
                {
                    if (m.Name == "LaunchConfiguration")
                    {
                        execute = m;
                    }
                }

                if (execute != null)
                {
                    object? initializeObj = execute != null ? execute.Invoke(null, Array.Empty<object>()) : throw new ArgumentNullException("LaunchConfiguration not found");
                    MethodInfo? initialize = initializeObj != null ? (MethodInfo)initializeObj : throw new ArgumentNullException("Unable to convert object because execute.Invoke is null");
                    if (initialize != null)
                    {
                        var success = false;
                        try
                        {
                            initialize.Invoke(null, new object[] { Array.Empty<string>() });
                            success = true;
                        }
                        catch (Exception innerException)
                        {
                            Assert.Fail($"An exception was thrown during initialize.Invoke: {innerException.Message}");
                        }
                        Assert.IsTrue(success);
                    }
                    else
                    {
                        Assert.Fail("Initialize is null.");
                    }
                }
                else
                {
                    Assert.Fail("Initialize method not found in Program class.");
                }
            }
            else
            {
                Assert.Fail("Program class not found in the assembly.");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail($"An exception was thrown: {ex}");
        }
    }

}
