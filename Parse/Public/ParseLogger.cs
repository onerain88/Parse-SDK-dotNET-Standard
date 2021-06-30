using System;
using System.Text;

namespace ParseSDK {
    public enum ParseLogLevel {
        Debug,
        Warn,
        Error
    }

    public static class ParseLogger {
        public static Action<ParseLogLevel, string> LogDelegate {
            get; set;
        }

        public static void Debug(string log) {
            LogDelegate?.Invoke(ParseLogLevel.Debug, log);
        }

        public static void Debug(string format, params object[] args) {
            LogDelegate?.Invoke(ParseLogLevel.Debug, string.Format(format, args));
        }

        public static void Warn(string log) {
            LogDelegate?.Invoke(ParseLogLevel.Warn, log);
        }

        public static void Warn(string format, params object[] args) {
            LogDelegate?.Invoke(ParseLogLevel.Warn, string.Format(format, args));
        }

        public static void Error(string log) {
            LogDelegate?.Invoke(ParseLogLevel.Error, log);
        }

        public static void Error(string format, params object[] args) {
            LogDelegate?.Invoke(ParseLogLevel.Error, string.Format(format, args));
        }

        public static void Error(Exception e) {
            StringBuilder sb = new StringBuilder();
            sb.Append(e.GetType());
            sb.Append("\n");
            sb.Append(e.Message);
            sb.Append("\n");
            sb.Append(e.StackTrace);
            Error(sb.ToString());
        }
    }
}
