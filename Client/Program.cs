using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shared.Services;
using Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
string IP_ADDRESS = "localhost";
string SERVER_PORT = "7166";
string SERVER_URL = $"https://{IP_ADDRESS}:{SERVER_PORT}";
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient(
    "Auth",
    opt => opt.BaseAddress = new Uri(SERVER_URL));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton(sp =>
    new HttpClient { BaseAddress = new Uri(SERVER_URL) });

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();