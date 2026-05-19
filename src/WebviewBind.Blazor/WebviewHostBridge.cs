using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace WebviewBind.Blazor;

public sealed class WebviewHostBridge : IWebviewHostBridge, IAsyncDisposable
{
    private const string ModulePath = "./_content/WebviewBind.Blazor/webviewHostBridge.js";

    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public WebviewHostBridge(IJSRuntime jsRuntime)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath).AsTask());
    }

    public async ValueTask<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        var module = await _moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<bool>("isAvailable", cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<TValue?> InvokeAsync<TValue>(string methodName, params object?[]? args)
    {
        return InvokeAsync<TValue>(methodName, CancellationToken.None, args);
    }

    public async ValueTask<TValue?> InvokeAsync<TValue>(
        string methodName,
        CancellationToken cancellationToken,
        params object?[]? args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        var module = await _moduleTask.Value.ConfigureAwait(false);
        return await module.InvokeAsync<TValue?>(
                "invokeHostMethod",
                cancellationToken,
                methodName,
                args ?? Array.Empty<object?>())
            .ConfigureAwait(false);
    }

    public ValueTask InvokeVoidAsync(string methodName, params object?[]? args)
    {
        return InvokeVoidAsync(methodName, CancellationToken.None, args);
    }

    public async ValueTask InvokeVoidAsync(
        string methodName,
        CancellationToken cancellationToken,
        params object?[]? args)
    {
        await InvokeAsync<object?>(methodName, cancellationToken, args).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_moduleTask.IsValueCreated)
        {
            return;
        }

        var module = await _moduleTask.Value.ConfigureAwait(false);
        await module.DisposeAsync().ConfigureAwait(false);
    }
}