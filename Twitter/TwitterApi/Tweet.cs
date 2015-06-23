using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace TwitterApi
{
    public class Tweet
    {
        public string User { get; private set; }
        public string Text { get; private set; }
        public string Topic { get; private set; }
        public string Url { get; private set; }
        public string ProfileImageUrl { get; private set; }
        public int UserFavouritesCount { get; private set; }
        public int UserFollowersCount { get; private set; }
        public DateTime TimeStamp { get; private set; }

        private Tweet()
        {

        }

        public Tweet(string user, string text, string topic, string url, string profileImageUrl, int userFavouritesCount, int userFollowersCount, DateTime timeStamp)
        {
            User = user;
            Text = text;
            Topic = topic;
            Url = url;
            ProfileImageUrl = profileImageUrl;
            UserFavouritesCount = userFavouritesCount;
            UserFollowersCount = userFollowersCount;
            TimeStamp = timeStamp;
        }

        public static bool TryParse(string json, string topic, out Tweet tweet)
        {
            tweet = null;
            try
            {
                tweet = ParseTweet(json, topic);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        } 

        private static Tweet ParseTweet(string json, string topic)
        {
            const string format = "ddd MMM dd HH:mm:ss zzzz yyyy";

            dynamic parsed = JObject.Parse(json);
            return new Tweet()
            {
                User = parsed.user.screen_name,
                Text = parsed.text,
                Topic = topic,
                Url = String.Format("https://twitter.com/statuses/{0}", parsed.id),
                ProfileImageUrl = parsed.user.profile_image_url,
                UserFavouritesCount = parsed.user.favourites_count,
                UserFollowersCount = parsed.user.followers_count,
                TimeStamp = DateTime.ParseExact((string)parsed.created_at, format, CultureInfo.InvariantCulture)
            };
        }

        public override string ToString()
        {
            return Text;
        }
    }
}