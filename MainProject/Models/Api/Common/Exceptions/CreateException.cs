using System;

namespace BasicDotnetTemplate.MainProject.Models.Api.Common.Exceptions;

public class CreateException : Exception
{
    public CreateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
