using Blazor.SubtleCrypto;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace NamePairing;

public static class StartupExtensions
{
    public static IServiceCollection AddCryptoServices(this IServiceCollection services,
        Action<CryptoOptions>? setup = null) {
        if (setup != null) {
            services.Configure(setup);
        }


        services.AddScoped<ICryptoService, CryptoService>(
            (ctx) =>
            {
                       
                var options = ctx.GetRequiredService<IOptions<CryptoOptions>> ();
                var jsRuntime = ctx.GetService<IJSRuntime>();
                var key = options.Value.Key;

                return new CryptoService(jsRuntime, options.Value);
            }
        );
        return services;
    }
}