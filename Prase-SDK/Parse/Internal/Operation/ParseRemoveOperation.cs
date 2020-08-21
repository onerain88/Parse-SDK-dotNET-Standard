using System;
using System.Collections;
using System.Collections.Generic;
using Parse.Internal.Codec;

namespace Parse.Internal.Operation {
    public class ParseRemoveOperation : IParseOperation {
        List<object> valueList;

        internal ParseRemoveOperation(IEnumerable<object> values) {
            valueList = new List<object>(values);
        }

        public IParseOperation MergeWithPrevious(IParseOperation previousOp) {
            if (previousOp is ParseSetOperation || previousOp is ParseDeleteOperation) {
                return previousOp;
            }
            if (previousOp is ParseRemoveOperation removeOp) {
                List<object> list = new List<object>(removeOp.valueList);
                list.AddRange(valueList);
                valueList = list;
                return this;
            }
            throw new ArgumentException("Operation is invalid after previous operation.");
        }

        public object Encode() {
            return new Dictionary<string, object> {
                { "__op", "Remove" },
                { "objects", ParseEncoder.Encode(valueList) }
            };
        }

        public object Apply(object oldValue, string key) {
            List<object> list = new List<object>();
            if (oldValue != null) {
                list.AddRange(oldValue as IEnumerable<object>);
            }
            list.RemoveAll(item => valueList.Contains(item));
            return list;
        }

        public IEnumerable GetNewObjectList() {
            return null;
        }
    }
}
