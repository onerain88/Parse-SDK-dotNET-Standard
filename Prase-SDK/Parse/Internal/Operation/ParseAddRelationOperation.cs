using System;
using System.Collections;
using System.Collections.Generic;
using Parse.Internal.Codec;

namespace Parse.Internal.Operation {
    public class ParseAddRelationOperation : IParseOperation {
        List<ParseObject> valueList;

        internal ParseAddRelationOperation(IEnumerable<ParseObject> objects) {
            valueList = new List<ParseObject>(objects);
        }

        public IParseOperation MergeWithPrevious(IParseOperation previousOp) {
            if (previousOp is ParseSetOperation || previousOp is ParseDeleteOperation) {
                return previousOp;
            }
            if (previousOp is ParseAddRelationOperation addRelationOp) {
                valueList.AddRange(addRelationOp.valueList);
                return this;
            }
            throw new ArgumentException("Operation is invalid after previous operation.");
        }

        public object Encode() {
            return new Dictionary<string, object> {
                { "__op", "AddRelation" },
                { "objects", ParseEncoder.Encode(valueList) }
            };
        }

        public object Apply(object oldValue, string key) {
            ParseRelation<ParseObject> relation = new ParseRelation<ParseObject>();
            relation.TargetClass = valueList[0].ClassName;
            return relation;
        }

        public IEnumerable GetNewObjectList() {
            return valueList;
        }
    }
}
