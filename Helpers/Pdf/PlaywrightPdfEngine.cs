using Microsoft.Playwright;

namespace SchoolManagementSystem.Helpers.Pdf;

public class PlaywrightPdfEngine : IDisposable
{
    private readonly object _lock = new();
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private bool _initialized;
    private Task? _initTask;

    public byte[] Convert(string html, bool isBulk)
    {
        return Task.Run(() => ConvertAsync(html, isBulk)).GetAwaiter().GetResult();
    }

    private async Task<byte[]> ConvertAsync(string html, bool isBulk)
    {
        await EnsureInitializedAsync();
        var page = await _browser!.NewPageAsync();
        try
        {
            await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.NetworkIdle });
            var pdfOptions = new PagePdfOptions
            {
                PrintBackground = true,
                PreferCSSPageSize = true,
                Width = isBulk ? "297mm" : "53.98mm",
                Height = isBulk ? "210mm" : "85.60mm",
            };
            return await page.PdfAsync(pdfOptions);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;
        lock (_lock)
        {
            if (_initialized) return;
            _initTask ??= InitializeAsync();
        }
        await _initTask;
    }

    private async Task InitializeAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        });
        _initialized = true;
    }

    public void Dispose()
    {
        _browser?.CloseAsync().GetAwaiter().GetResult();
        _browser?.DisposeAsync().GetAwaiter().GetResult();
        _playwright?.Dispose();
    }
}
