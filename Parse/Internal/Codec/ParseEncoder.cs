using System;
using System.Collections;
using System.Collections.Generic;
using Parse.Internal.Operation;
using Parse.Internal.Query;

namespace Parse.Internal.Codec {
    public static class ParseEncoder {
        public static object Encode(object obj) {
            if (obj is DateTime dateTime) {
                return EncodeDateTime(dateTime);
            } else if (obj is byte[] bytes) {
                return EncodeBytes(bytes);
            } else if (obj is IList list) {
                return EncodeList(list);
            } else if (obj is IDictionary dict) {
                return EncodeDictionary(dict);
            } else if (obj is ParseObject ParseObj) {
                return EncodeParseObject(ParseObj);
            } else if (obj is IParseOperation op) {
                return EncodeOperation(op);
            } else if (obj is IParseQueryCondition cond) {
                return EncodeQueryCondition(cond);
            } else if (obj is ParseACL acl) {
                return EncodeACL(acl);
            } else if (obj is ParseRelation<ParseObject> relation) {
                return EncodeRelation(relation);
            } else if (obj is ParseGeoPoint geoPoint) {
                return EncodeGeoPoint(geoPoint);
            }
            return obj;
        }

        public static object EncodeDateTime(DateTime dateTime) {
            DateTime utc = dateTime.ToUniversalTime();
            string str = utc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return new Dictionary<string, object> {
                { "__type", "Date" },
                { "iso", str }
            };
        }

        public static object EncodeBytes(byte[] bytes) {
            string str = Convert.ToBase64String(bytes);
            return new Dictionary<string, object> {
                { "__type", "Bytes" },
                { "base64", str }
            };
        }

        public static object EncodeList(IList list) {
            List<object> l = new List<object>();
            foreach (object obj in list) {
                l.Add(Encode(obj));
            }
            return l;
        }

        public static object EncodeDictionary(IDictionary dict) {
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in dict) {
                string key = entry.Key.ToString();
                object value = entry.Value;
                d[key] = Encode(value);
            }
            return d;
        }

        public static object EncodeParseObject(ParseObject obj) {
            return new Dictionary<string, object> {
                { "__type", "Pointer" },
                { "className", obj.ClassName },
                { "objectId", obj.ObjectId }
            };
        }

        static object EncodeOperation(IParseOperation operation) {
            return operation.Encode();
        }

        public static object EncodeQueryCondition(IParseQueryCondition cond) {
            return cond.Encode();
        }

        public static object EncodeACL(ParseACL acl) {
            HashSet<string> keys = new HashSet<string>();
            if (acl.readAccess.Count > 0) {
                keys.UnionWith(acl.readAccess.Keys);
            }
            if (acl.writeAccess.Count > 0) {
                keys.UnionWith(acl.writeAccess.Keys);
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in keys) {
                Dictionary<string, bool> access = new Dictionary<string, bool>();
                if (acl.readAccess.TryGetValue(key, out bool ra)) {
                    access["read"] = ra;
                }
                if (acl.writeAccess.TryGetValue(key, out bool wa)) {
                    access["write"] = wa;
                }
                result[key] = access;
            }
            return result;
        }

        public static object EncodeRelation(ParseRelation<ParseObject> relation) {
            return new Dictionary<string, object> {
                { "__type", "Relation" },
                { "className", relation.TargetClass }
            };
        }

        public static object EncodeGeoPoint(ParseGeoPoint geoPoint) {
            return new Dictionary<string, object> {
                { "__type", "GeoPoint" },
                { "latitude", geoPoint.Latitude },
                { "longitude", geoPoint.Longitude }
            };
        }
    }
}
