using System.Collections;
using System.Collections.Generic;

namespace Parse.Internal.Operation {
    public class ParseDeleteOperation : IParseOperation {
        internal ParseDeleteOperation() {
        }

        public IParseOperation MergeWithPrevious(IParseOperation previousOp) {
            return this;
        }

        public object Encode() {
            return new Dictionary<string, object> {
                { "__op", "Delete" }
            };
        }

        public object Apply(object oldValue, string key) {
            return null;
        }

        public IEnumerable GetNewObjectList() {
            return null;
        }
    }
}
