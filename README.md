# WebviewBind

A powerful C# source generator that simplifies bidirectional communication between JavaScript and C# in WebView2 applications. Automatically generates message dispatch infrastructure, enabling seamless JavaScript-to-C# method calls with zero boilerplate.

## Features

- 🚀 **Zero Boilerplate**: Source generator automatically handles WebView2 message routing
- 🔄 **Bidirectional Communication**: Call C# methods from JavaScript via `InvokeHostMethod()`
- ⚡ **Async Support**: Fully supports `Task` and `Task<T>` methods for non-blocking operations
- 📝 **Strong Typing**: Full type safety with automatic JSON serialization/deserialization
- 🎯 **Simple Attribute-Based API**: Just mark methods with `[WebviewExport]`
- 💻 **WPF Ready**: Complete working example with WPF + WebView2 integration
- 📦 **NETStandard 2.0 Compatible**: Works with modern .NET frameworks

## How It Works

### Architecture

1. **Attribute Decoration**: Mark C# methods with `[WebviewExport]` to expose them to JavaScript
2. **Source Generation**: The `WebviewExportGenerator` analyzes your code at compile-time
3. **Message Dispatch**: Generated code handles JSON serialization and async method invocation
4. **JavaScript Bridge**: Simple `InvokeHostMethod()` API for calling exported methods

### Method Types Supported

- **Synchronous methods**: Return values immediately
  ```csharp
  [WebviewExport]
  private string GetGreeting(string name) => $"Hello {name}!";
  ```

- **Async methods (Task<T>)**: Await-able with return values
  ```csharp
  [WebviewExport]
  private async Task<string> GetMessageAsync(string input)
  {
      await Task.Delay(2000);
      return $"Processed: {input}";
  }
  ```

- **Async methods (Task)**: Fire-and-forget with completion tracking
  ```csharp
  [WebviewExport]
  private async Task ProcessDataAsync(string data)
  {
      await Task.Delay(1000);
      // Do work...
  }
  ```

## Quick Start

### 1. Mark Your Container Class

```csharp
[WebviewBind.WebviewContainer]
public partial class MainWindow : Window
{
    // Your methods go here
}
```

### 2. Export Methods

```csharp
[WebviewBind.WebviewExport]
private string GetGreeting(string userName)
{
    return $"Hello {userName}!";
}

[WebviewBind.WebviewExport]
private async Task<string> FetchDataAsync(string id)
{
    // Async operation
    var data = await FetchFromDatabaseAsync(id);
    return data;
}
```

### 3. Register Handler in CodeBehind

```csharp
private async void OnLoaded(object sender, RoutedEventArgs args)
{
    await WebView.EnsureCoreWebView2Async();
    RegisterWebviewHandler(WebView.CoreWebView2);  // Register generated handler
    WebView.Source = new Uri("file:///path/to/index.html");
}
```

### 4. Call from JavaScript

```javascript
// Simple call
const greeting = await InvokeHostMethod('GetGreeting', 'Alice');
console.log(greeting);  // "Hello Alice!"

// Async call
const result = await InvokeHostMethod('FetchDataAsync', '123');
console.log(result);
```

## Project Structure

```
src/
├── WebviewBind/
│   ├── WebviewExportGenerator.cs       # Main source generator
│   └── WebviewBind.csproj
├── Examples/
│   └── WpfWithHtml/
│       ├── MainWindow.xaml.cs          # WPF container with exported methods
│       ├── WpfWithHtml.csproj
│       └── WebFiles/
│           ├── index.html              # Test page with method examples
│           ├── styles.css              # Styles
│           └── js/
│               └── webview-bridge.js   # JavaScript bridge
└── src-web/
    └── js/
        └── webview-bridge.js           # Shared JavaScript bridge
```

## WPF Example

The `WpfWithHtml` project demonstrates:

- **GetGreeting(string)**: Synchronous method returning a greeting
- **OpenLoadFileDialog()**: Synchronous method showing a file picker dialog  
- **GetMessageAsync(string)**: Asynchronous method with 2-second delay

The example HTML page includes:
- Input fields for testing each method
- Real-time status display (success/failure)
- Activity log showing all invocations

### Running the Example

1. Build the solution: `dotnet build`
2. Run the WPF app: `dotnet run --project src/Examples/WpfWithHtml`
3. Test the methods via the web interface

## JavaScript Bridge API

The `webview-bridge.js` exposes a single global function:

```javascript
window.InvokeHostMethod(methodName, ...args): Promise<any>
```

**Parameters:**
- `methodName` (string): Name of the C# method to call
- `...args` (any[]): Arguments to pass to the method

**Returns:**
- `Promise<any>`: Resolves with the return value, rejects on error

**Example:**
```javascript
try {
    const result = await InvokeHostMethod('GetGreeting', 'Bob');
    console.log('Success:', result);
} catch (error) {
    console.error('Failed:', error.message);
}
```

## Build System

The project uses MSBuild to automatically copy web assets:
- `src-web/js/**/*` → Output `WebFiles/js/`
- `WebFiles/` → Output `WebFiles/`

This ensures all static assets are available in the compiled application.

## License

See LICENSE file for details.
