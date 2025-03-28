using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BasicDotnetTemplate.MainProject.Core.Attributes;
using BasicDotnetTemplate.MainProject.Models.Api.Request.Auth;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using BasicDotnetTemplate.MainProject.Models.Api.Response.Auth;
using BasicDotnetTemplate.MainProject.Services;

namespace BasicDotnetTemplate.MainProject.Controllers
{
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        public AuthController(
            IConfiguration configuration,
            IAuthService authService
        ) : base(configuration)
        {
            this._authService = authService;
        }

        [HttpPost("authenticate")]
        [ProducesResponseType<AuthenticateResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(_requestNotWellFormed);
                }

                if (
                    request == null ||
                    request.Data == null ||
                    String.IsNullOrEmpty(request.Data.Email) ||
                    String.IsNullOrEmpty(request.Data.Password)
                )
                {
                    return BadRequest(_requestNotWellFormed);
                }
                var data = await this._authService.AuthenticateAsync(request.Data);

                if (data == null)
                {
                    return NotFound();
                }

                return Success(String.Empty, data);
            }
            catch (Exception exception)
            {
                var message = this._somethingWentWrong;
                if (!String.IsNullOrEmpty(exception.Message))
                {
                    message += $". {exception.Message}";
                }
                return InternalServerError(message);
            }

        }
    }
}