using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TubeStar
{
    public class SerializableShareData
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
    }

    public static class VideoViewer
    {
        private static ConcurrentDictionary<Guid, int> _videoShares;
        private static ConcurrentDictionary<Guid, int> _videoBoughtViews;

        public static List<SerializableShareData> GetShares()
        {
            List<SerializableShareData> data = new List<SerializableShareData>();
            foreach (var item in _videoShares)
            {
                data.Add(new SerializableShareData() { Id = item.Key, Count = item.Value });
            }
            return data;
        }

        public static void SetShares(List<SerializableShareData> data)
        {
            _videoShares.Clear();
            foreach (var item in data)
            {
                _videoShares[item.Id] = item.Count;
            }
        }

        public static List<SerializableShareData> GetBoughtViews()
        {
            List<SerializableShareData> data = new List<SerializableShareData>();
            foreach (var item in _videoBoughtViews)
            {
                data.Add(new SerializableShareData() { Id = item.Key, Count = item.Value });
            }
            return data;
        }

        public static void SetBoughtViews(List<SerializableShareData> data)
        {
            _videoBoughtViews.Clear();
            foreach (var item in data)
            {
                _videoBoughtViews[item.Id] = item.Count;
            }
        }

        static VideoViewer()
        {
            Reset();
        }

        public static void Reset()
        {
            _videoShares = new ConcurrentDictionary<Guid, int>();
            _videoBoughtViews = new ConcurrentDictionary<Guid, int>();
        }

        public static void BuyViews(Video video, int number)
        {
            if (!_videoBoughtViews.ContainsKey(video.Id))
            {
                _videoBoughtViews[video.Id] = 0;
            }

            _videoBoughtViews[video.Id] += number;
        }

        public static void FreeViews(Video video, int number)
        {
            Initialize(video);
            _videoShares[video.Id] += number;
        }

        public static void SubscriberView(Channel channel, Video video)
        {
            Initialize(video);
            _videoShares[video.Id] += (int)(channel.Subscribers * (Player.Current.ChallengeMode ? 0.2 : 0.4));
        }

        public static void Iteration(Channel channel, Video video, ref double dailyIncome, ref double dailyExpenses)
        {
            Initialize(video);
            video.Iterations++;

            if (channel.IsSuspended || video.IsSuspended)
            {
                _videoShares[video.Id] = 0;
                _videoBoughtViews[video.Id] = 0;
                return;
            }

            int tempShares = (int)(_videoShares[video.Id] * (Player.Current.ChallengeMode ? 0.1 : 0.2)); 
            _videoShares[video.Id] -= tempShares;

            int payedViews = _videoBoughtViews.ContainsKey(video.Id) ? _videoBoughtViews[video.Id] : 0;
            _videoBoughtViews[video.Id] = 0;

            var searchEngineViews = video.Views / (50 * video.Iterations);
            var qualityViews = Math.Max(0, ((video.Quality.Value - Math.Pow(video.Iterations, Player.Current.ChallengeMode ? 2 : 1)) * video.Quality.Value) / Math.Max((Player.Current.ChallengeMode ? 2 : 1), (100 - video.Quality.Value) / 2));
            var iterationViews = (int)(searchEngineViews + qualityViews + tempShares);

            if (iterationViews > 0)
            {
                ViewVideo(channel, video, ref dailyIncome, ref dailyExpenses, iterationViews);
            }

            if (payedViews > 0)
            {
                ViewVideo(channel, video, ref dailyIncome, ref dailyExpenses, payedViews, true);
            }

            if (channel.Subscribers < channel.MinimumSubsribers)
                channel.Subscribers = channel.MinimumSubsribers;
        }

        private static void ViewVideo(Channel channel, Video video, ref double income, ref double expenses, int numViews, bool payedView = false)
        {
            CommentType? commentType = null;

            video.Views += numViews;
            income += ViewIncome(channel, video, payedView, numViews);
            expenses += video.CostPerView * numViews;

            // Second watcher attribute
            if (video.Attributes.Contains(VideoAttributes.SecondTime))
            {
                int secondTimeViews = 0;
                for (int c = 0; c < 5; c++)
                {
                    if (RandomHelpers.Chance(5))
                    {
                        secondTimeViews += RandomHelpers.RandomInt((numViews * 2) / 5);
                    }
                }
                if (secondTimeViews > 0)
                {
                    video.Views += secondTimeViews;
                    income += ViewIncome(channel, video, payedView, secondTimeViews);
                    expenses += video.CostPerView * secondTimeViews;
                }
            }

            // Decide if comment triggers
            if (RandomHelpers.Chance(20))
            {
                if (RandomHelpers.Chance(50))
                    commentType = CommentType.Random;
            }

            if (CategoryHelpers.CheckInterest(video))
            {
                double likeChance = video.Quality.Value / 100.0;
                int likedViews = (int)(numViews * likeChance);
                video.Likes += likedViews;

                if (likedViews > 0)
                {
                    if (commentType == null && RandomHelpers.Chance(5))
                        commentType = CommentType.Like;

                    double subChance = GetSubscriptionChancePercentage(channel, video);
                    int subs = (int)(likedViews * subChance);

                    if (video.Attributes.Contains(VideoAttributes.Crowdfunding))
                    {
                        if (!payedView)
                            Player.Current.Money += subs * VideoAttributes.Crowdfunding.Money;
                    }
                    else
                    {
                        channel.Subscribers += subs;
                        if (subs > 0 && commentType == null && RandomHelpers.Chance(5))
                            commentType = CommentType.Subscribe;
                    }

                    ShareVideo(video, likedViews);
                }

                // Dislikes from remaining views
                int remainingViews = numViews - likedViews;
                if (remainingViews > 0 && RandomHelpers.RandomBool())
                {
                    int dislikedViews = remainingViews / 2;
                    video.Dislikes += dislikedViews;

                    if (dislikedViews > 0)
                    {
                        if (commentType == null && RandomHelpers.Chance(5))
                            commentType = CommentType.Dislike;

                        if (CheckUnsubscriptions(channel, video, dislikedViews, payedView))
                        {
                            if (commentType == null && RandomHelpers.Chance(5))
                                commentType = CommentType.UnsubscribeQuality;
                        }

                        if (video.Attributes.Contains(VideoAttributes.SoBad))
                        {
                            ShareVideo(video, dislikedViews);
                        }
                    }
                }
            }
            else
            {
                if (CheckUnsubscriptions(channel, video, numViews, payedView))
                {
                    if (commentType == null && RandomHelpers.Chance(5))
                        commentType = CommentType.UnsubscribeCategory;
                }
            }

            if (!channel.IsRivalChannel && commentType.HasValue)
            {
                var comment = CommentGenerator.GenerateComment(video, commentType.Value);
                var first = video.Comments.FirstOrDefault(c => c.Comment == comment);
                if (first != null)
                    first.Likes++;
                else
                    video.Comments.Add(new VideoComment(comment, 0));
            }
        }

        private static bool CheckUnsubscriptions(Channel channel, Video video, int numViews, bool payedView)
        {
            if (!channel.IsRivalChannel && !payedView)
            {
                int chance = (channel.Subscribers / Math.Max(1, video.Views)) * 100;
                if (!Player.Current.ChallengeMode) chance = chance / 2;
                if (RandomHelpers.Chance(25) && GetUnsubscriptionChance(channel) && RandomHelpers.Chance(chance))
                {
                    channel.Subscribers -= numViews;
                    return true;
                }
            }
            return false;
        }

        private static void ShareVideo(Video video, int numViews)
        {
            int memeticMutator = video.Attributes.Contains(VideoAttributes.Memetic) ? 20 : 0;
            if (RandomHelpers.RandomInt(100 + memeticMutator) >= 60)
            {
                var friends = RandomHelpers.RandomInt(2 * numViews); // Reduced scale factor to balance large values
                _videoShares[video.Id] += friends;

                if (RandomHelpers.Chance(1) && RandomHelpers.Chance(10))
                {
                    var subscribers = RandomHelpers.RandomInt((int)(25000 * (Player.Current.ChallengeMode ? 0.02 : 0.05)));
                    _videoShares[video.Id] += (int)(subscribers * 0.25);
                }
            }
        }

        private static double GetSubscriptionChancePercentage(Channel channel, Video video)
        {
            int initialChance = 0;
            switch (channel.Advertising)
            {
                case (AdvertisingStrategy.Low):
                    initialChance = 90;
                    break;
                case (AdvertisingStrategy.Normal):
                    initialChance = 70;
                    break;
                case (AdvertisingStrategy.High):
                    initialChance = 50;
                    break;
                case (AdvertisingStrategy.Aggressive):
                    initialChance = 30;
                    break;
                default: throw new NotSupportedException();
            }

            if (video.Attributes.Contains(VideoAttributes.Hypnotic))
                initialChance += 30;

            // Balance subscription rate
            return (Math.Min(100, Math.Max(0, initialChance)) / 100.0) * 0.1;
        }

        private static bool GetUnsubscriptionChance(Channel channel)
        {
            switch (channel.Advertising)
            {
                case (AdvertisingStrategy.Low): return RandomHelpers.Chance(10);
                case (AdvertisingStrategy.Normal): return RandomHelpers.Chance(20);
                case (AdvertisingStrategy.High): return RandomHelpers.Chance(40);
                case (AdvertisingStrategy.Aggressive): return RandomHelpers.Chance(60);
                default: throw new NotSupportedException();
            }
        }

        private static double ViewIncome(Channel channel, Video video, bool payedView, int numViews)
        {
            var income = payedView ? 0 : channel.IncomePerView;
            if (Player.Current.ChallengeMode)
                income = income / 2;

            var extraIncome = Math.Sqrt(channel.Subscribers / (Player.Current.ChallengeMode ? 20000.0 : 1000.0)) * 0.01;
            extraIncome = Math.Min(extraIncome, Player.Current.ChallengeMode ? 0.25 : 0.50);
            income += extraIncome;

            if (Player.Current.ChallengeMode && RandomHelpers.Chance(90))
            {
                if (Player.Current.UltraMode)
                {
                    if (RandomHelpers.Chance(90))
                    {
                        income = 0;
                    }
                }
                else
                {
                    if (RandomHelpers.Chance(50))
                    {
                        income = 0;
                    }
                }
            }

            if (video.Attributes.Contains(VideoAttributes.ProductPlacement))
            {
                income += 0.01;
            }

            return income * numViews;
        }

        private static void Initialize(Video video)
        {
            if (!_videoShares.ContainsKey(video.Id))
            {
                _videoShares[video.Id] = 0;
            }
        }
    }
}