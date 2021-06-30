using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace ParseSDK.Internal.Object {
    public class ParseBatch {
        internal HashSet<ParseObject> objects;

        internal ParseBatch(IEnumerable<ParseObject> objs) {
            if (objs == null) {
                objects = new HashSet<ParseObject>();
            } else {
                objects = new HashSet<ParseObject>(objs);
            }
        }

        internal static bool HasCircleReference(object obj, HashSet<ParseObject> parents) {
            if (obj is ParseObject parseObj && parents.Contains(parseObj)) {
                return true;
            }
            IEnumerable deps = null;
            if (obj is IList list) {
                deps = list;
            } else if (obj is IDictionary dict) {
                deps = dict.Values;
            } else if (obj is ParseObject lcObject) {
                deps = lcObject.estimatedData.Values;
            }
            HashSet<ParseObject> depParents = new HashSet<ParseObject>(parents);
            if (obj is ParseObject) {
                depParents.Add(obj as ParseObject);
            }
            if (deps != null) {
                foreach (object dep in deps) {
                    HashSet<ParseObject> ps = new HashSet<ParseObject>(depParents);
                    if (HasCircleReference(dep, ps)) {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static Stack<ParseBatch> BatchObjects(IEnumerable<ParseObject> objects, bool containSelf) {
            Stack<ParseBatch> batches = new Stack<ParseBatch>();
            if (containSelf) {
                batches.Push(new ParseBatch(objects));
            }
            HashSet<object> deps = new HashSet<object>();
            foreach (ParseObject obj in objects) {
                deps.UnionWith(obj.operationDict.Values.Select(op => op.GetNewObjectList()));
            }
            do {
                HashSet<object> childSet = new HashSet<object>();
                foreach (object dep in deps) {
                    IEnumerable children = null;
                    if (dep is IList list) {
                        children = list;
                    } else if (dep is IDictionary dict) {
                        children = dict;
                    } else if (dep is ParseObject lcDep && lcDep.ObjectId == null) {
                        children = lcDep.operationDict.Values.Select(op => op.GetNewObjectList());
                    }
                    if (children != null) {
                        childSet.UnionWith(children.Cast<object>());
                    }
                }
                IEnumerable<ParseObject> depObjs = deps.Where(item => item is ParseObject lcItem && lcItem.ObjectId == null)
                    .Cast<ParseObject>();
                if (depObjs != null && depObjs.Count() > 0) {
                    batches.Push(new ParseBatch(depObjs));
                }
                deps = childSet;
            } while (deps != null && deps.Count > 0);
            return batches;
        }
    }
}
