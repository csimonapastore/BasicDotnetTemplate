using System;

namespace BasicDotnetTemplate.MainProject.Models.Api.Common.Exceptions;

public class UpdateException : Exception
{
    public UpdateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
