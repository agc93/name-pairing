using Blazor.SubtleCrypto;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NamePairing;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddSubtleCrypto(o =>
{
    o.Key = Guid.NewGuid().ToString("N");
});
builder.Services.AddScoped<LinkService>();
builder.Services.AddTransient<IClipboardService, ClipboardService>();
builder.Services.AddScoped<IEncryptService, InteropEncryptService>();
builder.Services.AddScoped<IMatchService, DifferenceMatchService>();
builder.Services.AddScoped<IBrandProvider>(p =>
{
    var config = p.GetService<IConfiguration>();
    if (config != null && config.GetSection("Customization").Exists()) {
        return new ConfigBrandProvider(config.GetSection("Customization"));
    }

    return new DefaultBrandProvider();
});

await builder.Build().RunAsync();
