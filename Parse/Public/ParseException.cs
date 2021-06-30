using System;

namespace ParseSDK {
    public class ParseException : Exception {
        public int Code {
            get;
        }

        public new string Message {
            get;
        }

        public ParseException(int code, string message) {
            Code = code;
            Message = message;
        }

        public override string ToString() {
            return $"{Code} - {Message}";
        }
    }
}
