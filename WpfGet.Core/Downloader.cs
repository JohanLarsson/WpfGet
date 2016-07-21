namespace WpfGet.Core
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Threading;

    public static class Downloader
    {
        public static Task<string> DownloadStringAsync(string urlString)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            using (var browser = new WebBrowser())
            {
                //// http://reedcopsey.com/2011/11/28/launching-a-wpf-window-in-a-separate-thread-part-1/
                //var thread = new Thread(() =>
                //{
                //    // Start the Dispatcher Processing
                //    System.Windows.Threading.Dispatcher.Run();
                //});

                //// Set the apartment state
                //thread.SetApartmentState(ApartmentState.STA);
                //// Make the thread a background thread
                //thread.IsBackground = true;
                //// Start the thread
                //thread.Start();

                browser.LoadCompleted += (o, e) =>
                {
                    //Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    taskCompletionSource.SetResult(GetText((WebBrowser)o));
                };

                browser.Navigate(urlString);
                return taskCompletionSource.Task;
            }
        }

        private static string GetText(WebBrowser webBrowser)
        {
            var document = (mshtml.HTMLDocument)webBrowser.Document;
            return document.body.innerText;
        }
    }
}
