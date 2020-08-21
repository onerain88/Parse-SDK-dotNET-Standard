using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Parse.Internal.Query;
using Parse.Internal.Object;

namespace Parse {
    public class ParseQuery {
        public string ClassName {
            get; internal set;
        }

        public ParseCompositionCondition Condition {
            get; internal set;
        }

        public ParseQuery(string className) {
            ClassName = className;
            Condition = new ParseCompositionCondition();
        }

        internal Dictionary<string, object> BuildParams() {
            return Condition.BuildParams();
        }

        internal string BuildWhere() {
            return Condition.BuildWhere();
        }
    }

    public class ParseQuery<T> : ParseQuery where T : ParseObject {
        public ParseQuery(string className) :
            base(className) {

        }

        public ParseQuery<T> WhereEqualTo(string key, object value) {
            Condition.WhereEqualTo(key, value);
            return this;
        }

        public ParseQuery<T> WhereNotEqualTo(string key, object value) {
            Condition.WhereNotEqualTo(key, value);
            return this;
        }

        public ParseQuery<T> WhereContainedIn(string key, IEnumerable values) {
            Condition.WhereContainedIn(key, values);
            return this;
        }

        public ParseQuery<T> WhereNotContainedIn(string key, IEnumerable values) {
            Condition.WhereNotContainedIn(key, values);
            return this;
        }

        public ParseQuery<T> WhereContainsAll(string key, IEnumerable values) {
            Condition.WhereContainsAll(key, values);
            return this;
        }

        public ParseQuery<T> WhereExists(string key) {
            Condition.WhereExists(key);
            return this;
        }

        public ParseQuery<T> WhereDoesNotExist(string key) {
            Condition.WhereDoesNotExist(key);
            return this;
        }

        public ParseQuery<T> WhereSizeEqualTo(string key, int size) {
            Condition.WhereSizeEqualTo(key, size);
            return this;
        }

        public ParseQuery<T> WhereGreaterThan(string key, object value) {
            Condition.WhereGreaterThan(key, value);
            return this;
        }

        public ParseQuery<T> WhereGreaterThanOrEqualTo(string key, object value) {
            Condition.WhereGreaterThanOrEqualTo(key, value);
            return this;
        }

        public ParseQuery<T> WhereLessThan(string key, object value) {
            Condition.WhereLessThan(key, value);
            return this;
        }

        public ParseQuery<T> WhereLessThanOrEqualTo(string key, object value) {
            Condition.WhereLessThanOrEqualTo(key, value);
            return this;
        }

        public ParseQuery<T> WhereNear(string key, ParseGeoPoint point) {
            Condition.WhereNear(key, point);
            return this;
        }

        public ParseQuery<T> WhereWithinGeoBox(string key, ParseGeoPoint southwest, ParseGeoPoint northeast) {
            Condition.WhereWithinGeoBox(key, southwest, northeast);
            return this;
        }

        public ParseQuery<T> WhereRelatedTo(ParseObject parent, string key) {
            Condition.WhereRelatedTo(parent, key);
            return this;
        }

        public ParseQuery<T> WhereStartsWith(string key, string prefix) {
            Condition.WhereStartsWith(key, prefix);
            return this;
        }

        public ParseQuery<T> WhereEndsWith(string key, string suffix) {
            Condition.WhereEndsWith(key, suffix);
            return this;
        }

        public ParseQuery<T> WhereContains(string key, string subString) {
            Condition.WhereContains(key, subString);
            return this;
        }

        public ParseQuery<T> WhereMatches(string key, string regex, string modifiers = null) {
            Condition.WhereMatches(key, regex, modifiers);
            return this;
        }

        public ParseQuery<T> WhereMatchesQuery<K>(string key, ParseQuery<K> query) where K : ParseObject {
            Condition.WhereMatchesQuery(key, query);
            return this;
        }

        public ParseQuery<T> WhereDoesNotMatchQuery<K>(string key, ParseQuery<K> query) where K : ParseObject {
            Condition.WhereDoesNotMatchQuery(key, query);
            return this;
        }


