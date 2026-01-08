using Foundation;
using Microsoft.Maui.ApplicationModel;
using WebKit;

namespace YourOwnJournal.Services;

public class MacCatalystPdfExportService : IPdfExportService
{
    public async Task ExportHtmlToPdfAsync(string html, string outputPath)
    {
        var loadTcs = new TaskCompletionSource<bool>();
        WKWebView? webView = null;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var config = new WKWebViewConfiguration();
            webView = new WKWebView(CoreGraphics.CGRect.Empty, config);
            webView.NavigationDelegate = new PdfNavigationDelegate(loadTcs);
            var baseUrl = new NSUrl(FileSystem.AppDataDirectory, true);
            webView.LoadHtmlString(html, baseUrl);
        });

        await loadTcs.Task;

        var pdfTcs = new TaskCompletionSource<NSData>();
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (webView == null)
            {
                pdfTcs.TrySetException(new InvalidOperationException("WebView not initialized."));
                return;
            }

            var config = new WKPdfConfiguration();
            webView.CreatePdf(config, (data, error) =>
            {
                if (error != null)
                {
                    pdfTcs.TrySetException(new InvalidOperationException(error.LocalizedDescription));
                }
                else if (data == null)
                {
                    pdfTcs.TrySetException(new InvalidOperationException("PDF generation failed."));
                }
                else
                {
                    pdfTcs.TrySetResult(data);
                }
            });
        });

        var pdfData = await pdfTcs.Task;
        var nsUrl = NSUrl.FromFilename(outputPath);
        NSError? writeError;
        pdfData.Save(nsUrl, false, out writeError);
        if (writeError != null)
        {
            throw new InvalidOperationException(writeError.LocalizedDescription);
        }
    }

    private sealed class PdfNavigationDelegate : WKNavigationDelegate
    {
        private readonly TaskCompletionSource<bool> _tcs;

        public PdfNavigationDelegate(TaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            _tcs.TrySetResult(true);
        }

        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            _tcs.TrySetException(new InvalidOperationException(error.LocalizedDescription));
        }

        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            _tcs.TrySetException(new InvalidOperationException(error.LocalizedDescription));
        }
    }
}
