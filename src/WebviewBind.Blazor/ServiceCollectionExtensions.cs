using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebviewBind.Blazor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebviewHostBridge(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<IWebviewHostBridge, WebviewHostBridge>();
        return services;
    }
}