using System;
using System.Collections.Generic;

namespace ParseSDK {
    public class ParseACL {
        const string PublicKey = "*";

        const string RoleKeyPrefix = "role:";

        internal Dictionary<string, bool> readAccess = new Dictionary<string, bool>();
        internal Dictionary<string, bool> writeAccess = new Dictionary<string, bool>();

        public static ParseACL CreateWithOwner(ParseUser owner) {
            if (owner == null) {
                throw new ArgumentNullException(nameof(owner));
            }
            ParseACL acl = new ParseACL();
            acl.SetUserReadAccess(owner, true);
            acl.SetUserWriteAccess(owner, true);
            return acl;
        }

        public bool PublicReadAccess {
            get {
                return GetAccess(readAccess, PublicKey);
            }
            set {
                SetAccess(readAccess, PublicKey, value);
            }
        }

        public bool PublicWriteAccess {
            get {
                return GetAccess(writeAccess, PublicKey);
            }
            set {
                SetAccess(writeAccess, PublicKey, value);
            }
        }

        public bool GetUserIdReadAccess(string userId) {
            if (string.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException(nameof(userId));
            }
            return GetAccess(readAccess, userId);
        }

        public void SetUserIdReadAccess(string userId, bool value) {
            if (string.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException(nameof(userId));
            }
            SetAccess(readAccess, userId, value);
        }

        public bool GetUserIdWriteAccess(string userId) {
            if (string.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException(nameof(userId));
            }
            return GetAccess(writeAccess, userId);
        }

        public void SetUserIdWriteAccess(string userId, bool value) {
            if (string.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException(nameof(userId));
            }
            SetAccess(writeAccess, userId, value);
        }

        public bool GetUserReadAccess(ParseUser user) {
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            return GetUserIdReadAccess(user.ObjectId);
        }

        public void SetUserReadAccess(ParseUser user, bool value) {
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            SetUserIdReadAccess(user.ObjectId, value);
        }

        public bool GetUserWriteAccess(ParseUser user) {
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            return GetUserIdWriteAccess(user.ObjectId);
        }

        public void SetUserWriteAccess(ParseUser user, bool value) {
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            SetUserIdWriteAccess(user.ObjectId, value);
        }

        public bool GetRoleReadAccess(ParseRole role) {
            if (role == null) {
                throw new ArgumentNullException(nameof(role));
            }
            string roleKey = $"{RoleKeyPrefix}{role.ObjectId}";
            return GetAccess(readAccess, roleKey);
        }

        public void SetRoleReadAccess(ParseRole role, bool value) {
            if (role == null) {
                throw new ArgumentNullException(nameof(role));
            }
            string roleKey = $"{RoleKeyPrefix}{role.ObjectId}";
            SetAccess(readAccess, roleKey, value);
        }

        public bool GetRoleWriteAccess(ParseRole role) {
            if (role == null) {
                throw new ArgumentNullException(nameof(role));
            }
            string roleKey = $"{RoleKeyPrefix}{role.ObjectId}";
            return GetAccess(writeAccess, roleKey);
        }

        public void SetRoleWriteAccess(ParseRole role, bool value) {
            if (role == null) {
                throw new ArgumentNullException(nameof(role));
            }
            string roleKey = $"{RoleKeyPrefix}{role.ObjectId}";
            SetAccess(writeAccess, roleKey, value);
        }

        bool GetAccess(Dictionary<string, bool> access, string key) {
            return access.ContainsKey(key) ?
                access[key] : false;
        }

        void SetAccess(Dictionary<string, bool> access, string key, bool value) {
            access[key] = value;
        }
    }
}
