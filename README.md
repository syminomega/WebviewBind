# WebviewBind

WebviewBind is a WebView2 communication toolkit split into two NuGet packages:

- `WebviewBind`: C# source generator package for host-side message dispatch.
- `WebviewBind.Blazor`: Blazor runtime bridge package for calling host methods through DI.

This repository README is the entry point. Package-specific documentation is maintained separately.

## Package Docs

- [WebviewBind package guide](docs/WebviewBind.Package.md)
- [WebviewBind.Blazor package guide](docs/WebviewBind.Blazor.Package.md)

## Which Package To Use

Use `WebviewBind` when:
- You want attribute-driven source generation for WebView2 host method dispatch.
- You are exposing C# methods to JavaScript with `[WebviewExport]`.

Use `WebviewBind.Blazor` when:
- You run Blazor inside WebView2 and want DI-based host invocation.
- You need `IWebviewHostBridge` and the packaged `_content/WebviewBind.Blazor/webviewHostBridge.js` module.

Use both when:
- A WPF/WebView2 host exports methods and an embedded Blazor app calls them.

## Repository Examples

- `src/Examples/WpfWithHtml`: Static HTML + JavaScript bridge calls into WPF host.
- `src/Examples/WpfWithBlazor`: WPF host + Blazor WebAssembly app loaded in WebView2.
- `src/Examples/BlazorTest`: Minimal Blazor test UI used by `WpfWithBlazor`.

## License

See [LICENSE](LICENSE).
