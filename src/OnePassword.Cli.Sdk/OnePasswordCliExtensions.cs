using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace OnePassword.Cli.Sdk;

public static class OnePasswordCliExtensions
{
    public static IServiceCollection TryAdd1PasswordCliServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IOnePasswordClient, OnePasswordCliClient>();
        return services;
    }
}
