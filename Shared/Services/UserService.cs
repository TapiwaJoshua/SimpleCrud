global using System.Net;
global using System.Net.Http.Json;
global using System.Security.Claims;
global using Shared.Services;
using Microsoft.AspNetCore.Components;
using MySqlX.XDevAPI.Common;
using Server.Models;

namespace Shared.Services;


public class UserService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager) //, ZambeziDigital.Base.Services.Contracts.
    : IUserService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");
    public List<UserView> Users { get; set; } = new();
    
    public async Task<List<UserView>> GetUsers()
    {
        if (Users is not null && Users.Count > 0) return Users;
        var request = await _httpClient.GetAsync($"api/User");
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        var objects = await request.Content.ReadFromJsonAsync<List<UserView>>();
        Users = objects.Select(user => new UserView
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.Roles
        }).ToList();
        
        return Users;
    }

    public async Task<UserView> AddUser(Register user)
    {
        if (Users.Any(u => u.Email == user.Email))
            return new UserView()
            {
                Id = "0",
                Email = "Email already exists",
                FirstName = "Email already exists",
                LastName = "Email already exists",
                Roles = new List<string>()
            };
        var request = await _httpClient.PostAsJsonAsync($"/api/User", user);
        var result = await request.Content.ReadFromJsonAsync<UserView>();
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        return new UserView()
        {
            Id = result.Id,
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Roles = new List<string>()
        };
    }

    public async Task<bool> UpdateUser(string userId, Update user)
    {
        if (Users.All(u => u.Id != userId))
            return false;
        var request = await _httpClient.PutAsJsonAsync($"/api/User/{userId}", user);
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        var result = await request.Content.ReadFromJsonAsync<BaseResult>();
        return true;
    }

    public async Task<bool> DeleteUser(string userId)
    {
        if(Users.All(u => u.Id != userId))
            return false;
        var request = await _httpClient.DeleteAsync($"/api/User/{userId}");
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        return true;
    }

    public async Task<BaseResult> Login(LoginModel login)
    {
        // var request = await _httpClient.PostAsJsonAsync($"/Login?useCookies=true&useSessionCookies=true", login);
        var request = await _httpClient.PostAsJsonAsync($"/Login", login);
        if (request.StatusCode == HttpStatusCode.BadRequest)
            return await request.Content.ReadFromJsonAsync<BaseResult>() ?? throw new Exception("Error processing login request response");
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        var result = await request.Content.ReadFromJsonAsync<BaseResult>();
        return result;
    }

    public async Task<List<string>> GetRoles()
    {
        var request = await _httpClient.GetAsync($"/api/User/roles");
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        var result = request.Content.ReadFromJsonAsync<List<string>>();
        return await result;
    }
    
    public async Task<bool> AssignRole(string userId, string role)
    {
        var request = await _httpClient.PostAsJsonAsync<object>($"/api/User/{userId}/roles/{role}", null);
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        return true;
    }

    public Task<BaseResult> DeleteRole()
    {
        throw new NotImplementedException();
    }

    public Task<BaseResult> UpdateRole()
    {
        throw new NotImplementedException();
    }

    public async Task<string> AddRole(string role)
    {
        var request = await _httpClient.PostAsJsonAsync($"/api/User/CreateRole/{role}", role);
        if (!request.IsSuccessStatusCode) throw new Exception(request.ReasonPhrase);
        return role;
    }
}