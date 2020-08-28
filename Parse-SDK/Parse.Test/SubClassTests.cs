using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Parse.Test {
    public class SubClassTests {
        internal class Hello : ParseObject {
            internal World World => this["objectValue"] as World;

            internal Hello() : base("Hello") { }
        }

        internal class World : ParseObject {
            internal string Content {
                get {
                    return this["content"] as string;
                }
                set {
                    this["content"] = value;
                }
            }

            internal World() : base("World") { }
        }

        internal class Account : ParseObject {
            internal int Balance {
                get {
                    return (int)this["balance"];
                }
                set {
                    this["balance"] = value;
                }
            }

            internal Account() : base("Account") { }
        }

        [TestFixture]
        public class SubClassTest {
            [Test]
            public async Task Create() {
                ParseObject.RegisterSubclass("Account", () => new Account());
                Account account = new Account();
                account.Balance = 1000;
                await account.Save();
                TestContext.WriteLine(account.ObjectId);
                Assert.NotNull(account.ObjectId);
            }

            [Test]
            public async Task Query() {
                ParseObject.RegisterSubclass("Account", () => new Account());
                ParseQuery<Account> query = new ParseQuery<Account>("Account");
                query.WhereGreaterThan("balance", 500);
                ReadOnlyCollection<Account> list = await query.Find();
                TestContext.WriteLine(list.Count);
                Assert.Greater(list.Count, 0);
                foreach (Account account in list) {
                    Assert.NotNull(account.ObjectId);
                }
            }

            [Test]
            public async Task Delete() {
                ParseObject.RegisterSubclass("Account", () => new Account());
                Account account = new Account() {
                    Balance = 1024
                };
                await account.Save();
                await account.Delete();
            }

            [Test]
            public async Task Include() {
                ParseObject.RegisterSubclass("Hello", () => new Hello());
                ParseObject.RegisterSubclass("World", () => new World());

                ParseQuery<Hello> helloQuery = new ParseQuery<Hello>("Hello");
                helloQuery.Include("objectValue");
                Hello hello = await helloQuery.Get("5e0d55aedd3c13006a53cd87");
                World world = hello.World;

                TestContext.WriteLine(hello.ObjectId);
                Assert.AreEqual(hello.ObjectId, "5e0d55aedd3c13006a53cd87");
                TestContext.WriteLine(world.ObjectId);
                Assert.AreEqual(world.ObjectId, "5e0d55ae21460d006a1ec931");
                Assert.AreEqual(world.Content, "7788");
            }
        }
    }
}
