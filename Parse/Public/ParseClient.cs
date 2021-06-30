using System;
using ParseSDK.Internal.Http;

namespace ParseSDK {
    public class ParseClient {
        private static string appId;

        private static string server;

        public static ParseHttpClient HttpClient {
            get; private set;
        }

        public static void Initialize(string appId, string server) {
            if (string.IsNullOrEmpty(appId)) {
                throw new ArgumentNullException(nameof(appId));
            }
            if (string.IsNullOrEmpty(server)) {
                throw new ArgumentNullException(nameof(server));
            }

            ParseClient.appId = appId;
            ParseClient.server = server;

            ParseObject.RegisterSubclass(ParseUser.CLASS_NAME, () => new ParseUser());
            ParseObject.RegisterSubclass(ParseRole.CLASS_NAME, () => new ParseRole());

            HttpClient = new ParseHttpClient(appId, server);
        }
    }
}
