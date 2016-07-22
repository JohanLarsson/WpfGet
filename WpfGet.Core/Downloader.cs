namespace WpfGet.Core
{
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    public static class Downloader
    {
        public static async Task<string> DownloadStringAsync(string url)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            using (var pump = new MessagePump())
            {
                pump.Post(async s => await Navigate(s).ConfigureAwait(false), new UrlAndCompletionSource(url, taskCompletionSource));
                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        private static async Task Navigate(UrlAndCompletionSource state)
        {
            using (var browser = new WebBrowser())
            {
                NavigatedEventHandler onNavigated = null;
                onNavigated = (sender, _) =>
                {
                    var b = (WebBrowser)sender;
                    b.Navigated -= onNavigated;
                    state.TaskCompletionSource.SetResult(((mshtml.HTMLDocument)b.Document).body.innerText);
                };
                browser.Navigated += onNavigated;
                browser.Navigate(state.Url);
                await state.TaskCompletionSource.Task.ConfigureAwait(true);
            }
        }

        private struct UrlAndCompletionSource
        {
            internal readonly string Url;
            internal readonly TaskCompletionSource<string> TaskCompletionSource;

            public UrlAndCompletionSource(string url, TaskCompletionSource<string> taskCompletionSource)
            {
                this.Url = url;
                this.TaskCompletionSource = taskCompletionSource;
            }
        }
    }
}
