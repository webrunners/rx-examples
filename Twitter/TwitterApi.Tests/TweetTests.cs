using System;
using NFluent;
using NUnit.Framework;

namespace TwitterApi.Tests
{
    [TestFixture]
    public class TweetTests
    {
        [Test]
        public void Parse_validInput_works()
        {
            Tweet tweet;

            var expected = new Tweet(
                user: "user123",
                text: "bla bla bla",
                topic: "singing",
                url: @"https://twitter.com/statuses/456",
                profileImageUrl: @"http://pbs.twimg.com/profile_images/123/XN98Qfnx_normal.jpg",
                userFavouritesCount: 4086,
                userFollowersCount: 640,
                timeStamp: new DateTime(2015, 5, 21));

            if (Tweet.TryParse(
                json: Properties.Resources.Json,
                topic: "singing",
                tweet: out tweet))
            {
                Check.That(tweet.Text).IsEqualTo(expected.Text);
                Check.That(tweet.User).IsEqualTo(expected.User);
                Check.That(tweet.Topic).IsEqualTo(expected.Topic);
                Check.That(tweet.Url).IsEqualTo(expected.Url);
                Check.That(tweet.ProfileImageUrl).IsEqualTo(expected.ProfileImageUrl);
                Check.That(tweet.UserFavouritesCount).IsEqualTo(expected.UserFavouritesCount);
                Check.That(tweet.UserFollowersCount).IsEqualTo(expected.UserFollowersCount);
            }
            else
            {
                Assert.Fail("Result should be Some");
            }
        }

    }
}
