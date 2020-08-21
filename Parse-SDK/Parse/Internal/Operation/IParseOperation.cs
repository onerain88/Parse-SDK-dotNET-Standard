using System.Collections;

namespace Parse.Internal.Operation {
    public interface IParseOperation {
        IParseOperation MergeWithPrevious(IParseOperation previousOp);

        object Encode();

        object Apply(object oldValue, string key);

        IEnumerable GetNewObjectList();
    }
}
