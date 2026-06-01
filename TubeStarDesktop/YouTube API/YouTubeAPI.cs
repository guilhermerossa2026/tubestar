using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TubeStar
{
    public class YouTubeAPI
    {
        private const string DefaultApiKey = "AIzaSyDt2IG2mpYGRgadq9mMCndggF_uPKEsPsE";

        public static string GetCategoryFallbackImageUrl(VideoCategory category)
        {
            switch (category)
            {
                case VideoCategory.UkuleleCover: return "https://images.unsplash.com/photo-1507838153414-b4b713384a76?w=400";
                case VideoCategory.AnimalVSAnimal: return "https://images.unsplash.com/photo-1474511320723-9a56873867b5?w=400";
                case VideoCategory.Technology: return "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400";
                case VideoCategory.MusicVideo: return "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=400";
                case VideoCategory.TheWeirdSide: return "https://images.unsplash.com/photo-1509248961158-e54f6934749c?w=400";
                case VideoCategory.Sports: return "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?w=400";
                case VideoCategory.Comedy: return "https://images.unsplash.com/photo-1527224857830-43a7acc85260?w=400";
                case VideoCategory.Gaming: return "https://images.unsplash.com/photo-1538481199705-c710c4e965fc?w=400";
                case VideoCategory.HowTo: return "https://images.unsplash.com/photo-1513258496099-48168024aec0?w=400";
                case VideoCategory.Hauls: return "https://images.unsplash.com/photo-1483985988355-763728e1935b?w=400";
                case VideoCategory.Cats: return "https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?w=400";
                case VideoCategory.ConspiraryTheories: return "https://images.unsplash.com/photo-1509198397868-475647b2a1e5?w=400";
                case VideoCategory.Vlog: return "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=400";
                case VideoCategory.Accidents: return "https://images.unsplash.com/photo-1501139083538-0139583c060f?w=400";
                case VideoCategory.NonProfit: return "https://images.unsplash.com/photo-1488521787991-ed7bbaae773c?w=400";
                case VideoCategory.Anime: return "https://images.unsplash.com/photo-1607604276583-eef5d076aa5f?w=400";
                case VideoCategory.Movies: return "https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?w=400";
                case VideoCategory.Creepypasta: return "https://images.unsplash.com/photo-1505635552518-3448ff116af3?w=400";
                default: return "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400";
            }
        }

        public static void GetRandomImagesAsync(string search, int max, Action<List<string>> onCompleted)
        {
            SearchVideosAsync(search, max, (entries) =>
            {
                var ids = new List<string>();
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        if (entry.Id != null && !string.IsNullOrEmpty(entry.Id.VideoId))
                        {
                            ids.Add(entry.Id.VideoId);
                        }
                    }
                }
                onCompleted(ids);
            });
        }

        public static void SearchVideosAsync(string search, int max, Action<List<YouTubeSearchEntry>> onCompleted)
        {
            string customKey = Settings.CustomYouTubeApiKey;
            if (!string.IsNullOrEmpty(customKey))
            {
                string query = HttpHelpers.UrlEncoding(search.Replace(' ', '+'));
                var uriString = String.Format("https://www.googleapis.com/youtube/v3/search?part=snippet&q={0}&type=video&&maxResults={1}&fields=nextPageToken,prevPageToken,items(id(videoId),snippet(title))&key={2}",
                    query, max, customKey);

                WebClientHelpers.Download<YouTubeSearchResponse>(new Uri(uriString), (response) =>
                {
                    if (response != null && response.Entries != null && response.Entries.Count > 0)
                    {
                        onCompleted(response.Entries);
                    }
                    else
                    {
                        SearchVideosKeylessAsync(search, max, onCompleted);
                    }
                }, () =>
                {
                    SearchVideosKeylessAsync(search, max, onCompleted);
                });
            }
            else
            {
                SearchVideosKeylessAsync(search, max, onCompleted);
            }
        }

        public static List<YouTubeSearchEntry> SearchVideosKeyless(string search, int max)
        {
            var entries = new List<YouTubeSearchEntry>();

            // 1. Try public Invidious API
            try
            {
                string query = HttpHelpers.UrlEncoding(search.Replace(' ', '+'));
                string invidiousUrl = string.Format("https://yewtu.be/api/v1/search?q={0}&type=video", query);

                using (var client = new TimeoutWebClient(1200))
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    string json = client.DownloadString(invidiousUrl);
                    var items = JsonConvert.DeserializeObject<List<InvidiousVideoItemRaw>>(json);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            if (!string.IsNullOrEmpty(item.VideoId))
                            {
                                entries.Add(new YouTubeSearchEntry
                                {
                                    Id = new YouTubeSearchId { VideoId = item.VideoId },
                                    Snippet = new YouTubeSearchTitle { Title = item.Title ?? search }
                                });
                                if (entries.Count >= max) break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback to HTML scrape
            }

            // 2. YouTube HTML scrape
            if (entries.Count == 0)
            {
                try
                {
                    string query = HttpHelpers.UrlEncoding(search.Replace(' ', '+'));
                    string ytUrl = string.Format("https://www.youtube.com/results?search_query={0}", query);
                    using (var client = new TimeoutWebClient(1200))
                    {
                        client.Encoding = Encoding.UTF8;
                        client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                        string html = client.DownloadString(ytUrl);

                        var matches = System.Text.RegularExpressions.Regex.Matches(html, @"""videoId""\s*:\s*""([a-zA-Z0-9_-]{11})""");
                        var titles = System.Text.RegularExpressions.Regex.Matches(html, @"""title""\s*:\s*\{\s*""runs""\s*:\s*\[\s*\{\s*""text""\s*:\s*""([^""]+)""");

                        for (int i = 0; i < matches.Count; i++)
                        {
                            string id = matches[i].Groups[1].Value;
                            string title = (i < titles.Count) ? titles[i].Groups[1].Value : (search + " #" + (i + 1));

                            try
                            {
                                title = System.Text.RegularExpressions.Regex.Unescape(title);
                            }
                            catch { }

                            if (!entries.Exists(e => e.Id.VideoId == id))
                            {
                                entries.Add(new YouTubeSearchEntry
                                {
                                    Id = new YouTubeSearchId { VideoId = id },
                                    Snippet = new YouTubeSearchTitle { Title = title }
                                });
                                if (entries.Count >= max) break;
                            }
                        }
                    }
                }
                catch
                {
                    // Silence
                }
            }

            // 3. absolute fallback offline
            if (entries.Count == 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    entries.Add(new YouTubeSearchEntry
                    {
                        Id = new YouTubeSearchId { VideoId = "" },
                        Snippet = new YouTubeSearchTitle { Title = search + " Video " + i }
                    });
                }
            }

            return entries;
        }

        public static void SearchVideosKeylessAsync(string search, int max, Action<List<YouTubeSearchEntry>> onCompleted)
        {
            var bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                e.Result = SearchVideosKeyless(search, max);
            };
            bw.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error == null && e.Result != null)
                {
                    onCompleted((List<YouTubeSearchEntry>)e.Result);
                }
                else
                {
                    onCompleted(new List<YouTubeSearchEntry>());
                }
            };
            bw.RunWorkerAsync();
        }

        public static Uri GetPhotoUri(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new Uri("pack://application:,,,/TubeStar;component/Resources/InternetDown.jpg", UriKind.Absolute);
            }
            return new Uri(String.Format("http://i.ytimg.com/vi/{0}/hqdefault.jpg", id));
        }

        public static Uri GetSmallPhotoUri(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new Uri("pack://application:,,,/TubeStar;component/Resources/InternetDown.jpg", UriKind.Absolute);
            }
            return new Uri(String.Format("http://i.ytimg.com/vi/{0}/default.jpg", id));
        }

        public static Uri GetLinkUri(string id)
        {
            return new Uri(String.Format("http://www.youtube.com/watch?v={0}", id));
        }

        public static Uri GetRandomComments(string videoId, int max)
        {
            // Just return a dummy Uri since YouTube Comment API is blocked
            // This allows CommentGenerator.cs to compile cleanly and fall back to local comments database
            return new Uri("http://localhost/dummy_comments");
        }
    }

    [JsonObject()]
    public class InvidiousVideoItemRaw
    {
        [JsonProperty("videoId")]
        public string VideoId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    [JsonObject()]
    public class YouTubeCommentResponse
    {
        [JsonProperty("items")]
        public List<YouTubeCommentItemSnippet> Entries { get; set; }
    }

    [JsonObject()]
    public class YouTubeCommentItemSnippet
    {
        [JsonProperty("snippet")]
        public YouTubeTopLevelComment Snippet { get; set; }
    }

    [JsonObject()]
    public class YouTubeTopLevelComment
    {
        [JsonProperty("topLevelComment")]
        public YouTubeCommentSnippet TopLevelComment { get; set; }
    }

    [JsonObject()]
    public class YouTubeCommentSnippet
    {
        [JsonProperty("snippet")]
        public YouTubeComment Snippet { get; set; }
    }

    [JsonObject()]
    public class YouTubeComment
    {
        [JsonProperty("textDisplay")]
        public string Comment { get; set; }
    }

    [JsonObject()]
    public class YouTubeSearchResponse
    {
        [JsonProperty("prevPageToken")]
        public string PreviousPageToken { get; set; }

        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; set; }

        [JsonProperty("items")]
        public List<YouTubeSearchEntry> Entries { get; set; }
    }

    [JsonObject()]
    public class YouTubeSearchEntry
    {
        [JsonProperty("id")]
        public YouTubeSearchId Id { get; set; }

        [JsonProperty("snippet")]
        public YouTubeSearchTitle Snippet { get; set; }
    }

    [JsonObject()]
    public class YouTubeSearchTitle
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    [JsonObject()]
    public class YouTubeSearchId
    {
        [JsonProperty("videoId")]
        public string VideoId { get; set; }
    }
}