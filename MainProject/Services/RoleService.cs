
using System.Collections;
using BasicDotnetTemplate.MainProject.Core.Database;
using BasicDotnetTemplate.MainProject.Models.Api.Common.Exceptions;
using BasicDotnetTemplate.MainProject.Models.Api.Data.Role;
using BasicDotnetTemplate.MainProject.Models.Database.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace BasicDotnetTemplate.MainProject.Services;

public interface IRoleService
{
    Task<Role?> GetRoleByIdAsync(int id);
    Task<Role?> GetRoleByGuidAsync(string guid);
    Task<bool> CheckIfNameIsValid(string name, string? guid = "");
    Task<Role?> CreateRoleAsync(CreateRoleRequestData data);
    Task<Role?> UpdateRoleAsync(CreateRoleRequestData data, Role role);
    Task<Role?> GetRoleForUser(string? guid);
    Task<bool?> DeleteRoleAsync(Role role);
}

public class RoleService : BaseService, IRoleService
{
    private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public RoleService(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        SqlServerContext sqlServerContext
    ) : base(httpContextAccessor, configuration, sqlServerContext)
    { }

    private IQueryable<Role> GetRolesQueryable()
    {
        return this._sqlServerContext.Roles.Where(x => !x.IsDeleted);
    }
    private IQueryable<Role> GetRoleByNameQueryable(string name)
    {
        return this.GetRolesQueryable().Where(x =>
            x.Name.ToString() == name.ToString()
        );
    }



    private Role CreateRoleData(CreateRoleRequestData data)
    {
        Role role = new()
        {
            CreationTime = DateTime.UtcNow,
            CreationUserId = this.GetCurrentUserId(),
            IsDeleted = false,
            Guid = Guid.NewGuid().ToString(),
            Name = data.Name,
            IsNotEditable = data.IsNotEditable
        };

        return role;
    }





    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        return await this.GetRolesQueryable().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Role?> GetRoleByGuidAsync(string guid)
    {
        return await this.GetRolesQueryable().Where(x => x.Guid == guid).FirstOrDefaultAsync();
    }

    public async Task<bool> CheckIfNameIsValid(string name, string? guid = "")
    {
        var valid = false;

        Role? role = await this.GetRoleByNameQueryable(name).FirstOrDefaultAsync();
        if (role != null)
        {
            if (!String.IsNullOrEmpty(guid))
            {
                valid = role.Guid == guid && role.Name == name;
            }
        }
        else
        {
            valid = true;
        }

        return valid;
    }

    public async Task<Role?> CreateRoleAsync(CreateRoleRequestData data)
    {
        Role? role = null;

        using var transaction = await _sqlServerContext.Database.BeginTransactionAsync();

        try
        {
            var tempRole = this.CreateRoleData(data);
            await _sqlServerContext.Roles.AddAsync(tempRole);
            await _sqlServerContext.SaveChangesAsync();
            await transaction.CommitAsync();
            role = tempRole;
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            Logger.Error(exception, $"[RoleService][CreateRoleAsync]");
            throw new CreateException($"An error occurred while saving the role for transaction ID {transaction.TransactionId}.", exception);
        }

        return role;
    }

    public async Task<Role?> UpdateRoleAsync(CreateRoleRequestData data, Role role)
    {
        if (role.IsNotEditable)
            return role;

        using var transaction = await _sqlServerContext.Database.BeginTransactionAsync();

        try
        {
            role.Name = data.Name;
            role.IsNotEditable = data.IsNotEditable;
            role.UpdateTime = DateTime.UtcNow;
            role.UpdateUserId = this.GetCurrentUserId();

            _sqlServerContext.Roles.Update(role);
            await _sqlServerContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            Logger.Error(exception, $"[RoleService][UpdateRoleAsync] | {transaction.TransactionId}");
            await transaction.RollbackAsync();
            throw new UpdateException($"An error occurred while updating the role for transaction ID {transaction.TransactionId}.", exception);
        }

        return role;
    }

    public async Task<Role?> GetRoleForUser(string? guid)
    {
        Role? role = null;

        if (String.IsNullOrEmpty(guid))
        {
            role = await this.GetRoleByNameQueryable("Default").FirstOrDefaultAsync();
        }
        else
        {
            role = await this.GetRoleByGuidAsync(guid);
        }

        return role;
    }

    public async Task<bool?> DeleteRoleAsync(Role role)
    {
        bool? deleted = false;

        using (var transaction = _sqlServerContext.Database.BeginTransactionAsync())
        {
            role.IsDeleted = true;
            role.DeletionTime = DateTime.UtcNow;
            _sqlServerContext.Update(role);
            await _sqlServerContext.SaveChangesAsync();
            await (await transaction).CommitAsync();
            deleted = true;
        }

        return deleted;
    }


}

