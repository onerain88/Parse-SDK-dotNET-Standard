using System.Collections.Generic;
using Parse.Internal.Codec;

namespace Parse.Internal.Query {
    public class ParseEqualCondition : IParseQueryCondition {
        private readonly string key;

        private readonly object value;

        public ParseEqualCondition(string key, object value) {
            this.key = key;
            this.value = value;
        }

        public bool Equals(IParseQueryCondition other) {
            if (other is ParseEqualCondition cond) {
                return cond.key == key;
            }
            return false;
        }

        public Dictionary<string, object> Encode() {
            return new Dictionary<string, object> {
                { key, ParseEncoder.Encode(value) }
            };
        }
    }
}
