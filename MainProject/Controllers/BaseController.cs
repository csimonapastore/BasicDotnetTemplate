using System.Net;
using Microsoft.AspNetCore.Mvc;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using BasicDotnetTemplate.MainProject.Models.Settings;
using AutoMapper;
using BasicDotnetTemplate.MainProject.Core.Middlewares;

namespace BasicDotnetTemplate.MainProject.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMapper? _mapper;
        protected readonly IConfiguration _configuration;
        protected readonly AppSettings _appSettings;
        protected readonly string _requestNotWellFormed = "Request is not well formed";
        protected readonly string _somethingWentWrong = "Something went wrong";

        protected BaseController(
            IConfiguration configuration
        )
        {
            _configuration = configuration;
            _appSettings = new AppSettings();
            _configuration.GetSection("AppSettings").Bind(_appSettings);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfiguration>();
            });

            _mapper = config.CreateMapper();
        }

#nullable enable
        private static BaseResponse<T> CreateResponse<T>(HttpStatusCode status, string message, T? data)
        {
            return new BaseResponse<T>((int)status, message, data);
        }

        protected new IActionResult Created(string message, object? data = null)
        {
            message = String.IsNullOrEmpty(message) ? "Created" : message;
            return StatusCode((int)HttpStatusCode.Created, CreateResponse(HttpStatusCode.Created, message, data));
        }

        protected IActionResult Success(string message, object? data = null)
        {
            message = String.IsNullOrEmpty(message) ? "Success" : message;
            return StatusCode((int)HttpStatusCode.OK, CreateResponse(HttpStatusCode.OK, message, data));
        }

        protected IActionResult NotModified(string message, object? data = null)
        {
            message = String.IsNullOrEmpty(message) ? "Not modified" : message;
            return StatusCode((int)HttpStatusCode.NotModified, CreateResponse(HttpStatusCode.NotModified, message, data));
        }

        protected IActionResult NotFound(string message, object? data = null)
        {
            message = String.IsNullOrEmpty(message) ? "Not found" : message;
            return StatusCode((int)HttpStatusCode.NotFound, CreateResponse(HttpStatusCode.NotFound, message, data));
        }

        protected IActionResult BadRequest(string message, object? data = null)
        {
            message = String.IsNullOrEmpty(message) ? "Bad request" : message;
            return StatusCode((int)HttpStatusCode.BadRequest, CreateResponse(HttpStatusCode.BadRequest, message, data));
        }

        protected IActionResult InternalServerError(string message)
        {
            message = String.IsNullOrEmpty(message) ? "Internal server error" : message;
            return StatusCode((int)HttpStatusCode.InternalServerError, CreateResponse(HttpStatusCode.InternalServerError, message, new object()));
        }

#nullable disable
    }
}