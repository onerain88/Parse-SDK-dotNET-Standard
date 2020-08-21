using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parse.Test {
    public class UserTests {
        [SetUp]
        public void Setup() {
            ParseLogger.LogDelegate += Utils.Log;
            Utils.Init();
        }

        [TearDown]
        public void TearDown() {
            ParseLogger.LogDelegate -= Utils.Log;
        }

        [Test]
        public async Task SignUp() {
            ParseUser user = new ParseUser();
            long unixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            user.Username = $"{unixTime}";
            user.Password = "world";
            string email = $"{unixTime}@qq.com";
            user.Email = email;
            string mobile = $"{unixTime / 100}";
            user.Mobile = mobile;
            await user.SignUp();

            TestContext.WriteLine(user.Username);
            TestContext.WriteLine(user.Password);

            Assert.NotNull(user.ObjectId);
            TestContext.WriteLine(user.ObjectId);
            Assert.NotNull(user.SessionToken);
            TestContext.WriteLine(user.SessionToken);
            Assert.AreEqual(user.Email, email);
        }

        [Test]
        public async Task Login() {
            await ParseUser.Login("hello", "world");
            ParseUser current = await ParseUser.GetCurrent();
            Assert.NotNull(current.ObjectId);
        }

        [Test]
        public async Task LoginByEmail() {
            await ParseUser.LoginByEmail("171253484@qq.com", "world");
            ParseUser current = await ParseUser.GetCurrent();
            Assert.NotNull(current.ObjectId);
        }

        [Test]
        public async Task LoginBySessionToken() {
            await ParseUser.Logout();
            string sessionToken = "luo2fpl4qij2050e7enqfz173";
            await ParseUser.BecomeWithSessionToken(sessionToken);
            ParseUser current = await ParseUser.GetCurrent();
            Assert.NotNull(current.ObjectId);
        }

        [Test]
        public async Task RelateObject() {
            ParseUser user = await ParseUser.LoginByMobilePhoneNumber("15101006007", "112358");
            ParseObject account = new ParseObject("Account");
            account["user"] = user;
            await account.Save();
            Assert.AreEqual(user.ObjectId, "5e0d5c667d5774006a5c1177");
        }

        [Test]
        public async Task IsAuthenticated() {
            ParseUser currentUser = await ParseUser.Login("hello", "world");
            bool isAuthenticated = await currentUser.IsAuthenticated();
            TestContext.WriteLine(isAuthenticated);
            Assert.IsTrue(isAuthenticated);
        }

        [Test]
        public async Task UpdatePassword() {
            ParseUser currentUser = await ParseUser.Login("hello", "world");
            await currentUser.UpdatePassword("world", "newWorld");
            await currentUser.UpdatePassword("newWorld", "world");
        }
    }
}
