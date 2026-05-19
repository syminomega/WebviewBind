using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;

namespace WpfWithBlazor;

[WebviewBind.WebviewContainer]
public partial class MainWindow : Window
{
    // Serve the published Blazor app from a virtual HTTPS origin instead of file://.
    // Blazor WebAssembly loads _framework and _content assets with fetch/script requests,
    // and those requests are blocked by WebView2 when the page origin is null (file scheme).
    private const string BlazorAppFolderName = "BlazorApp";
    private const string BlazorAppHostName = "app.local";

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs args)
    {
        try
        {
            var appFolderPath = Path.Combine(AppContext.BaseDirectory, BlazorAppFolderName);
            var htmlPath = Path.Combine(appFolderPath, "index.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show($"Could not find published Blazor app: {htmlPath}");
                return;
            }

            await WebView.EnsureCoreWebView2Async();
            // Map the local output folder to a same-origin HTTPS host so WebAssembly runtime files
            // such as _framework/dotnet.js and _content/WebviewBind.Blazor/webviewHostBridge.js
            // can be loaded without hitting file:// CORS restrictions.
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                BlazorAppHostName,
                appFolderPath,
                CoreWebView2HostResourceAccessKind.Allow);
            RegisterWebviewHandler(WebView.CoreWebView2);
            // Once the folder mapping is active, navigate through the virtual host rather than
            // a direct file path so every static asset stays on the same origin.
            WebView.Source = new Uri($"https://{BlazorAppHostName}/index.html");
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    [WebviewBind.WebviewExport]
    private string GetGreeting(string userName)
    {
        return $"Hello {userName}! This message came from the WPF host.";
    }

    [WebviewBind.WebviewExport]
    private string OpenLoadFileDialog()
    {
        var openFileDialog = new OpenFileDialog();
        return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
    }

    [WebviewBind.WebviewExport]
    private async Task<string> GetMessageAsync(string arg)
    {
        await Task.Delay(1200);
        return $"{arg}, async reply from WPF at {DateTime.Now:T}.";
    }
}