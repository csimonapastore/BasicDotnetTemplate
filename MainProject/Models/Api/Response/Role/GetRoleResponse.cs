using BasicDotnetTemplate.MainProject.Models.Api.Common.Role;

namespace BasicDotnetTemplate.MainProject.Models.Api.Response.Role;

public class GetRoleResponse : BaseResponse<RoleDto>
{
    public GetRoleResponse(int status, string? message, RoleDto? data) : base(status, message, data) { }
}