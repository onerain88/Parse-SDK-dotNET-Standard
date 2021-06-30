using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ParseSDK.Internal.Object;
using ParseSDK.Internal.Operation;
using ParseSDK.Internal.Codec;

namespace ParseSDK {
    public class ParseObject {
        ParseObjectData data;

        internal Dictionary<string, object> estimatedData;

        internal Dictionary<string, IParseOperation> operationDict;

        static readonly Dictionary<Type, ParseSubClassInfo> subclassTypeDict = new Dictionary<Type, ParseSubClassInfo>();
        static readonly Dictionary<string, ParseSubClassInfo> subclassNameDict = new Dictionary<string, ParseSubClassInfo>();

        public string ClassName {
            get {
                return data.ClassName;
            }
        }

        public string ObjectId {
            get {
                return data.ObjectId;
            }
        }

        public DateTime CreatedAt {
            get {
                return data.CreatedAt;
            }
        }

        public DateTime UpdatedAt {
            get {
                return data.UpdatedAt;
            }
        }

        public ParseACL ACL {
            get {
                return this["ACL"] as ParseACL;
            }
            set {
                this["ACL"] = value;
            }
        }

        bool isNew;

        bool IsDirty {
            get {
                return isNew || estimatedData.Count > 0;
            }
        }

        public ParseObject(string className) {
            if (string.IsNullOrEmpty(className)) {
                throw new ArgumentNullException(nameof(className));
            }
            data = new ParseObjectData();
            estimatedData = new Dictionary<string, object>();
            operationDict = new Dictionary<string, IParseOperation>();

            data.ClassName = className;
            isNew = true;
        }

        public static ParseObject CreateWithoutData(string className, string objectId) {
            if (string.IsNullOrEmpty(objectId)) {
                throw new ArgumentNullException(nameof(objectId));
            }
            ParseObject obj = Create(className);
            obj.data.ObjectId = objectId;
            obj.isNew = false;
            return obj;
        }

        public static ParseObject Create(string className) {
            if (subclassNameDict.TryGetValue(className, out ParseSubClassInfo subclassInfo)) {
                return subclassInfo.Constructor.Invoke();
            }
            return new ParseObject(className);
        }

        internal static ParseObject Create(Type type) {
            if (subclassTypeDict.TryGetValue(type, out ParseSubClassInfo subclassInfo)) {
                return subclassInfo.Constructor.Invoke();
            }
            return null;
        }

        public object this[string key] {
            get {
                if (estimatedData.TryGetValue(key, out object value)) {
                    if (value is ParseRelation<ParseObject> relation) {
                        relation.Key = key;
                        relation.Parent = this;
                    }
                    return value;
                }
                return null;
            }
            set {
                if (string.IsNullOrEmpty(key)) {
                    throw new ArgumentNullException(nameof(key));
                }
                if (key.StartsWith("_")) {
                    throw new ArgumentException("key should not start with '_'");
                }
                if (key == "objectId" || key == "createdAt" || key == "updatedAt") {
                    throw new ArgumentException($"{key} is reserved by LeanCloud");
                }
                ParseSetOperation setOp = new ParseSetOperation(value);
                ApplyOperation(key, setOp);
            }
        }

        public void Unset(string key) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            ParseDeleteOperation deleteOp = new ParseDeleteOperation();
            ApplyOperation(key, deleteOp);
        }


