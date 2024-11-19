using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

// [Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class User(
    IUserService userService,
    UserManager<ApplicationUser> userManager)
    : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserView>>> GetUsers()
    {
        return await userService.GetAllUsers();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserView>> GetUser(string id)
    {
        var user = await userService.GetUser(id);
        if (user == null) return NotFound();
        return user;
    }
    
    [HttpPost]
    public async Task<UserView> AddUser([FromBody] Register user)
    {
        return await userService.AddUser(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, Update model)
    {
        try
        {
            var result = await userService.UpdateUser(id, model);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IdentityResult> DeleteUser(string id)
    {
        return await userService.DeleteUser(id);
    }

    [HttpGet("roles")]
    public async Task<List<string>> GetRoles()
    {
        return await userService.GetRoles();
    }
    
    [HttpPost("CreateRole/{role}")]
    
    public async Task<ActionResult<IdentityResult>> CreateRole(string role)
    {
        return await userService.CreateRole(role);
    }

    [HttpPost("{userId}/roles/{role}")]
    public async Task<IdentityResult> AssignRole(string userId, string role)
    { 
        return await userService.AssignRole(userId, role);
    }

    [HttpDelete("{userId}/roles/{role}")]
    public async Task<IActionResult> RemoveRole(string userId, string role)
    {
        try
        {
            var result = await userService.RemoveRole(userId, role);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}