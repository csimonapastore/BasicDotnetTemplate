using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasicDotnetTemplate.MainProject.Core.Attributes;
using BasicDotnetTemplate.MainProject.Services;
using BasicDotnetTemplate.MainProject.Models.Api.Request.User;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using BasicDotnetTemplate.MainProject.Models.Api.Response.User;
using BasicDotnetTemplate.MainProject.Models.Database.SqlServer;
using BasicDotnetTemplate.MainProject.Models.Api.Common.User;

namespace BasicDotnetTemplate.MainProject.Controllers
{
    [Route("[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public UserController(
            IConfiguration configuration,
            IUserService userService,
            IRoleService roleService
        ) : base(configuration)
        {
            this._userService = userService;
            this._roleService = roleService;
        }

        [JwtAuthorization()]
        [HttpGet("get/{guid}")]
        [ProducesResponseType<GetUserResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByGuidAsync(string guid)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(_requestNotWellFormed);
                }

                if (String.IsNullOrEmpty(guid))
                {
                    return BadRequest(_requestNotWellFormed);
                }
                var user = await this._userService.GetUserByGuidAsync(guid);

                if (user == null || String.IsNullOrEmpty(user.Guid))
                {
                    return NotFound();
                }

                var userDto = _mapper?.Map<UserDto>(user);

                return Success(String.Empty, userDto);
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

        [JwtAuthorization()]
        [HttpPost("create")]
        [ProducesResponseType<GetUserResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(_requestNotWellFormed);
                }

                if (request == null || request.Data == null ||
                    String.IsNullOrEmpty(request.Data.FirstName) ||
                    String.IsNullOrEmpty(request.Data.LastName) ||
                    String.IsNullOrEmpty(request.Data.Email) ||
                    String.IsNullOrEmpty(request.Data.Password)
                )
                {
                    return BadRequest(_requestNotWellFormed);
                }

                if (await this._userService.CheckIfEmailIsValid(request.Data.Email))
                {
                    var role = await this._roleService.GetRoleForUser(request.Data.RoleGuid);
                    if (role == null)
                    {
                        return BadRequest("Role not found");
                    }

                    var user = await this._userService.CreateUserAsync(request.Data, role);

                    if (user == null || String.IsNullOrEmpty(user.Guid))
                    {
                        return BadRequest("Not created");
                    }

                    var userDto = _mapper?.Map<UserDto>(user);

                    return Success(String.Empty, userDto);
                }
                else
                {
                    return BadRequest("Invalid email");
                }

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