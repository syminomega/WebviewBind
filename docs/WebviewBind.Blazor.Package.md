# WebviewBind.Blazor (NuGet Package)

`WebviewBind.Blazor` is a Razor class library that provides a DI-friendly bridge service for invoking WebView2 host methods from Blazor.

## Install

```bash
dotnet add package WebviewBind.Blazor
```

## Register Service

```csharp
using WebviewBind.Blazor;

builder.Services.AddWebviewHostBridge();
```

## Use In Razor

```razor
@inject IWebviewHostBridge HostBridge

<button @onclick="CallHost">Call Host</button>

@code {
    private async Task CallHost()
    {
        var greeting = await HostBridge.InvokeAsync<string>("GetGreeting", "Blazor");
    }
}
```

## API Surface

```csharp
ValueTask<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
ValueTask<T?> InvokeAsync<T>(string methodName, params object?[]? args)
ValueTask<T?> InvokeAsync<T>(string methodName, CancellationToken cancellationToken, params object?[]? args)
ValueTask InvokeVoidAsync(string methodName, params object?[]? args)
ValueTask InvokeVoidAsync(string methodName, CancellationToken cancellationToken, params object?[]? args)
```

## Runtime Notes (WebView2)

For Blazor WebAssembly hosted in WebView2, do not load app entry page with `file://`.
Use virtual host mapping and navigate to a local HTTPS-style URL.

```csharp
await WebView.EnsureCoreWebView2Async();
WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
    "app.local",
    appFolderPath,
    CoreWebView2HostResourceAccessKind.Allow);
WebView.Source = new Uri("https://app.local/index.html");
```

This avoids CORS failures when the runtime fetches `_framework/*` and `_content/*` assets.

## Packaged Static Asset

This package includes:

- `_content/WebviewBind.Blazor/webviewHostBridge.js`

The DI service imports that module automatically.

## Example Projects

- `src/Examples/BlazorTest`
- `src/Examples/WpfWithBlazor`

## Repository

- Root: `README.md`
- Source generator package doc: `docs/WebviewBind.Package.md`
