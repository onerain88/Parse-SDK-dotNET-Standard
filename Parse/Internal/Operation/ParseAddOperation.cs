using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ParseSDK.Internal.Codec;

namespace ParseSDK.Internal.Operation {
    public class ParseAddOperation : IParseOperation {
        internal List<object> valueList;

        internal ParseAddOperation(IEnumerable<object> values) {
            valueList = new List<object>(values);
        }

        IParseOperation IParseOperation.MergeWithPrevious(IParseOperation previousOp) {
            if (previousOp is ParseSetOperation || previousOp is ParseDeleteOperation) {
                return previousOp;
            }
            if (previousOp is ParseAddOperation addOp) {
                List<object> list = new List<object>(addOp.valueList);
                list.AddRange(valueList);
                valueList = list;
                return this;
            }
            if (previousOp is ParseAddUniqueOperation addUniqueOp) {
                List<object> list = addUniqueOp.values.ToList();
                list.AddRange(valueList);
                valueList = list;
                return this;
            }
            throw new ArgumentException("Operation is invalid after previous operation.");
        }

        object IParseOperation.Encode() {
            return new Dictionary<string, object> {
                { "__op", "Add" },
                { "objects", ParseEncoder.Encode(valueList) }
            };
        }

        object IParseOperation.Apply(object oldValue, string key) {
            List<object> list = new List<object>();
            if (oldValue != null) {
                list.AddRange(oldValue as IEnumerable<object>);
            }
            list.AddRange(valueList);
            return list;
        }

        IEnumerable IParseOperation.GetNewObjectList() {
            return valueList;
        }
    }
}
