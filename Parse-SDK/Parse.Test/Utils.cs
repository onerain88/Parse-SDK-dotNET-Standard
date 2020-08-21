using NUnit.Framework;
using System;
using Parse;

namespace Parse.Test {
    public static class Utils {
        internal static void Init() {
            ParseClient.Initialize("hello", "http://localhost:1337/parse");
        }

        internal static void Log(ParseLogLevel level, string info) {
            switch (level) {
                case ParseLogLevel.Debug:
                    TestContext.Out.WriteLine($"[DEBUG] {info}");
                    break;
                case ParseLogLevel.Warn:
                    TestContext.Out.WriteLine($"[WARNING] {info}");
                    break;
                case ParseLogLevel.Error:
                    TestContext.Out.WriteLine($"[ERROR] {info}");
                    break;
                default:
                    TestContext.Out.WriteLine(info);
                    break;
            }
        }
    }
}
