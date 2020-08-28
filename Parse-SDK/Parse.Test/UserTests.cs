using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Parse.Test {
    public class UserTests {
        private string username;

        private string password;

        private string email;

        private string mobile;

        [OneTimeSetUp]
        public void Setup() {
            long unixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            username = $"{unixTime}";
            password = "world";
            email = $"{unixTime}@qq.com";
            mobile = $"{unixTime / 100}";
        }

        [Test]
        [Order(0)]
        public async Task SignUp() {
            ParseUser user = new ParseUser {
                Username = username,
                Password = password,
                Email = email,
                Mobile = mobile
            };
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
        [Order(1)]
        public async Task Login() {
            await ParseUser.Login(username, password);
            ParseUser current = await ParseUser.GetCurrent();
            Assert.NotNull(current.ObjectId);
        }

        [Test]
        [Order(2)]
        public async Task LoginByEmail() {
            await ParseUser.LoginByEmail(email, password);
            ParseUser current = await ParseUser.GetCurrent();
            Assert.NotNull(current.ObjectId);
        }

        [Test]
        [Order(3)]
        public async Task LoginBySessionToken() {
            ParseUser currentUser = await ParseUser.Login(username, password);
            string sessionToken = currentUser.SessionToken;
            await ParseUser.Logout();
            await ParseUser.BecomeWithSessionToken(sessionToken);
            ParseUser current = await ParseUser.GetCurrent();
            Assert.NotNull(current.ObjectId);
        }

        [Test]
        [Order(4)]
        public async Task RelateObject() {
            ParseUser currentUser = await ParseUser.Login(username, password);
            ParseObject account = new ParseObject("Account");
            account["user"] = currentUser;
            await account.Save();
        }

        [Test]
        [Order(5)]
        public async Task IsAuthenticated() {
            ParseUser currentUser = await ParseUser.Login(username, password);
            bool isAuthenticated = await currentUser.IsAuthenticated();
            TestContext.WriteLine(isAuthenticated);
            Assert.IsTrue(isAuthenticated);
        }
    }
}
