using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using TwitterApi;

namespace RxTwitterSample.viewmodels
{
    public class MainWindowViewModel : ViewModel
    {
        private string _searchTextBox;
        private ObservableCollection<Tweet> _awesomeTweets = new ObservableCollection<Tweet>();
        private ObservableCollection<Tweet> _boringTweets = new ObservableCollection<Tweet>();
        private string _headerAwesomeTweets = "Awesome Tweets";
        private string _headerBoringTweets = "Boring Tweets";

        public string HeaderAwesomeTweets
        {
            get { return _headerAwesomeTweets; }
            set { SetProperty(ref _headerAwesomeTweets, value); }
        }

        public string HeaderBoringTweets
        {
            get { return _headerBoringTweets; }
            set { SetProperty(ref _headerBoringTweets, value); }
        }

        public string SearchTextBox
        {
            get { return _searchTextBox; }
            set { SetProperty(ref _searchTextBox, value); }
        }

        public ObservableCollection<Tweet> AwesomeTweets
        {
            get { return _awesomeTweets; }
            set { SetProperty(ref _awesomeTweets, value); }
        }

        public ObservableCollection<Tweet> BoringTweets
        {
            get { return _boringTweets; }
            set { SetProperty(ref _boringTweets, value); }
        }

        public MainWindowViewModel(Func<Tweet, ClassifiedTweet> classifyTweet, Func<string, IObservable<Tweet>> allTweetsAbout)
        {
            var searchTexts = this
                .ToObservable(() => SearchTextBox)
                .Throttle(TimeSpan.FromSeconds(2))
                .Where(x => x.Length > 2);

            var tweets = searchTexts
                .Select(allTweetsAbout)
                .Switch()
                .Select(classifyTweet)
                .Sample(TimeSpan.FromSeconds(1))
                .Select(x => new { x.Score, x.Tweet })
                .Publish();

            tweets
                .Where(x => x.Score > 1000)
                .Select(x => x.Tweet)
                .ObserveOnDispatcher()
                .Subscribe(UpdateViewModelAwesomeTweets);

            tweets
                .Where(x => x.Score <= 1000)
                .Select(x => x.Tweet)
                .ObserveOnDispatcher()
                .Subscribe(UpdateViewModelBoringTweets);

            tweets.Connect();
        }

        private void UpdateViewModelBoringTweets(Tweet tweet)
        {
            AddToList(tweet, BoringTweets);
            HeaderBoringTweets = String.Format("Boring Tweets ({0})", BoringTweets.Count);
        }

        private void UpdateViewModelAwesomeTweets(Tweet tweet)
        {
            AddToList(tweet, AwesomeTweets);
            HeaderAwesomeTweets = String.Format("Awesome Tweets ({0})", AwesomeTweets.Count);
        }

        private static void AddToList(Tweet tweet, IList<Tweet> collection)
        {
            collection.Insert(0, tweet);
            if (collection.Count > 20)
                collection.Remove(collection.Last());
        }
    }
}
