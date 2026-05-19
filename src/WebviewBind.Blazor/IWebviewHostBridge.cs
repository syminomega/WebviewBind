using System.Threading;
using System.Threading.Tasks;

namespace WebviewBind.Blazor;

public interface IWebviewHostBridge
{
    ValueTask<bool> IsAvailableAsync(CancellationToken cancellationToken = default);

    ValueTask<TValue?> InvokeAsync<TValue>(string methodName, params object?[]? args);

    ValueTask<TValue?> InvokeAsync<TValue>(
        string methodName,
        CancellationToken cancellationToken,
        params object?[]? args);

    ValueTask InvokeVoidAsync(string methodName, params object?[]? args);

    ValueTask InvokeVoidAsync(
        string methodName,
        CancellationToken cancellationToken,
        params object?[]? args);
}