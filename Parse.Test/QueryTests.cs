using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParseSDK.Test {
    public class QueryTests {
        [Test]
        public async Task BaseQuery() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Hello");
            query.Limit(2);
            ReadOnlyCollection<ParseObject> list = await query.Find();
            TestContext.WriteLine(list.Count);
            Assert.AreEqual(list.Count, 2);

            foreach (ParseObject item in list) {
                Assert.NotNull(item.ClassName);
                Assert.NotNull(item.ObjectId);
                Assert.NotNull(item.CreatedAt);
                Assert.NotNull(item.UpdatedAt);

                TestContext.WriteLine(item.ClassName);
                TestContext.WriteLine(item.ObjectId);
                TestContext.WriteLine(item.CreatedAt);
                TestContext.WriteLine(item.UpdatedAt);
                TestContext.WriteLine(item["intValue"]);
                TestContext.WriteLine(item["boolValue"]);
                TestContext.WriteLine(item["stringValue"]);
            }
        }

        [Test]
        public async Task Count() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            query.WhereGreaterThan("balance", 200);
            int count = await query.Count();
            TestContext.WriteLine(count);
            Assert.Greater(count, 0);
        }

        [Test]
        public async Task OrderBy() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            query.OrderByAscending("balance");
            ReadOnlyCollection<ParseObject> results = await query.Find();
            Assert.LessOrEqual((int)results[0]["balance"], (int)results[1]["balance"]);

            query = new ParseQuery<ParseObject>("Account");
            query.OrderByDescending("balance");
            results = await query.Find();
            Assert.GreaterOrEqual((int)results[0]["balance"], (int)results[1]["balance"]);
        }

        [Test]
        public async Task AddOrder() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            query.AddAscendingOrder("balance");
            query.AddDescendingOrder("createdAt");
            ReadOnlyCollection<ParseObject> results = await query.Find();
            for (int i = 0; i + 1 < results.Count; i++) {
                ParseObject a1 = results[i];
                ParseObject a2 = results[i + 1];
                int b1 = (int)a1["balance"];
                int b2 = (int)a2["balance"];
                Assert.IsTrue(b1 < b2 ||
                    a1.CreatedAt.CompareTo(a2.CreatedAt) >= 0);
            }
        }

        [Test]
        public async Task Include() {
            ParseObject world = new ParseObject("World");
            world["content"] = "7788";
            ParseObject hello = new ParseObject("Hello");
            hello["objectValue"] = world;
            await hello.Save();

            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Hello");
            query.Include("objectValue");
            ParseObject hello2 = await query.Get(hello.ObjectId);
            ParseObject world2 = hello2["objectValue"] as ParseObject;
            TestContext.WriteLine(world2["content"]);
            Assert.AreEqual(world2["content"], "7788");
        }

        [Test]
        public async Task Get() {
            ParseObject account = new ParseObject("Account");
            account["balance"] = 400;
            await account.Save();

            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            ParseObject account2 = await query.Get(account.ObjectId);
            Assert.AreEqual(account2["balance"], 400);
        }

        [Test]
        public async Task First() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            ParseObject account = await query.First();
            Assert.NotNull(account.ObjectId);
        }

        [Test]
        public async Task GreaterQuery() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            query.WhereGreaterThan("balance", 200);
            ReadOnlyCollection<ParseObject> list = await query.Find();
            TestContext.WriteLine(list.Count);
            Assert.Greater(list.Count, 0);
        }

        [Test]
        public async Task And() {
            ParseQuery<ParseObject> q1 = new ParseQuery<ParseObject>("Account");
            q1.WhereGreaterThan("balance", 100);
            ParseQuery<ParseObject> q2 = new ParseQuery<ParseObject>("Account");
            q2.WhereLessThan("balance", 500);
            ParseQuery<ParseObject> query = ParseQuery<ParseObject>.And(new List<ParseQuery<ParseObject>> { q1, q2 });
            ReadOnlyCollection<ParseObject> results = await query.Find();
            TestContext.WriteLine(results.Count);
            foreach (ParseObject item in results) {
                int balance = (int)item["balance"];
                Assert.IsTrue(balance >= 100 || balance <= 500);
            }
        }

        [Test]
        public async Task Or() {
            ParseQuery<ParseObject> q1 = new ParseQuery<ParseObject>("Account");
            q1.WhereLessThanOrEqualTo("balance", 100);
            ParseQuery<ParseObject> q2 = new ParseQuery<ParseObject>("Account");
            q2.WhereGreaterThanOrEqualTo("balance", 500);
            ParseQuery<ParseObject> query = ParseQuery<ParseObject>.Or(new List<ParseQuery<ParseObject>> { q1, q2 });
            ReadOnlyCollection<ParseObject> results = await query.Find();
            TestContext.WriteLine(results.Count);
            foreach (ParseObject item in results) {
                int balance = (int)item["balance"];
                Assert.IsTrue(balance <= 100 || balance >= 500);
            }
        }

        [Test]
        public async Task Exist() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            query.WhereExists("user");
            ReadOnlyCollection<ParseObject> results = await query.Find();
            foreach (ParseObject item in results) {
                Assert.NotNull(item["user"]);
            }

            query = new ParseQuery<ParseObject>("Account");
            query.WhereDoesNotExist("user");
            results = await query.Find();
            foreach (ParseObject item in results) {
                Assert.IsNull(item["user"]);
            }
        }

        [Test]
        public async Task Select() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            query.Select("balance");
            ReadOnlyCollection<ParseObject> results = await query.Find();
            foreach (ParseObject item in results) {
                Assert.NotNull(item["balance"]);
                Assert.IsNull(item["user"]);
            }
        }

        [Test]
        public async Task String() {
            // Start
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Hello");
            query.WhereStartsWith("stringValue", "hello");
            ReadOnlyCollection<ParseObject> results = await query.Find();
            foreach (ParseObject item in results) {
                string str = item["stringValue"] as string;
                Assert.IsTrue(str.StartsWith("hello"));
            }

            // End
            query = new ParseQuery<ParseObject>("Hello");
            query.WhereEndsWith("stringValue", "world");
            results = await query.Find();
            foreach (ParseObject item in results) {
                string str = item["stringValue"] as string;
                Assert.IsTrue(str.EndsWith("world"));
            }

            // Contains
            query = new ParseQuery<ParseObject>("Hello");
            query.WhereContains("stringValue", ",");
            results = await query.Find();
            foreach (ParseObject item in results) {
                string str = item["stringValue"] as string;
                Assert.IsTrue(str.Contains(','));
            }
        }

        [Test]
        public async Task Array() {
            // equal
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Book");
            query.WhereEqualTo("pages", 3);
            ReadOnlyCollection<ParseObject> results = await query.Find();
            foreach (ParseObject item in results) {
                List<object> pages = item["pages"] as List<object>;
                Assert.IsTrue(pages.Contains(3));
            }

            // contain all
            List<int> containAlls = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            query = new ParseQuery<ParseObject>("Book");
            query.WhereContainsAll("pages", containAlls);
            results = await query.Find();
            foreach (ParseObject item in results) {
                List<object> pages = item["pages"] as List<object>;
                pages.ForEach(i => {
                    Assert.IsTrue(pages.Contains(i));
                });
            }

            // contain in
            List<int> containIns = new List<int> { 4, 5, 6 };
            query = new ParseQuery<ParseObject>("Book");
            query.WhereContainedIn("pages", containIns);
            results = await query.Find();
            foreach (ParseObject item in results) {
                List<object> pages = item["pages"] as List<object>;
                bool f = false;
                containIns.ForEach(i => {
                    f |= pages.Contains(i);
                });
                Assert.IsTrue(f);
            }

            // not contain in
            List<int> notContainIns = new List<int> { 1, 2, 3 };
            query = new ParseQuery<ParseObject>("Book");
            query.WhereNotContainedIn("pages", notContainIns);
            results = await query.Find();
            foreach (ParseObject item in results) {
                List<object> pages = item["pages"] as List<object>;
                bool f = true;
                notContainIns.ForEach(i => {
                    f &= !pages.Contains(i);
                });
                Assert.IsTrue(f);
            }
        }

        [Test]
        public async Task Geo() {
            ParseObject obj = new ParseObject("Todo");
            ParseGeoPoint location = new ParseGeoPoint(39.9, 116.4);
            obj["location"] = location;
            await obj.Save();

            // near
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Todo");
            ParseGeoPoint point = new ParseGeoPoint(39.91, 116.41);
            query.WhereNear("location", point);
            ReadOnlyCollection<ParseObject> results = await query.Find();
            Assert.Greater(results.Count, 0);

            // in box
            query = new ParseQuery<ParseObject>("Todo");
            ParseGeoPoint southwest = new ParseGeoPoint(30, 115);
            ParseGeoPoint northeast = new ParseGeoPoint(40, 118);
            query.WhereWithinGeoBox("location", southwest, northeast);
            results = await query.Find();
            Assert.Greater(results.Count, 0);
        }

        [Test]
        public async Task Regex() {
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Hello");
            query.WhereMatches("stringValue", "^HEllo.*", modifiers: "i");
            ReadOnlyCollection<ParseObject> results = await query.Find();
            Assert.Greater(results.Count, 0);
            foreach (ParseObject item in results) {
                string str = item["stringValue"] as string;
                Assert.IsTrue(str.StartsWith("hello"));
            }
        }

        [Test]
        public async Task InQuery() {
            ParseQuery<ParseObject> worldQuery = new ParseQuery<ParseObject>("World");
            worldQuery.WhereEqualTo("content", "7788");
            ParseQuery<ParseObject> helloQuery = new ParseQuery<ParseObject>("Hello");
            helloQuery.WhereMatchesQuery("objectValue", worldQuery);
            helloQuery.Include("objectValue");
            ReadOnlyCollection<ParseObject> hellos = await helloQuery.Find();
            Assert.Greater(hellos.Count, 0);
            foreach (ParseObject item in hellos) {
                ParseObject world = item["objectValue"] as ParseObject;
                Assert.AreEqual(world["content"], "7788");
            }
        }

        [Test]
        public async Task NotInQuery() {
            ParseQuery<ParseObject> worldQuery = new ParseQuery<ParseObject>("World");
            worldQuery.WhereEqualTo("content", "7788");
            ParseQuery<ParseObject> helloQuery = new ParseQuery<ParseObject>("Hello");
            helloQuery.WhereDoesNotMatchQuery("objectValue", worldQuery);
            helloQuery.Include("objectValue");
            ReadOnlyCollection<ParseObject> hellos = await helloQuery.Find();
            Assert.Greater(hellos.Count, 0);
            foreach (ParseObject item in hellos) {
                ParseObject world = item["objectValue"] as ParseObject;
                Assert.IsTrue(world == null ||
                    world["content"] == null ||
                    world["content"] as string != "7788");
            }
        }
    }
}
