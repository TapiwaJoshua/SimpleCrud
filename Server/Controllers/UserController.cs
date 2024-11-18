using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var result = await userService.DeleteUser(id);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetRoles()
    {
        return await userService.GetRoles();
    }

    [HttpPost("{userId}/roles/{role}")]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        try
        {
            var result = await userService.AssignRole(userId, role);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
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