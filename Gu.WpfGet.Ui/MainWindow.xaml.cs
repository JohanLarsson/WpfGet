namespace Gu.WpfGet.Ui
{
    using System.IO;
    using System.Windows;
    using System.Windows.Navigation;

    public partial class MainWindow : Window
    {
        private readonly string fileName;

        public MainWindow(string urlString, string fileName)
        {
            this.fileName = fileName;
            this.InitializeComponent();
            this.Browser.Navigate(urlString);
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var document = (mshtml.HTMLDocument)this.Browser.Document;
            var innerText = document.body.innerText;
            File.WriteAllText(this.fileName, innerText);
            this.Close();
        }
    }
}
