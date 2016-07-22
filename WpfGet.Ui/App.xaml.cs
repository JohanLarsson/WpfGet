namespace WpfGet.Ui
{
    using System.Windows;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow(e.Args[0], e.Args[1]);
            window.ShowDialog();
        }
    }
}
