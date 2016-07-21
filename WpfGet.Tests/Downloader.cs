using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WpfGet.Tests
{
    using System.Diagnostics;
    using System.IO;

    [TestClass]
    public class Downloader
    {
        [TestMethod]
        public void Download()
        {
            var url = @"https://raw.githubusercontent.com/fsharp/FAKE/f4024fd5b790485828c1cfc002190716eee97597/modules/Octokit/Octokit.fsx";
            var fileName = @"C:\Temp\Octokit.fsx";
            File.Delete(fileName);
            var process = Process.Start(Path.Combine(Environment.CurrentDirectory, "WpfGet.exe"), $@"{url} {fileName}");
            process.WaitForExit();
            Assert.IsTrue(File.Exists(fileName));
        }
    }
}
