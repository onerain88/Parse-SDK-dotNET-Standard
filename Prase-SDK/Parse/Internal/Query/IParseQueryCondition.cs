using System.Collections.Generic;

namespace Parse.Internal.Query {
    public interface IParseQueryCondition {
        bool Equals(IParseQueryCondition other);
        Dictionary<string, object> Encode();
    }
}
