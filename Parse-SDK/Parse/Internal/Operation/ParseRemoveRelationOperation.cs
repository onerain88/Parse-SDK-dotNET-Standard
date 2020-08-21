using System;
using System.Collections;
using System.Collections.Generic;
using Parse.Internal.Codec;

namespace Parse.Internal.Operation {
    public class ParseRemoveRelationOperation : IParseOperation {
        List<ParseObject> valueList;

        internal ParseRemoveRelationOperation(ParseObject obj) {
            valueList = new List<ParseObject> { obj };
        }

        public IParseOperation MergeWithPrevious(IParseOperation previousOp) {
            if (previousOp is ParseSetOperation || previousOp is ParseDeleteOperation) {
                return previousOp;
            }
            if (previousOp is ParseRemoveRelationOperation removeRelationOp) {
                valueList.AddRange(removeRelationOp.valueList);
                return this;
            }
            throw new ArgumentException("Operation is invalid after previous operation.");
        }

        public object Encode() {
            return new Dictionary<string, object> {
                { "__op", "RemoveRelation" },
                { "objects", ParseEncoder.Encode(valueList) }
            };
        }

        public object Apply(object oldValue, string key) {
            ParseRelation<ParseObject> relation = new ParseRelation<ParseObject>();
            relation.TargetClass = valueList[0].ClassName;
            return relation;
        }

        public IEnumerable GetNewObjectList() {
            return null;
        }
    }
}
