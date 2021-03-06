﻿using System.Collections.Generic;

namespace ParseSDK.Internal.Query {
    public interface IParseQueryCondition {
        bool Equals(IParseQueryCondition other);
        Dictionary<string, object> Encode();
    }
}
