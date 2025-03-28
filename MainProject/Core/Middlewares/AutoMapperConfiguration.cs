using BasicDotnetTemplate.MainProject.Models.Api.Common.User;
using SqlServerDatabase = BasicDotnetTemplate.MainProject.Models.Database.SqlServer;
using AutoMapper;
using BasicDotnetTemplate.MainProject.Models.Api.Common.Role;


namespace BasicDotnetTemplate.MainProject.Core.Middlewares;
public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<SqlServerDatabase.Role, RoleDto>();
        CreateMap<SqlServerDatabase.User, UserDto>();

    }
}
