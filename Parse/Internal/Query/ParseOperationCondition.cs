using System.Collections.Generic;
using ParseSDK.Internal.Codec;

namespace ParseSDK.Internal.Query {
    public class ParseOperationCondition : IParseQueryCondition {
        readonly string key;

        readonly string op;

        readonly object value;

        public ParseOperationCondition(string key, string op, object value) {
            this.key = key;
            this.op = op;
            this.value = value;
        }

        public bool Equals(IParseQueryCondition other) {
            if (other is ParseOperationCondition cond) {
                return cond.key == key && cond.op == op;
            }
            return false;
        }

        public Dictionary<string, object> Encode() {
            return new Dictionary<string, object> {
                { key, new Dictionary<string, object> {
                    { op, ParseEncoder.Encode(value) }
                } }
            };
        }
    }
}