        public ParseQuery<T> OrderByAscending(string key) {
            Condition.OrderByAscending(key);
            return this;
        }


        public ParseQuery<T> OrderByDescending(string key) {
            Condition.OrderByDescending(key);
            return this;
        }

        public ParseQuery<T> AddAscendingOrder(string key) {
            Condition.AddAscendingOrder(key);
            return this;
        }

        public ParseQuery<T> AddDescendingOrder(string key) {
            Condition.AddDescendingOrder(key);
            return this;
        }

        public ParseQuery<T> Include(string key) {
            Condition.Include(key);
            return this;
        }

        public ParseQuery<T> Select(string key) {
            Condition.Select(key);
            return this;
        }

        public bool IncludeACL {
            get {
                return Condition.IncludeACL;
            }
            set {
                Condition.IncludeACL = value;
            }
        }

        public ParseQuery<T> Skip(int value) {
            Condition.Skip = value;
            return this;
        }

        public ParseQuery<T> Limit(int value) {
            Condition.Limit = value;
            return this;
        }

        public async Task<int> Count() {
            string path = $"classes/{ClassName}";
            Dictionary<string, object> parameters = BuildParams();
            parameters["limit"] = 0;
            parameters["count"] = 1;
            Dictionary<string, object> ret = await ParseClient.HttpClient.Get<Dictionary<string, object>>(path, queryParams: parameters);
            return (int)ret["count"];
        }

        public async Task<T> Get(string objectId) {
            if (string.IsNullOrEmpty(objectId)) {
                throw new ArgumentNullException(nameof(objectId));
            }
            WhereEqualTo("objectId", objectId);
            Limit(1);
            ReadOnlyCollection<T> results = await Find();
            if (results != null) {
                if (results.Count == 0) {
                    return null;
                }
                return results[0];
            }
            return null;
        }

        public async Task<ReadOnlyCollection<T>> Find() {
            string path = $"classes/{ClassName}";
            Dictionary<string, object> parameters = BuildParams();
            Dictionary<string, object> response = await ParseClient.HttpClient.Get<Dictionary<string, object>>(path, queryParams: parameters);
            List<object> results = response["results"] as List<object>;
            List<T> list = new List<T>();
            foreach (object item in results) {
                ParseObjectData objectData = ParseObjectData.Decode(item as Dictionary<string, object>);
                T obj = ParseObject.Create(ClassName) as T;
                obj.Merge(objectData);
                list.Add(obj);
            }
            return list.AsReadOnly();
        }

        public async Task<T> First() {
            Limit(1);
            ReadOnlyCollection<T> results = await Find();
            if (results != null && results.Count > 0) {
                return results[0];
            }
            return null;
        }

        public static ParseQuery<T> And(IEnumerable<ParseQuery<T>> queries) {
            if (queries == null || queries.Count() < 1) {
                throw new ArgumentNullException(nameof(queries));
            }
            ParseQuery<T> compositionQuery = new ParseQuery<T>(null);
            string className = null;
            foreach (ParseQuery<T> query in queries) {
                if (className != null && className != query.ClassName) {
                    throw new Exception("All of the queries in an or query must be on the same class.");
                }
                className = query.ClassName;
                compositionQuery.Condition.Add(query.Condition);
            }
            compositionQuery.ClassName = className;
            return compositionQuery;
        }

        public static ParseQuery<T> Or(IEnumerable<ParseQuery<T>> queries) {
            if (queries == null || queries.Count() < 1) {
                throw new ArgumentNullException(nameof(queries));
            }
            ParseQuery<T> compositionQuery = new ParseQuery<T>(null);
            compositionQuery.Condition = new ParseCompositionCondition(ParseCompositionCondition.Or);
            string className = null;
            foreach (ParseQuery<T> query in queries) {
                if (className != null && className != query.ClassName) {
                    throw new Exception("All of the queries in an or query must be on the same class.");
                }
                className = query.ClassName;
                compositionQuery.Condition.Add(query.Condition);
            }
            compositionQuery.ClassName = className;
            return compositionQuery;
        }
    }
}
