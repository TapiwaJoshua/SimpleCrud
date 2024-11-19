using Microsoft.AspNetCore.Identity;
using Server.Controllers;
using Server.Models;

namespace Server.Services;

public interface IUserService
{
    Task<UserView> GetUser(string userId);
    Task<List<UserView>> GetAllUsers();
    Task<IdentityResult> UpdateUser(string userId, Update model);
    Task<IdentityResult> DeleteUser(string userId);
    Task<List<string>> GetRoles();
    Task<IdentityResult> AssignRole(string userId, string role);
    Task<IdentityResult> RemoveRole(string userId, string role);
    Task<IdentityResult> CreateRole(string role);
    Task <UserView> AddUser(Register user);
}