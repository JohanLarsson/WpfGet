namespace WpfGet.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WpfGet.Core;

    [TestClass]
    public class DownloaderTests
    {
        [TestMethod]
        public void DownloadUsingWindow()
        {
            var url = @"https://raw.githubusercontent.com/fsharp/FAKE/f4024fd5b790485828c1cfc002190716eee97597/modules/Octokit/Octokit.fsx";
            var fileName = @"C:\Temp\Octokit.fsx";
            File.Delete(fileName);
            var process = Process.Start(Path.Combine(Environment.CurrentDirectory, "WpfGet.exe"), $@"{url} {fileName}");
            process.WaitForExit();
            Assert.IsTrue(File.Exists(fileName));
        }

        //[TestMethod]
        public async Task DownloadStringAsyncTest()
        {
            var url = @"https://raw.githubusercontent.com/fsharp/FAKE/f4024fd5b790485828c1cfc002190716eee97597/modules/Octokit/Octokit.fsx";
            var task = Downloader.DownloadStringAsync(url);
            var text = await task.ConfigureAwait(false);
        }

        [TestMethod]
        public async Task WinformsDownloadStringAsyncTest()
        {
            var url = @"https://raw.githubusercontent.com/fsharp/FAKE/f4024fd5b790485828c1cfc002190716eee97597/modules/Octokit/Octokit.fsx";
            var task = WinFormsGet.Downloader.DownloadStringAsync(url);
            var text = await task.ConfigureAwait(false);
            Console.Write(text);
        }
    }
}
