using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace WpfWithHtml;

[WebviewBind.WebviewContainer]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs args)
    {
        try
        {
            var htmlPath = Path.Combine(AppContext.BaseDirectory, "WebFiles", "index.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show($"Could not find HTML file: {htmlPath}");
                return;
            }

            await WebView.EnsureCoreWebView2Async();
            // Here we register the WebView message handler that allows JavaScript to call C# methods marked with [WebviewExport].
            RegisterWebviewHandler(WebView.CoreWebView2);
            WebView.Source = new Uri(htmlPath);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    // Marking this method with [WebviewExport] makes it callable from JavaScript via InvokeHostMethod("GetGreeting", userName).
    // Parameters can be of any type that can be serialized/deserialized to/from JSON, and the return value will be sent back to JavaScript as a Promise result.
    [WebviewBind.WebviewExport]
    private string GetGreeting(string userName)
    {
        return $"Hello {userName}!";
    }
    
    [WebviewBind.WebviewExport]
    private string OpenLoadFileDialog()
    {
        var openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }
        return string.Empty;
    }

    [WebviewBind.WebviewExport]
    private async Task<string> GetMessageAsync(string arg)
    {
        await Task.Delay(2000); // Simulate async work
        return $"{arg}, This is an async message from C#.";
    }
}