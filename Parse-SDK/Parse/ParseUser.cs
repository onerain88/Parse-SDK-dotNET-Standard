using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parse.Internal.Object;

namespace Parse {
    public class ParseUser : ParseObject {
        public const string CLASS_NAME = "_User";

        public string Username {
            get {
                return this["username"] as string;
            }
            set {
                this["username"] = value;
            }
        }

        public string Password {
            get {
                return this["password"] as string;
            }
            set {
                this["password"] = value;
            }
        }

        public string Email {
            get {
                return this["email"] as string;
            }
            set {
                this["email"] = value;
            }
        }

        public string Mobile {
            get {
                return this["mobilePhoneNumber"] as string;
            }
            set {
                this["mobilePhoneNumber"] = value;
            }
        }

        public string SessionToken {
            get {
                return this["sessionToken"] as string;
            }
            set {
                this["sessionToken"] = value;
            }
        }

        public bool EmailVerified {
            get {
                return Convert.ToBoolean(this["emailVerified"]);
            }
        }

        public bool MobileVerified {
            get {
                return Convert.ToBoolean(this["mobilePhoneVerified"]);
            }
        }

        public Dictionary<string, object> AuthData {
            get {
                return this["authData"] as Dictionary<string, object>;
            }
            set {
                this["authData"] = value;
            }
        }

        public bool IsAnonymous => AuthData != null &&
            AuthData.ContainsKey("anonymous");

        static ParseUser currentUser;

        public static Task<ParseUser> GetCurrent() {
            // TODO 加载持久化数据

            return Task.FromResult(currentUser);
        }

        public ParseUser() : base(CLASS_NAME) {

        }

        public ParseUser(ParseObjectData objectData) : this() {
            Merge(objectData);
        }

        public async Task<ParseUser> SignUp() {
            if (string.IsNullOrEmpty(Username)) {
                throw new ArgumentNullException(nameof(Username));
            }
            if (string.IsNullOrEmpty(Password)) {
                throw new ArgumentNullException(nameof(Password));
            }
            if (!string.IsNullOrEmpty(ObjectId)) {
                throw new ArgumentException("Cannot sign up a user that already exists.");
            }
            await Save();
            currentUser = this;
            // TODO Persistence

            return this;
        }

