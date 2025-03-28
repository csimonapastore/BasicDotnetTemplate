using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasicDotnetTemplate.MainProject.Core.Attributes;
using BasicDotnetTemplate.MainProject.Services;
using BasicDotnetTemplate.MainProject.Models.Api.Request.Role;
using BasicDotnetTemplate.MainProject.Models.Api.Response;
using BasicDotnetTemplate.MainProject.Models.Api.Response.Role;
using BasicDotnetTemplate.MainProject.Models.Database.SqlServer;
using BasicDotnetTemplate.MainProject.Models.Api.Common.Role;

namespace BasicDotnetTemplate.MainProject.Controllers
{
    [Route("[controller]")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        public RoleController(
            IConfiguration configuration,
            IRoleService roleService
        ) : base(configuration)
        {
            this._roleService = roleService;
        }

        [JwtAuthorization()]
        [HttpGet("get/{guid}")]
        [ProducesResponseType<GetRoleResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoleByGuidAsync(string guid)
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
                var role = await this._roleService.GetRoleByGuidAsync(guid);

                if (role == null || String.IsNullOrEmpty(role.Guid))
                {
                    return NotFound();
                }

                var roleDto = _mapper?.Map<RoleDto>(role);

                return Success(String.Empty, roleDto);
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
        [ProducesResponseType<GetRoleResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(_requestNotWellFormed);
                }

                if (request == null || request.Data == null || String.IsNullOrEmpty(request.Data.Name)
                )
                {
                    return BadRequest(_requestNotWellFormed);
                }

                if (await this._roleService.CheckIfNameIsValid(request.Data.Name))
                {
                    var role = await this._roleService.CreateRoleAsync(request.Data);

                    if (role == null || String.IsNullOrEmpty(role.Guid))
                    {
                        return BadRequest("Not created");
                    }

                    var roleDto = _mapper?.Map<RoleDto>(role);

                    return Success(String.Empty, roleDto);
                }
                else
                {
                    return BadRequest("Invalid name");
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

        [JwtAuthorization()]
        [HttpPut("update/{guid}")]
        [ProducesResponseType<GetRoleResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] CreateRoleRequest request, string guid)
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
                    String.IsNullOrEmpty(request.Data.Name) ||
                    String.IsNullOrEmpty(guid)
                )
                {
                    return BadRequest(_requestNotWellFormed);
                }

                var role = await this._roleService.GetRoleByGuidAsync(guid);

                if (role == null || String.IsNullOrEmpty(role.Guid))
                {
                    return NotFound();
                }

                if (role.IsNotEditable)
                {
                    return BadRequest("This role is not editable");
                }

                if (
                    await this._roleService.CheckIfNameIsValid(request.Data.Name) ||
                    await this._roleService.CheckIfNameIsValid(request.Data.Name, guid)
                )
                {
                    role = await this._roleService.UpdateRoleAsync(request.Data, role);

                    var roleDto = _mapper?.Map<RoleDto>(role);

                    return Success(String.Empty, roleDto);
                }
                else
                {
                    return BadRequest("Invalid name");
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

        [JwtAuthorization()]
        [HttpDelete("{guid}")]
        [ProducesResponseType<GetRoleResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<BaseResponse<object>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRoleByGuidAsync(string guid)
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
                var role = await this._roleService.GetRoleByGuidAsync(guid);

                if (role == null || String.IsNullOrEmpty(role.Guid))
                {
                    return NotFound();
                }

                await this._roleService.DeleteRoleAsync(role);

                return Success(String.Empty);
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