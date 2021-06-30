using System;

namespace ParseSDK {
    public class ParseRelation<T> where T : ParseObject {
        public string Key {
            get; set;
        }

        public ParseObject Parent {
            get; set;
        }

        public string TargetClass {
            get; set;
        }

        public ParseRelation() {
        }

        public ParseQuery<T> Query {
            get {
                ParseQuery<T> query = new ParseQuery<T>(TargetClass);
                query.WhereRelatedTo(Parent, Key);
                return query;
            }
        }
    }
}
