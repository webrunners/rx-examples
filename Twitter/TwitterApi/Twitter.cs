    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using LinqToTwitter;

    namespace TwitterApi
    {
        public class Twitter
        {
            private readonly SingleUserAuthorizer _auth = new SingleUserAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                    OAuthToken = ConfigurationManager.AppSettings["authtoken"],
                    OAuthTokenSecret = ConfigurationManager.AppSettings["authtokensecret"],
                }
            };

            private readonly TwitterContext _twitterCtx;

            public Twitter()
            {
                if (String.IsNullOrWhiteSpace(_auth.CredentialStore.ConsumerKey)
                    || String.IsNullOrWhiteSpace(_auth.CredentialStore.ConsumerSecret)
                    || String.IsNullOrWhiteSpace(_auth.CredentialStore.OAuthToken)
                    || String.IsNullOrWhiteSpace(_auth.CredentialStore.OAuthTokenSecret))
                    throw new Exception("User Credentials are not set. Please update your App.config file.");

                _twitterCtx = new TwitterContext(_auth);
            }

            private IObservable<StreamContent> TwitterStream(string track)
            {
                return Observable.Create<StreamContent>(o =>
                {
                    var query = from s in _twitterCtx.Streaming
                        where s.Type == StreamingType.Filter
                              && s.Track == track
                        select s;

                    var disposed = false;

                    query.StartAsync(s =>
                    {
                        if (disposed)
                            s.CloseStream();

                        o.OnNext(s);

                        return Task.FromResult(s);
                    });

                    return Disposable.Create(() => { disposed = true; });
                });
            }

            public IObservable<Tweet> AllTweetsAbout(string topic)
            {
                return TwitterStream(topic)
                    .Where(x => x.EntityType == StreamEntityType.Status)
                    .Where(x => Predicate(x.Entity as Status, topic))
                    .Select(status =>
                    {
                        Tweet tweet;
                        Tweet.TryParse(status.Content, topic, out tweet);
                        return tweet;
                    })
                    .Where(t => t != null);
            }

            private static bool Predicate(Status status, string topic)
            {
                return status.Text.ToLower().Contains(topic.ToLower());
            }
        }
    }
