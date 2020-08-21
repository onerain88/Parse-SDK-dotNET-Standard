using System.Collections;
using System.Collections.Generic;
using Parse.Internal.Codec;

namespace Parse.Internal.Operation {
    public class ParseSetOperation : IParseOperation {
        object value;

        internal ParseSetOperation(object value) {
            this.value = value;
        }

        public IParseOperation MergeWithPrevious(IParseOperation previousOp) {
            return this;
        }

        public object Encode() {
            return ParseEncoder.Encode(value);
        }

        public object Apply(object oldValue, string key) {
            return value;
        }

        public IEnumerable GetNewObjectList() {
            if (value is IEnumerable enumerable) {
                return enumerable;
            }
            return new List<object> { value };
        }
    }
}
