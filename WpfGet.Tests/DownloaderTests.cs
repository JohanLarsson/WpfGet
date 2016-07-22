namespace WpfGet.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DownloaderTests
    {
        private static readonly string Url = @"https://raw.githubusercontent.com/fsharp/FAKE/f4024fd5b790485828c1cfc002190716eee97597/modules/Octokit/Octokit.fsx";

        [TestMethod]
        public async Task DownloadStringAsyncReference()
        {
            // this may not work if behind a proxy.
            using (var client = new WebClient())
            {
                var sw = Stopwatch.StartNew();
                var text = await client.DownloadStringTaskAsync(Url).ConfigureAwait(false);
                sw.Stop();
                Console.WriteLine(sw.Elapsed);
                Console.Write(text);
                Assert.AreEqual(Properties.Resources.Octokit_fsx, text);
            }
        }

        [TestMethod]
        public void DownloadUsingWindow()
        {
            var fileName = @"C:\Temp\Octokit.fsx";
            File.Delete(fileName);
            var process = Process.Start(Path.Combine(Environment.CurrentDirectory, "WpfGet.Ui.exe"), $@"{Url} {fileName}");
            process.WaitForExit();
            Assert.IsTrue(File.Exists(fileName));
            Assert.AreEqual(Properties.Resources.Octokit_fsx, File.ReadAllText(fileName));
        }

        [TestMethod]
        public async Task DownloadStringAsyncTest()
        {
            var sw = Stopwatch.StartNew();
            var text = await WpfGet.Downloader.DownloadStringAsync(Url).ConfigureAwait(false);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            //Console.Write(text);
            Assert.AreEqual(Properties.Resources.Octokit_fsx, text);
        }

        [TestMethod]
        public async Task WinformsDownloadStringAsyncTest()
        {
            var sw = Stopwatch.StartNew();
            var text = await WinFormsGet.Downloader.DownloadStringAsync(Url).ConfigureAwait(false);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            //Console.Write(text);
            Assert.AreEqual(Properties.Resources.Octokit_fsx, text);
        }
    }
}
