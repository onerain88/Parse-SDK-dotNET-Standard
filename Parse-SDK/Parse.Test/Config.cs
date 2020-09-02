using NUnit.Framework;
using System;

namespace Parse.Test {
    [SetUpFixture]
    public class Config {
        [OneTimeSetUp]
        public void SetUp() {
            ParseClient.Initialize("hello", "http://localhost:1337/parse");
            ParseLogger.LogDelegate += Log;
        }

        [OneTimeTearDown]
        public void TearDown() {
            ParseLogger.LogDelegate -= Log;
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
