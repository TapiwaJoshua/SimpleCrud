using Microsoft.AspNetCore.Identity;
using Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Server.Data;

namespace Server.Services;

public class UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context )
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
    
    public async Task<UserView> AddUser(Register user)
    {
        ApplicationUser userToCreate = new ApplicationUser
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.Email,
            PasswordHash = user.Password,
        };
        await userManager.CreateAsync(userToCreate);
        var users = await GetAllUsers();
        var UserCreated = users.FirstOrDefault(x => x.Email == user.Email);

        return new UserView()
        {
            Id = UserCreated.Id,
            Email = UserCreated.Email,
            FirstName = UserCreated.FirstName,
            LastName = UserCreated.LastName,
            Roles = new List<string>()
        };
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

        // Ensure the user is not already being tracked
        var trackedUser = context.Users.Local.FirstOrDefault(u => u.Id == userId);
        if (trackedUser != null)
        {
            context.Entry(trackedUser).State = EntityState.Detached;
        }

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
        await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
        return await userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveRole(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        return await userManager.RemoveFromRoleAsync(user, role);
    }

    public Task<IdentityResult> CreateRole(string role)
    {
        var result = roleManager.CreateAsync(new IdentityRole(role));
        return result;
    }
    public async Task<bool> Login(LoginModel login)
    {
        var user = await userManager.FindByEmailAsync(login.Email);
        var result = await userManager.CheckPasswordAsync(user, login.Password);
        return result;
    }
}