using System;

namespace Parse.Internal.Object {
    public class ParseSubClassInfo {
        public string ClassName {
            get;
        }

        public Type Type {
            get;
        }

        Func<ParseObject> Constructor {
            get;
        }

        public ParseSubClassInfo(string className, Type type, Func<ParseObject> constructor) {
            ClassName = className;
            Type = type;
            Constructor = constructor;
        }
    }
}
