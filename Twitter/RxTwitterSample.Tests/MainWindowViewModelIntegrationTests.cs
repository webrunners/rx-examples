using System;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;
using RxTwitterSample.viewmodels;
using TwitterApi;

namespace RxTwitterSample.Tests
{
    [TestFixture]
    public class MainWindowViewModelIntegrationTests
    {
        /// <summary>
        /// This test is dependend on time. So it is rather an integration test than a unit test.
        /// To remove the time dependency a TestScheduler can be used. For more information check out: http://www.introtorx.com/content/v1.0.10621.0/16_TestingRx.html
        /// 
        /// For this test to work make the following chnges:
        ///      In MainWindowViewModel:
        ///          - A new parameter named 'schedulerProvider' of type ISchedulerProvider has to be added to the constructor
        ///          - The line .ObserOnDispatcher() has to be changed to .ObserveOn(schedulerProvider.Dispatcher)
        ///      In App.xaml.xs:
        ///          - A SchedulerProvider instance has to be passed to the view model ctor
        ///      In this test:
        ///          - An ImmediateSchedulers instance has to be passed to the view model ctor
        /// </summary>
        [Ignore, Test, Category("IntegrationTest")]
        public void Ctor_setSearchText_tweetIsAddToList()
        {
            // arrange
            var tweet = new Tweet(
                user : "user",
                text : "foobar",
                topic : "topic",
                url : "https://twitter.com/statuses/123",
                profileImageUrl : "https://pbs.twimg.com/profile_images/456/abc.jpeg",
                userFavouritesCount : 42,
                userFollowersCount : 23,
                timeStamp : new DateTime(2015, 1, 1));

            Func<string, IObservable<Tweet>> allTweetsAbout =
                s => new[] { tweet }.ToObservable();

            Func<Tweet, ClassifiedTweet> scoreTweet =
                x => new ClassifiedTweet(x, 2000);

            // act
            //var vm = new MainWindowViewModel(scoreTweet, allTweetsAbout, new ImmediateSchedulers()) { SearchTextBox = "qwertz" };
            var vm = new MainWindowViewModel(scoreTweet, allTweetsAbout) { SearchTextBox = "qwertz" };

            Thread.Sleep(6000);

            // assert
            Assert.That(vm.AwesomeTweets.Count, Is.EqualTo(1));
        }
    }
}
