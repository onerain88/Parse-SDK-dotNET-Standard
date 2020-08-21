using System.Collections.Generic;
using Parse.Internal.Codec;

namespace Parse.Internal.Query {
    public class ParseRelatedCondition : IParseQueryCondition {
        readonly ParseObject parent;

        readonly string key;

        public ParseRelatedCondition(ParseObject parent, string key) {
            this.parent = parent;
            this.key = key;
        }

        public bool Equals(IParseQueryCondition other) {
            if (other is ParseRelatedCondition cond) {
                return cond.key == key;
            }
            return false;
        }

        public Dictionary<string, object> Encode() {
            return new Dictionary<string, object> {
                { "$relatedTo", new Dictionary<string, object> {
                    { "object", ParseEncoder.Encode(parent) },
                    { "key", key }
                } }
            };
        }
    }
}