        public void AddRelation(string key, ParseObject value) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            ParseAddRelationOperation op = new ParseAddRelationOperation(new List<ParseObject> { value });
            ApplyOperation(key, op);
        }

        public void RemoveRelation(string key, ParseObject value) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            ParseRemoveRelationOperation op = new ParseRemoveRelationOperation(value);
            ApplyOperation(key, op);
        }

        public void Increment(string key, object value) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            ParseNumberOperation op = new ParseNumberOperation(value);
            ApplyOperation(key, op);
        }

        public void Add(string key, object value) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            ParseAddOperation op = new ParseAddOperation(new List<object> { value });
            ApplyOperation(key, op);
        }

        public void AddAll(string key, IEnumerable values) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (values == null) {
                throw new ArgumentNullException(nameof(values));
            }
            ParseAddOperation op = new ParseAddOperation(new List<object>(values.Cast<object>()));
            ApplyOperation(key, op);
        }

        public void AddUnique(string key, object value) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            ParseAddUniqueOperation op = new ParseAddUniqueOperation(new List<object> { value });
            ApplyOperation(key, op);
        }

        public void AddAllUnique(string key, IEnumerable values) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (values == null) {
                throw new ArgumentNullException(nameof(values));
            }
            ParseAddUniqueOperation op = new ParseAddUniqueOperation(new List<object>(values.Cast<object>()));
            ApplyOperation(key, op);
        }

        public void Remove(string key, object value) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            ParseRemoveOperation op = new ParseRemoveOperation(new List<object> { value });
            ApplyOperation(key, op);
        }

        public void RemoveAll(string key, IEnumerable values) {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key));
            }
            if (values == null) {
                throw new ArgumentNullException(nameof(values));
            }
            ParseRemoveOperation op = new ParseRemoveOperation(new List<object>(values.Cast<object>()));
            ApplyOperation(key, op);
        }

        static async Task SaveBatches(Stack<ParseBatch> batches) {
            while (batches.Count > 0) {
                ParseBatch batch = batches.Pop();
                List<ParseObject> dirtyObjects = batch.objects.Where(item => item.IsDirty)
            .ToList();

                List<Dictionary<string, object>> requestList = dirtyObjects.Select(item => {
                    string path = item.ObjectId == null ?
                                $"/parse/classes/{item.ClassName}" :
                                $"/parse/classes/{item.ClassName}/{item.ClassName}";
                    string method = item.ObjectId == null ? "POST" : "PUT";
                    Dictionary<string, object> body = ParseEncoder.Encode(item.operationDict) as Dictionary<string, object>;
                    return new Dictionary<string, object> {
                        { "path", path },
                        { "method", method },
                        { "body", body }
                    };
                }).ToList();

                Dictionary<string, object> data = new Dictionary<string, object> {
                    { "requests", ParseEncoder.Encode(requestList) }
                };

                List<Dictionary<string, object>> results = await ParseClient.HttpClient.Post<List<Dictionary<string, object>>>("batch", data: data);
                List<ParseObjectData> resultList = results.Select(item => {
                    if (item.TryGetValue("error", out object error)) {
                        Dictionary<string, object> err = error as Dictionary<string, object>;
                        int code = (int)err["code"];
                        string message = (string)err["error"];
                        throw new ParseException(code, message as string);
                    }
                    return ParseObjectData.Decode(item["success"] as IDictionary);
                }).ToList();

                for (int i = 0; i < dirtyObjects.Count; i++) {
                    ParseObject obj = dirtyObjects[i];
                    ParseObjectData objData = resultList[i];
                    obj.Merge(objData);
                }
            }
        }

        public async Task<ParseObject> Save(bool fetchWhenSave = false, ParseQuery<ParseObject> query = null) {
            if (ParseBatch.HasCircleReference(this, new HashSet<ParseObject>())) {
                throw new ArgumentException("Found a circle dependency when save.");
            }

            Stack<ParseBatch> batches = ParseBatch.BatchObjects(new List<ParseObject> { this }, false);
            if (batches.Count > 0) {
                await SaveBatches(batches);
            }

            string path = ObjectId == null ? $"classes/{ClassName}" : $"classes/{ClassName}/{ObjectId}";
            Dictionary<string, object> queryParams = new Dictionary<string, object>();
            if (fetchWhenSave) {
                queryParams["fetchWhenSave"] = true;
            }
            if (query != null) {
                queryParams["where"] = query.BuildWhere();
            }
            Dictionary<string, object> response = ObjectId == null ?
                await ParseClient.HttpClient.Post<Dictionary<string, object>>(path, data: ParseEncoder.Encode(operationDict) as Dictionary<string, object>, queryParams: queryParams) :
                await ParseClient.HttpClient.Put<Dictionary<string, object>>(path, data: ParseEncoder.Encode(operationDict) as Dictionary<string, object>, queryParams: queryParams);
            ParseObjectData data = ParseObjectData.Decode(response);
            Merge(data);
            return this;
        }

        public static async Task<List<ParseObject>> SaveAll(List<ParseObject> objectList) {
            if (objectList == null) {
                throw new ArgumentNullException(nameof(objectList));
            }
            foreach (ParseObject obj in objectList) {
                if (ParseBatch.HasCircleReference(obj, new HashSet<ParseObject>())) {
                    throw new ArgumentException("Found a circle dependency when save.");
                }
            }
            Stack<ParseBatch> batches = ParseBatch.BatchObjects(objectList, true);
            await SaveBatches(batches);
            return objectList;
        }

        public async Task Delete() {
            if (ObjectId == null) {
                return;
            }
            string path = $"classes/{ClassName}/{ObjectId}";
            await ParseClient.HttpClient.Delete(path);
        }

        public static async Task DeleteAll(List<ParseObject> objectList) {
            if (objectList == null || objectList.Count == 0) {
                throw new ArgumentNullException(nameof(objectList));
            }
            IEnumerable<ParseObject> objects = objectList.Where(item => item.ObjectId != null);
            HashSet<ParseObject> objectSet = new HashSet<ParseObject>(objects);
            List<Dictionary<string, object>> requestList = objectSet.Select(item => {
                string path = $"/parse/classes/{item.ClassName}/{item.ObjectId}";
                return new Dictionary<string, object> {
                    { "path", path },
                    { "method", "DELETE" }
                };
            }).ToList();
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "requests", ParseEncoder.Encode(requestList) }
            };
            await ParseClient.HttpClient.Post<List<object>>("batch", data: data);
        }

        public async Task<ParseObject> Fetch(IEnumerable<string> keys = null, IEnumerable<string> includes = null) {
            Dictionary<string, object> queryParams = new Dictionary<string, object>();
            if (keys != null) {
                queryParams["keys"] = string.Join(",", keys);
            }
            if (includes != null) {
                queryParams["include"] = string.Join(",", includes);
            }
            string path = $"classes/{ClassName}/{ObjectId}";
            Dictionary<string, object> response = await ParseClient.HttpClient.Get<Dictionary<string, object>>(path, queryParams: queryParams);
            ParseObjectData objectData = ParseObjectData.Decode(response);
            Merge(objectData);
            return this;
        }

        public static async Task<IEnumerable<ParseObject>> FetchAll(IEnumerable<ParseObject> objects) {
            if (objects == null || objects.Count() == 0) {
                throw new ArgumentNullException(nameof(objects));
            }

            IEnumerable<ParseObject> uniqueObjects = objects.Where(item => item.ObjectId != null);
            List<Dictionary<string, object>> requestList = uniqueObjects.Select(item => {
                string path = $"/parse/classes/{item.ClassName}/{item.ObjectId}";
                return new Dictionary<string, object> {
                    { "path", path },
                    { "method", "GET" }
                };
            }).ToList();

            Dictionary<string, object> data = new Dictionary<string, object> {
                { "requests", ParseEncoder.Encode(requestList) }
            };
            List<Dictionary<string, object>> results = await ParseClient.HttpClient.Post<List<Dictionary<string, object>>>("batch",
                data: data);
            Dictionary<string, ParseObjectData> dict = new Dictionary<string, ParseObjectData>();
            foreach (Dictionary<string, object> item in results) {
                if (item.TryGetValue("error", out object error)) {
                    int code = (int)error;
                    string message = item["error"] as string;
                    throw new ParseException(code, message);
                }
                Dictionary<string, object> d = item["success"] as Dictionary<string, object>;
                string objectId = d["objectId"] as string;
                dict[objectId] = ParseObjectData.Decode(d);
            }
            foreach (ParseObject obj in objects) {
                ParseObjectData objData = dict[obj.ObjectId];
                obj.Merge(objData);
            }
            return objects;
        }

        public static void RegisterSubclass<T>(string className, Func<T> constructor) where T : ParseObject {
            Type classType = typeof(T);
            ParseSubClassInfo subclassInfo = new ParseSubClassInfo(className, classType, constructor);
            subclassNameDict[className] = subclassInfo;
            subclassTypeDict[classType] = subclassInfo;
        }

                                        public override string ToString() {
            Dictionary<string, object> originalData = ParseObjectData.Encode(data);
            Dictionary<string, object> currentData = estimatedData.Union(originalData.Where(kv => !estimatedData.ContainsKey(kv.Key)))
                .ToDictionary(k => k.Key, v => v.Value);
            return JsonConvert.SerializeObject(currentData);
        }

        public static ParseObject Parse(string json) {
            ParseObjectData objectData = ParseObjectData.Decode(JsonConvert.DeserializeObject<Dictionary<string, object>>(json));
            ParseObject obj = Create(objectData.ClassName);
            obj.Merge(objectData);
            return obj;
        }

        void ApplyOperation(string key, IParseOperation op) {
            if (op is ParseDeleteOperation) {
                estimatedData.Remove(key);
            } else {
                if (estimatedData.TryGetValue(key, out object oldValue)) {
                    estimatedData[key] = op.Apply(oldValue, key);
                } else {
                    estimatedData[key] = op.Apply(null, key);
                }
            }
            if (operationDict.TryGetValue(key, out IParseOperation previousOp)) {
                operationDict[key] = op.MergeWithPrevious(previousOp);
            } else {
                operationDict[key] = op;
            }
        }

        public void Merge(ParseObjectData objectData) {
            data.ClassName = objectData.ClassName ?? data.ClassName;
            data.ObjectId = objectData.ObjectId ?? data.ObjectId;
            data.CreatedAt = objectData.CreatedAt != null ? objectData.CreatedAt : data.CreatedAt;
            data.UpdatedAt = objectData.UpdatedAt != null ? objectData.UpdatedAt : data.UpdatedAt;
            // 先将本地的预估数据直接替换
            data.CustomPropertyDict = estimatedData;
            // 再将服务端的数据覆盖
            foreach (KeyValuePair<string, object> kv in objectData.CustomPropertyDict) {
                string key = kv.Key;
                object value = kv.Value;
                data.CustomPropertyDict[key] = value;
            }

            // 最后重新生成预估数据，用于后续访问和操作
            RebuildEstimatedData();
            // 清空操作
            operationDict.Clear();
            isNew = false;
        }

        void RebuildEstimatedData() {
            estimatedData = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kv in data.CustomPropertyDict) {
                string key = kv.Key;
                object value = kv.Value;
                if (value is IList list) {
                    estimatedData[key] = new List<object>(list.Cast<object>());
                } else if (value is IDictionary dict) {
                    Dictionary<string, object> d = new Dictionary<string, object>();
                    foreach (DictionaryEntry entry in dict) {
                        string k = entry.Key.ToString();
                        object v = entry.Value;
                        d[k] = v;
                    }
                    estimatedData[key] = d;
                } else {
                    estimatedData[key] = value;
                }
            }
        }
    }
}
