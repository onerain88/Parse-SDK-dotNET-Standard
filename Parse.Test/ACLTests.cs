using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ParseSDK.Test {
    public class ACLTests {
        [Test]
        public async Task PrivateReadAndWrite() {
            ParseObject account = new ParseObject("Account");
            ParseACL acl = new ParseACL();
            acl.PublicReadAccess = false;
            acl.PublicWriteAccess = false;
            account.ACL = acl;
            account["balance"] = 1024;
            await account.Save();
            Assert.IsFalse(acl.PublicReadAccess);
            Assert.IsFalse(acl.PublicWriteAccess);
        }

        [Test]
        public async Task UserReadAndWrite() {
            await ParseUser.Login("hello", "world");
            ParseObject account = new ParseObject("Account");
            ParseUser currentUser = await ParseUser.GetCurrent();
            ParseACL acl = ParseACL.CreateWithOwner(currentUser);
            account.ACL = acl;
            account["balance"] = 512;
            await account.Save();

            Assert.IsTrue(acl.GetUserReadAccess(currentUser));
            Assert.IsTrue(acl.GetUserWriteAccess(currentUser));

            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            ParseObject result = await query.Get(account.ObjectId);
            TestContext.WriteLine(result.ObjectId);
            Assert.NotNull(result.ObjectId);

            await ParseUser.Logout();
            result = await query.Get(account.ObjectId);
            Assert.IsNull(result);
        }

        [Test]
        public async Task RoleReadAndWrite() {
            ParseQuery<ParseRole> query = ParseRole.GetQuery();
            ParseRole owner = await query.Get("5e1440cbfc36ed006add1b8d");
            ParseObject account = new ParseObject("Account");
            ParseACL acl = new ParseACL();
            acl.SetRoleReadAccess(owner, true);
            acl.SetRoleWriteAccess(owner, true);
            account.ACL = acl;
            await account.Save();
            Assert.IsTrue(acl.GetRoleReadAccess(owner));
            Assert.IsTrue(acl.GetRoleWriteAccess(owner));
        }

        [Test]
        public async Task Query() {
            await ParseUser.Login("game", "play");
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account");
            ParseObject account = await query.Get("5e144525dd3c13006a8f8de2");
            TestContext.WriteLine(account.ObjectId);
            Assert.NotNull(account.ObjectId);
        }

        [Test]
        public async Task Serialization() {
            await ParseUser.Login("hello", "world");
            ParseQuery<ParseObject> query = new ParseQuery<ParseObject>("Account") {
                IncludeACL = true
            };
            query.OrderByDescending("createdAt");
            ReadOnlyCollection<ParseObject> accounts = await query.Find();
            foreach (ParseObject account in accounts) {
                TestContext.WriteLine($"public read access: {account.ACL.PublicReadAccess}");
                TestContext.WriteLine($"public write access: {account.ACL.PublicWriteAccess}");
            }
        }
    }
}
