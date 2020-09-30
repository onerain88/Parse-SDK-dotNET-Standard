using System;

namespace Parse {
    public class ParseRole : ParseObject {
        public const string CLASS_NAME = "_Role";

        public string Name {
            get {
                return this["name"] as string;
            }
            set {
                this["name"] = value;
            }
        }

        public ParseRelation<ParseRole> Roles {
            get {
                ParseRelation<ParseObject> roles = this["roles"] as ParseRelation<ParseObject>;
                return new ParseRelation<ParseRole> {
                    Parent = roles.Parent,
                    Key = "roles"
                };
            }
        }

        public ParseRelation<ParseUser> Users {
            get {
                ParseRelation<ParseObject> users = this["users"] as ParseRelation<ParseObject>;
                return new ParseRelation<ParseUser> {
                    Parent = users.Parent,
                    Key = "users"
                };
            }
        }

        public ParseRole() : base(CLASS_NAME) {
        }

        public static ParseRole Create(string name, ParseACL acl) {
            ParseRole role = new ParseRole() {
                Name = name,
                ACL = acl
            };
            return role;
        }

        public static ParseQuery<ParseRole> GetQuery() {
            return new ParseQuery<ParseRole>(CLASS_NAME);
        }
    }
}
