using MySqlX.XDevAPI.Common;
using Server.Models;
namespace Shared.Services;

public interface IUserService
{
    Task<List<UserView>> GetUsers();
    
    Task<UserView> AddUser(Register user);
    Task<bool> UpdateUser(string userId, Update user);
    Task<bool> DeleteUser(string userId);
    Task<BaseResult> Login(LoginModel login);

    Task<List<string>> GetRoles();
    Task<bool> AssignRole(string userID, string role);
    Task<BaseResult> DeleteRole();
    Task<BaseResult> UpdateRole();
    Task<string> AddRole(string role);
}