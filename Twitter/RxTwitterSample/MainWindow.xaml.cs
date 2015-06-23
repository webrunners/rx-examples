using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TwitterApi;

namespace RxTwitterSample
{
    public partial class MainWindow
    {
        private static readonly Regex LinkToTweetPattern = new Regex(@"https://twitter.com/statuses/\d+", RegexOptions.IgnoreCase);

        public MainWindow()
        {
            InitializeComponent();

            Action<object> openInBrowser =
                obj =>
                {
                    var tweet = obj as Tweet;

                    if (obj == null) return;

                    var match = LinkToTweetPattern.Match(tweet.Url);
                    if (match.Success)
                        System.Diagnostics.Process.Start(match.Groups[0].ToString());
                };

            AwesomeTweetsListBox.MouseDoubleClick += (sender, args) => openInBrowser(AwesomeTweetsListBox.SelectedItem);
            BoringTweetsListBox.MouseDoubleClick += (sender, args) => openInBrowser(BoringTweetsListBox.SelectedItem);

            FocusManager.SetFocusedElement(this, SearchTextBox);
        }
    }
}
