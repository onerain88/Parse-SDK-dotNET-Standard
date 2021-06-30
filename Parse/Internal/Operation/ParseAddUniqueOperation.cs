using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ParseSDK.Internal.Codec;

namespace ParseSDK.Internal.Operation {
    public class ParseAddUniqueOperation : IParseOperation {
        internal HashSet<object> values;

        internal ParseAddUniqueOperation(IEnumerable<object> values) {
            this.values = new HashSet<object>(values);
        }

        public IParseOperation MergeWithPrevious(IParseOperation previousOp) {
            if (previousOp is ParseSetOperation || previousOp is ParseDeleteOperation) {
                return previousOp;
            }
            if (previousOp is ParseAddUniqueOperation addUniqueOp) {
                values.UnionWith(addUniqueOp.values);
                return this;
            }
            throw new ArgumentException("Operation is invalid after previous operation.");
        }

        public object Encode() {
            return new Dictionary<string, object> {
                { "__op", "AddUnique" },
                { "objects", ParseEncoder.Encode(values.ToList()) }
            };
        }

        public object Apply(object oldValue, string key) {
            HashSet<object> set = new HashSet<object>();
            if (oldValue != null) {
                set.UnionWith(oldValue as IEnumerable<object>);
            }
            set.UnionWith(values);
            return set.ToList();
        }

        public IEnumerable GetNewObjectList() {
            return values;
        }
    }
}
