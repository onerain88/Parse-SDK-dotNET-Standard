using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Parse.Internal.Json;
using Parse.Internal.File;

namespace Parse.Internal.Http {
    public class ParseHttpClient {
        private readonly string appId;
        private readonly string server;

        private readonly HttpClient client;

        public ParseHttpClient(string appId, string server) {
            this.appId = appId;
            this.server = server;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Parse-Application-Id", appId);
        }

        public async Task<T> Get<T>(string path,
            Dictionary<string, object> headers = null,
            Dictionary<string, object> queryParams = null) {
            string url = BuildUrl(path, queryParams);
            HttpRequestMessage request = new HttpRequestMessage {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            await FillHeaders(request.Headers, headers);

            PrintRequest(client, request);
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            request.Dispose();

            string resultString = await response.Content.ReadAsStringAsync();
            response.Dispose();
            PrintResponse(response, resultString);

            if (response.IsSuccessStatusCode) {
                T ret = JsonConvert.DeserializeObject<T>(resultString,
                    ParseJsonConverter.Default);
                return ret;
            }
            throw HandleErrorResponse(response.StatusCode, resultString);
        }

        public async Task<T> Post<T>(string path,
            Dictionary<string, object> headers = null,
            object data = null,
            Dictionary<string, object> queryParams = null) {
            string url = BuildUrl(path, queryParams);
            HttpRequestMessage request = new HttpRequestMessage {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };
            await FillHeaders(request.Headers, headers);

            string content = null;
            if (data != null) {
                content = JsonConvert.SerializeObject(data);
                StringContent requestContent = new StringContent(content);
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = requestContent;
            }

            PrintRequest(client, request, content);
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            request.Dispose();

            string resultString = await response.Content.ReadAsStringAsync();
            response.Dispose();
            PrintResponse(response, resultString);

            if (response.IsSuccessStatusCode) {
                T ret = JsonConvert.DeserializeObject<T>(resultString,
                    ParseJsonConverter.Default);
                return ret;
            }
            throw HandleErrorResponse(response.StatusCode, resultString);
        }

        public async Task<T> PostStream<T>(string path, Stream stream, Action<long, long> onProgress = null) {
            string url = BuildUrl(path);
            HttpRequestMessage request = new HttpRequestMessage {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
            };
            request.Content = new ParseProgressableStreamContent(new StreamContent(stream), onProgress);
            PrintRequest(client, request);
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            request.Dispose();

            string resultString = await response.Content.ReadAsStringAsync();
            response.Dispose();
            PrintResponse(response, resultString);

            if (response.IsSuccessStatusCode) {
                T ret = JsonConvert.DeserializeObject<T>(resultString,
                    ParseJsonConverter.Default);
                return ret;
            }
            throw HandleErrorResponse(response.StatusCode, resultString);
        }

        public async Task<T> Put<T>(string path,
            Dictionary<string, object> headers = null,
            object data = null,
            Dictionary<string, object> queryParams = null) {
            string url = BuildUrl(path, queryParams);
            HttpRequestMessage request = new HttpRequestMessage {
                RequestUri = new Uri(url),
                Method = HttpMethod.Put,
            };
            await FillHeaders(request.Headers, headers);

            string content = null;
            if (data != null) {
                content = JsonConvert.SerializeObject(data);
                StringContent requestContent = new StringContent(content);
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = requestContent;
            }
            PrintRequest(client, request, content);
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            request.Dispose();

            string resultString = await response.Content.ReadAsStringAsync();
            response.Dispose();
            PrintResponse(response, resultString);

            if (response.IsSuccessStatusCode) {
                T ret = JsonConvert.DeserializeObject<T>(resultString,
                    ParseJsonConverter.Default);
                return ret;
            }
            throw HandleErrorResponse(response.StatusCode, resultString);
        }

        public async Task Delete(string path) {
            string url = BuildUrl(path);
            HttpRequestMessage request = new HttpRequestMessage {
                RequestUri = new Uri(url),
                Method = HttpMethod.Delete
            };

            PrintRequest(client, request);
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            request.Dispose();

            string resultString = await response.Content.ReadAsStringAsync();
            response.Dispose();
            PrintResponse(response, resultString);

            if (response.IsSuccessStatusCode) {
                Dictionary<string, object> ret = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultString,
                    ParseJsonConverter.Default);
                return;
            }
            throw HandleErrorResponse(response.StatusCode, resultString);
        }

        private string BuildUrl(string path, Dictionary<string, object> queryParams = null) {
            string url = $"{server}/{path}";
            if (queryParams != null) {
                IEnumerable<string> queryPairs = queryParams.Select(kv => $"{kv.Key}={kv.Value}");
                string queries = string.Join("&", queryPairs);
                url = $"{url}?{queries}";
            }
            return url;
        }

        private ParseException HandleErrorResponse(HttpStatusCode statusCode, string responseContent) {
            int code = (int)statusCode;
            string message = responseContent;
            try {
                // 尝试获取 LeanCloud 返回错误信息
                Dictionary<string, object> error = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent,
                    ParseJsonConverter.Default);
                code = (int)error["code"];
                message = error["error"].ToString();
            } catch (Exception e) {
                ParseLogger.Error(e);
            }
            return new ParseException(code, message);
        }

        async Task FillHeaders(HttpRequestHeaders headers, Dictionary<string, object> additionalHeaders = null) {
            // 额外 headers
            if (additionalHeaders != null) {
                foreach (KeyValuePair<string, object> kv in additionalHeaders) {
                    headers.Add(kv.Key, kv.Value.ToString());
                }
            }
            // 当前用户 Session Token
            ParseUser currentUser = await ParseUser.GetCurrent();
            if (currentUser != null) {
                headers.Add("X-Parse-Session-Token", currentUser.SessionToken);
            }
        }

        private static void PrintRequest(HttpClient client, HttpRequestMessage request, string content = null) {
            if (ParseLogger.LogDelegate == null) {
                return;
            }

            ParseLogger.Debug(ParseHttpUtils.FormatRequest(client, request, content));
        }

        private static void PrintResponse(HttpResponseMessage response, string content = null) {
            if (ParseLogger.LogDelegate == null) {
                return;
            }

            ParseLogger.Debug(ParseHttpUtils.FormatResponse(response, content));
        }
    }
}
