using System;

namespace ParseSDK.Internal.Object {
    public class ParseSubClassInfo {
        public string ClassName {
            get;
        }

        public Type Type {
            get;
        }

        internal Func<ParseObject> Constructor {
            get;
        }

        public ParseSubClassInfo(string className, Type type, Func<ParseObject> constructor) {
            ClassName = className;
            Type = type;
            Constructor = constructor;
        }
    }
}
