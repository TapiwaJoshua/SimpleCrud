using Microsoft.AspNetCore.Identity;
using Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Services;

public class UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    : IUserService
{
    public async Task<UserView> GetUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return new UserView
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = new List<string>(roles)
        };
    }

    public async Task<List<UserView>> GetAllUsers()
    {
        var users = new List<ApplicationUser>(userManager.Users);
        var userViewModels = new List<UserView>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userViewModels.Add(new UserView
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = new List<string>(roles)
            });
        }

        return userViewModels;
    }

    public async Task<IdentityResult> UpdateUser(string userId, Update model)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.UserName = model.Email;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded) return result;
        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = new List<string>();
        var rolesToAdd = new List<string>();

        foreach (var role in currentRoles)
        {
            if (!model.Roles.Contains(role))
            {
                rolesToRemove.Add(role);
            }
        }

        foreach (var role in model.Roles)
        {
            if (!currentRoles.Contains(role))
            {
                rolesToAdd.Add(role);
            }
        }

        if (rolesToRemove.Count > 0)
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        
        if (rolesToAdd.Count > 0)
            await userManager.AddToRolesAsync(user, rolesToAdd);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        return await userManager.DeleteAsync(user);
    }

    public async Task<List<string>> GetRoles()
    {
        var roles = new List<string>();
        foreach (var role in roleManager.Roles)
        {
            roles.Add(role.Name);
        }
        return roles;
    }

    public async Task<IdentityResult> AssignRole(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

        return await userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveRole(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        return await userManager.RemoveFromRoleAsync(user, role);
    }
}