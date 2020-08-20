using System;

namespace Parse {
    public class ParseClient {
        private static string appId;

        private static string server;

        public static void Initialize(string appId, string server) {
            ParseClient.appId = appId;
            ParseClient.server = server;
        }
    }
}
