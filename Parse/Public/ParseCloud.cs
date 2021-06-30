using System.Threading.Tasks;
using System.Collections.Generic;
using ParseSDK.Internal.Codec;

namespace ParseSDK {
    public static class ParseCloud {
        public static async Task<Dictionary<string, object>> Run(string name,
            Dictionary<string, object> parameters = null) {
            string path = $"functions/{name}";
            object encodeParams = ParseEncoder.Encode(parameters);
            Dictionary<string, object> response = await ParseClient.HttpClient.Post<Dictionary<string, object>>(path,
                data: encodeParams);
            return response;
        }

        public static async Task<object> RPC(string name, object parameters = null) {
            string path = $"call/{name}";
            object encodeParams = ParseEncoder.Encode(parameters);
            Dictionary<string, object> response = await ParseClient.HttpClient.Post<Dictionary<string, object>>(path,
                data: encodeParams);
            return ParseDecoder.Decode(response["result"]);
        }
    }
}
