# WebviewBind (NuGet Package)

`WebviewBind` is a C# source generator package that creates WebView2 host-side dispatch code for methods marked with `[WebviewExport]`.

## Install

```bash
dotnet add package WebviewBind
```

## What It Generates

For each partial class marked with `[WebviewContainer]`, the generator emits:

- `RegisterWebviewHandler(CoreWebView2 coreWebView2)` to attach message handling.
- JSON call deserialization and method dispatch.
- Support for sync return values, `Task`, and `Task<T>` methods.
- Error propagation back to JavaScript response payload.

## Quick Start

### 1. Mark host container class

```csharp
[WebviewBind.WebviewContainer]
public partial class MainWindow : Window
{
}
```

### 2. Export host methods

```csharp
[WebviewBind.WebviewExport]
private string GetGreeting(string userName)
{
    return $"Hello {userName}!";
}

[WebviewBind.WebviewExport]
private async Task<string> GetMessageAsync(string arg)
{
    await Task.Delay(1200);
    return $"{arg}, async reply from host.";
}
```

### 3. Register handler after CoreWebView2 is ready

```csharp
await WebView.EnsureCoreWebView2Async();
RegisterWebviewHandler(WebView.CoreWebView2);
```

### 4. Call from JavaScript

```javascript
const greeting = await InvokeHostMethod('GetGreeting', 'Alice');
const asyncResult = await InvokeHostMethod('GetMessageAsync', 'WebView');
```

## JavaScript Call Contract

Request payload:

```json
{
  "callId": "string",
  "method": "GetGreeting",
  "args": ["Alice"]
}
```

Response payload:

```json
{
  "callId": "string",
  "result": "Hello Alice!",
  "error": null
}
```

## Supported Method Shapes

- `void Method(...)`
- `T Method(...)`
- `Task Method(...)`
- `Task<T> Method(...)`

## Notes

- Export methods should be instance methods on a class marked with `[WebviewContainer]`.
- Parameters and return values must be JSON-serializable.
- Unknown method names return an error response.

## Example Projects

- `src/Examples/WpfWithHtml`
- `src/Examples/WpfWithBlazor` (host side)

## Repository

- Root: `README.md`
- Blazor bridge package doc: `docs/WebviewBind.Blazor.Package.md`
