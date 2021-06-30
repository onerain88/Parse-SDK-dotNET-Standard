using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static NUnit.Framework.TestContext;

namespace ParseSDK.Test {
    public class ObjectTests {
        [Test]
        public async Task CreateObject() {
            ParseObject @object = new ParseObject("Hello");
            @object["intValue"] = 123;
            @object["boolValue"] = true;
            @object["stringValue"] = "hello, world";
            @object["time"] = DateTime.Now;
            @object["intList"] = new List<int> { 1, 1, 2, 3, 5, 8 };
            @object["stringMap"] = new Dictionary<string, object> {
                { "k1", 111 },
                { "k2", true },
                { "k3", "haha" }
            };
            ParseObject nestedObj = new ParseObject("World");
            nestedObj["content"] = "7788";
            @object["objectValue"] = nestedObj;
            @object["pointerList"] = new List<object> { new ParseObject("World"), nestedObj };
            await @object.Save();

            WriteLine(@object.ClassName);
            WriteLine(@object.ObjectId);
            WriteLine(@object.CreatedAt);
            WriteLine(@object.UpdatedAt);
            WriteLine(@object["intValue"]);
            WriteLine(@object["boolValue"]);
            WriteLine(@object["stringValue"]);
            WriteLine(@object["objectValue"]);
            WriteLine(@object["time"]);

            Assert.AreEqual(nestedObj, @object["objectValue"]);
            WriteLine(nestedObj.ClassName);
            WriteLine(nestedObj.ObjectId);

            Assert.NotNull(@object.ObjectId);
            Assert.NotNull(@object.ClassName);
            Assert.NotNull(@object.CreatedAt);
            Assert.NotNull(@object.UpdatedAt);
            Assert.AreEqual(@object["intValue"], 123);
            Assert.AreEqual(@object["boolValue"], true);
            Assert.AreEqual(@object["stringValue"], "hello, world");

            Assert.NotNull(nestedObj);
            Assert.NotNull(nestedObj.ClassName);
            Assert.NotNull(nestedObj.ObjectId);
            Assert.NotNull(nestedObj.CreatedAt);
            Assert.NotNull(nestedObj.UpdatedAt);

            List<object> pointerList = @object["pointerList"] as List<object>;
            foreach (object pointerObj in pointerList) {
                ParseObject pointer = pointerObj as ParseObject;
                Assert.NotNull(pointer.ObjectId);
            }
        }

        [Test]
        public async Task SaveAll() {
            List<ParseObject> list = new List<ParseObject>();
            for (int i = 0; i < 5; i++) {
                ParseObject world = new ParseObject("World");
                world["content"] = $"word_{i}";
                list.Add(world);
            }
            await ParseObject.SaveAll(list);
            foreach (ParseObject obj in list) {
                Assert.NotNull(obj.ObjectId);
            }
        }

        [Test]
        public async Task Delete() {
            ParseObject world = new ParseObject("World");
            await world.Save();
            await world.Delete();
        }

        [Test]
        public async Task DeleteAll() {
            List<ParseObject> list = new List<ParseObject> {
                new ParseObject("World"),
                new ParseObject("World"),
                new ParseObject("World"),
                new ParseObject("World")
            };
            await ParseObject.SaveAll(list);
            await ParseObject.DeleteAll(list);
        }

        [Test]
        public async Task Fetch() {
            ParseObject world = new ParseObject("World");
            world["content"] = "7788";
            ParseObject hello = new ParseObject("Hello");
            hello["objectValue"] = world;
            await hello.Save();

            ParseObject hello2 = ParseObject.CreateWithoutData("Hello", hello.ObjectId);
            await hello2.Fetch(includes: new List<string> { "objectValue" });
            ParseObject world2 = hello2["objectValue"] as ParseObject;
            WriteLine(world2["content"]);
            Assert.AreEqual(world2["content"], "7788");
        }

        [Test]
        public async Task SaveWithOption() {
            ParseObject account = new ParseObject("Account");
            account["balance"] = 10;
            await account.Save();

            account["balance"] = 1000;
            ParseQuery<ParseObject> q = new ParseQuery<ParseObject>("Account");
            q.WhereGreaterThan("balance", 100);
            try {
                await account.Save(fetchWhenSave: true, query: q);
            } catch (ParseException e) {
                WriteLine($"{e.Code} : {e.Message}");
                Assert.AreEqual(e.Code, 305);
            }
        }

        [Test]
        public async Task Unset() {
            ParseObject hello = new ParseObject("Hello");
            hello["content"] = "hello, world";
            await hello.Save();
            WriteLine(hello["content"]);
            Assert.AreEqual(hello["content"], "hello, world");

            hello.Unset("content");
            await hello.Save();
            WriteLine(hello["content"]);
            Assert.IsNull(hello["content"]);
        }

        [Test]
        public async Task OperateNullProperty() {
            ParseObject obj = new ParseObject("Hello");
            obj.Increment("intValue", 123);
            obj.Increment("intValue", 321);
            obj.Add("intList", 1);
            obj.Add("intList", 2);
            obj.Add("intList", 3);
            await obj.Save();

            WriteLine(obj["intValue"]);
            Assert.AreEqual(obj["intValue"], 444);
            List<object> intList = obj["intList"] as List<object>;
            WriteLine(intList.Count);
            Assert.AreEqual(intList.Count, 3);
            Assert.AreEqual(intList[0], 1);
            Assert.AreEqual(intList[1], 2);
            Assert.AreEqual(intList[2], 3);
        }

        [Test]
        public async Task Serialization() {
            ParseObject obj = new ParseObject("Hello");
            obj["intValue"] = 123;
            obj["boolValue"] = true;
            obj["stringValue"] = "hello, world";
            obj["time"] = DateTime.Now;
            obj["intList"] = new List<int> { 1, 1, 2, 3, 5, 8 };
            obj["stringMap"] = new Dictionary<string, object> {
                { "k1", 111 },
                { "k2", true },
                { "k3", "haha" }
            };
            ParseObject nestedObj = new ParseObject("World");
            nestedObj["content"] = "7788";
            obj["objectValue"] = nestedObj;
            obj["pointerList"] = new List<object> {
                new ParseObject("World"),
                nestedObj
            };
            await obj.Save();

            string json = obj.ToString();
            WriteLine(json);
            ParseObject newObj = ParseObject.Parse(json);
            Assert.NotNull(newObj.ObjectId);
            Assert.NotNull(newObj.ClassName);
            Assert.NotNull(newObj.CreatedAt);
            Assert.NotNull(newObj.UpdatedAt);
            Assert.AreEqual(newObj["intValue"], 123);
            Assert.AreEqual(newObj["boolValue"], true);
            Assert.AreEqual(newObj["stringValue"], "hello, world");
        }
    }
}