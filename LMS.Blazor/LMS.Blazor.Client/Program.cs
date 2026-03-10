using LMS.Blazor.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace LMS.Blazor.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        });

        builder.Services.AddScoped<IApiService, ClientApiService>();

        await builder.Build().RunAsync();
    }
}
