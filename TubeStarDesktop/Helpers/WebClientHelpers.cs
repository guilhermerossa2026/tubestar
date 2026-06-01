using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TubeStar
{
    public class TimeoutWebClient : WebClient
    {
        private readonly int _timeoutMs;

        public TimeoutWebClient(int timeoutMs = 1200)
        {
            _timeoutMs = timeoutMs;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = _timeoutMs;
                var httpRequest = request as HttpWebRequest;
                if (httpRequest != null)
                {
                    httpRequest.ReadWriteTimeout = _timeoutMs;
                }
            }
            return request;
        }
    }

    public static class WebClientHelpers
    {
        public static void Download(Uri uri, Action<string> onCompleted, Action onError)
        {
            var client = new TimeoutWebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    onCompleted(e.Result);
                }
                else
                {
                    if (onError != null)
                        onError();
                }
            };
            client.DownloadStringAsync(uri);
        }

        public static void Download<T>(Uri uri, Action<T> onCompleted, Action onError)
        {
            var client = new TimeoutWebClient();
            client.Encoding = Encoding.UTF8;
            client.DownloadStringCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        var jsonResult = JsonConvert.DeserializeObject<T>(e.Result);
                        onCompleted(jsonResult);
                    }
                    else
                    {
                        if (onError != null)
                            onError();
                    }
                };
            client.DownloadStringAsync(uri);
        }

        public static void DownloadImage(Uri imageUri, Action<Stream> onCompleted, Action onError)
        {
            var client = new TimeoutWebClient();
            client.OpenReadCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        onCompleted(e.Result);
                    }
                    else
                    {
                        if (onError != null)
                            onError();
                    }
                };
            client.OpenReadAsync(imageUri, client);
        }

        public static void DownloadPostData<T>(string uri, string dataKey, string data)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Encoding = System.Text.UTF8Encoding.UTF8;
                var reqparm = new NameValueCollection();
                reqparm.Add(dataKey, data);
                client.UploadValues(uri, "POST", reqparm);
            }
        }
    }
}