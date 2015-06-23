using System;
using System.Windows;
using RxTwitterSample.viewmodels;
using TwitterApi;

namespace RxTwitterSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var twitter = new Twitter();
            var mainWindow = new MainWindow();
            var vm = new MainWindowViewModel(TweetAnalysis.Classify , twitter.AllTweetsAbout);
            mainWindow.DataContext = vm;
            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            Environment.Exit(1);
        }
    }
}