        public static async Task RequestLoginSMSCode(string mobile) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile }
            };
            await ParseClient.HttpClient.Post<Dictionary<string, object>>("requestLoginSmsCode", data: data);
        }

        public static async Task<ParseUser> SignUpOrLoginByMobilePhone(string mobile, string code) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(code));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile },
                { "smsCode", code }
            };
            Dictionary<string, object> response = await ParseClient.HttpClient.Post<Dictionary<string, object>>("usersByMobilePhone", data: data);
            ParseObjectData objectData = ParseObjectData.Decode(response);
            currentUser = new ParseUser(objectData);
            return currentUser;
        }

        public static Task<ParseUser> Login(string username, string password) {
            if (string.IsNullOrEmpty(username)) {
                throw new ArgumentNullException(nameof(username));
            }
            if (string.IsNullOrEmpty(password)) {
                throw new ArgumentNullException(nameof(password));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "username", username },
                { "password", password }
            };
            return Login(data);
        }

        public static Task<ParseUser> LoginByEmail(string email, string password) {
            if (string.IsNullOrEmpty(email)) {
                throw new ArgumentNullException(nameof(email));
            }
            if (string.IsNullOrEmpty(password)) {
                throw new ArgumentNullException(nameof(password));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "email", email },
                { "password", password }
            };
            return Login(data);
        }

        public static Task<ParseUser> LoginByMobilePhoneNumber(string mobile, string password) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            if (string.IsNullOrEmpty(password)) {
                throw new ArgumentNullException(nameof(password));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile },
                { "password", password }
            };
            return Login(data);
        }

        public static Task<ParseUser> LoginBySMSCode(string mobile, string code) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            if (string.IsNullOrEmpty(code)) {
                throw new ArgumentNullException(nameof(code));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile },
                { "smsCode", code }
            };
            return Login(data);
        }

        public static async Task RequestEmailVerify(string email) {
            if (string.IsNullOrEmpty(email)) {
                throw new ArgumentNullException(nameof(email));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "email", email }
            };
            await ParseClient.HttpClient.Post<Dictionary<string, object>>("requestEmailVerify", data: data);
        }

        public static async Task RequestMobilePhoneVerify(string mobile) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile }
            };
            await ParseClient.HttpClient.Post<Dictionary<string, object>>("requestMobilePhoneVerify", data: data);
        }

        public static async Task VerifyMobilePhone(string mobile, string code) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            if (string.IsNullOrEmpty(code)) {
                throw new ArgumentNullException(nameof(code));
            }
            string path = $"verifyMobilePhone/{code}";
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile }
            };
            await ParseClient.HttpClient.Post<Dictionary<string, object>>(path, data: data);
        }

        public static async Task<ParseUser> BecomeWithSessionToken(string sessionToken) {
            if (string.IsNullOrEmpty(sessionToken)) {
                throw new ArgumentNullException(nameof(sessionToken));
            }
            Dictionary<string, object> headers = new Dictionary<string, object> {
                { "X-Parse-Session", sessionToken }
            };
            Dictionary<string, object> response = await ParseClient.HttpClient.Get<Dictionary<string, object>>("users/me",
                headers: headers);
            ParseObjectData objectData = ParseObjectData.Decode(response);
            currentUser = new ParseUser(objectData);
            return currentUser;
        }

        public static async Task RequestPasswordReset(string email) {
            if (string.IsNullOrEmpty(email)) {
                throw new ArgumentNullException(nameof(email));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "email", email }
            };
            await ParseClient.HttpClient.Post<Dictionary<string, object>>("requestPasswordReset",
                data: data);
        }

        public static async Task RequestPasswordRestBySmsCode(string mobile) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile }
            };
            await ParseClient.HttpClient.Post<Dictionary<string, object>>("requestPasswordResetBySmsCode",
                data: data);
        }

        public static async Task ResetPasswordBySmsCode(string mobile, string code, string newPassword) {
            if (string.IsNullOrEmpty(mobile)) {
                throw new ArgumentNullException(nameof(mobile));
            }
            if (string.IsNullOrEmpty(code)) {
                throw new ArgumentNullException(nameof(code));
            }
            if (string.IsNullOrEmpty(newPassword)) {
                throw new ArgumentNullException(nameof(newPassword));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile },
                { "password", newPassword }
            };
            await ParseClient.HttpClient.Put<Dictionary<string, object>>($"resetPasswordBySmsCode/{code}",
                data: data);
        }

        public async Task UpdatePassword(string oldPassword, string newPassword) {
            if (string.IsNullOrEmpty(oldPassword)) {
                throw new ArgumentNullException(nameof(oldPassword));
            }
            if (string.IsNullOrEmpty(newPassword)) {
                throw new ArgumentNullException(nameof(newPassword));
            }
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "old_password", oldPassword },
                { "new_password", newPassword }
            };
            Dictionary<string, object> response = await ParseClient.HttpClient.Put<Dictionary<string, object>>(
                $"users/{ObjectId}/updatePassword", data: data);
            ParseObjectData objectData = ParseObjectData.Decode(response);
            Merge(objectData);
        }

        public static Task Logout() {
            currentUser = null;
            // TODO 清理持久化数据

            return Task.FromResult<object>(null);
        }

        public async Task<bool> IsAuthenticated() {
            if (SessionToken == null || ObjectId == null) {
                return false;
            }
            try {
                await ParseClient.HttpClient.Get<Dictionary<string, object>>("users/me");
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static ParseQuery<ParseUser> GetQuery() {
            return new ParseQuery<ParseUser>(CLASS_NAME);
        }

        Task LinkWithAuthData(string authType, Dictionary<string, object> data) {
            AuthData = new Dictionary<string, object> {
                { authType, data }
            };
            return Save();
        }

        static async Task<ParseUser> Login(Dictionary<string, object> data) {
            Dictionary<string, object> response = await ParseClient.HttpClient.Post<Dictionary<string, object>>("login", data: data);
            ParseObjectData objectData = ParseObjectData.Decode(response);
            currentUser = new ParseUser(objectData);
            return currentUser;
        }

        static async Task<ParseUser> LoginWithAuthData(string authType, Dictionary<string, object> data, bool failOnNotExist) {
            Dictionary<string, object> authData = new Dictionary<string, object> {
                { authType, data }
            };
            string path = failOnNotExist ? "users?failOnNotExist=true" : "users";
            Dictionary<string, object> response = await ParseClient.HttpClient.Post<Dictionary<string, object>>(path, data: new Dictionary<string, object> {
                { "authData", authData }
            });
            ParseObjectData objectData = ParseObjectData.Decode(response);
            currentUser = new ParseUser(objectData);
            return currentUser;
        }

        public static async Task RequestSMSCodeForUpdatingPhoneNumber(string mobile, int ttl = 360, string captchaToken = null) {
            string path = "requestChangePhoneNumber";
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile },
                { "ttl", ttl }
            };
            if (!string.IsNullOrEmpty(captchaToken)) {
                data["validate_token"] = captchaToken;
            }
            await ParseClient.HttpClient.Post<Dictionary<string, object>>(path, data: data);
        }

        public static async Task VerifyCodeForUpdatingPhoneNumber(string mobile, string code) {
            string path = "changePhoneNumber";
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "mobilePhoneNumber", mobile },
                { "code", code }
            };
            await ParseClient.HttpClient.Post<Dictionary<string, object>>(path, data: data);
        }
    }
}
