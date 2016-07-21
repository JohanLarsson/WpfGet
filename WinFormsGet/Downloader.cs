namespace WinFormsGet
{
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public static class Downloader
    {
        public static async Task<string> DownloadStringAsync(string url)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            using (var pump = new MessagePump())
            {
                using (var browser = await pump.InvokeAsync(() => new WebBrowser()).ConfigureAwait(false))
                {
                    WebBrowserNavigatedEventHandler onNavigated = null;
                    onNavigated = async (o, _) =>
                    {
                        browser.Navigated -= onNavigated;
                        await pump.InvokeAsync(() => taskCompletionSource.SetResult(((WebBrowser)o).Document.Body.InnerText));
                    };

                    browser.Navigated += onNavigated;
                    await pump.InvokeAsync(() => browser.Navigate(url));
                    return await taskCompletionSource.Task.ConfigureAwait(false);
                }
            }
        }
    }
}