using System;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Parse.Internal.Http {
    public static class ParseHttpUtils {
        public static string FormatRequest(HttpClient client, HttpRequestMessage request, string content = null) {
            if (client == null) {
                return null;
            }
            if (request == null) {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== HTTP Request Start ===");
            sb.AppendLine($"URL: {request.RequestUri}");
            sb.AppendLine($"Method: {request.Method}");
            sb.AppendLine($"Headers: ");
            foreach (var header in client.DefaultRequestHeaders) {
                sb.AppendLine($"\t{header.Key}: {string.Join(",", header.Value.ToArray())}");
            }
            foreach (var header in request.Headers) {
                sb.AppendLine($"\t{header.Key}: {string.Join(",", header.Value.ToArray())}");
            }
            if (request.Content != null) {
                foreach (var header in request.Content.Headers) {
                    sb.AppendLine($"\t{header.Key}: {string.Join(",", header.Value.ToArray())}");
                }
            }
            if (!string.IsNullOrEmpty(content)) {
                sb.AppendLine($"Content: {content}");
            }
            sb.AppendLine("=== HTTP Request End ===");
            return sb.ToString();
        }

        public static string FormatResponse(HttpResponseMessage response, string content = null) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== HTTP Response Start ===");
            sb.AppendLine($"URL: {response.RequestMessage.RequestUri}");
            sb.AppendLine($"Status Code: {response.StatusCode}");
            if (!string.IsNullOrEmpty(content)) {
                sb.AppendLine($"Content: {content}");
            }
            sb.AppendLine("=== HTTP Response End ===");
            return sb.ToString();
        }
    }
}
