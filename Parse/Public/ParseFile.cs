using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using ParseSDK.Internal.File;

namespace ParseSDK {
    public class ParseFile : ParseObject {
        public const string CLASS_NAME = "File";

        public string Name {
            get {
                return this["name"] as string;
            }
            set {
                this["name"] = value;
            }
        }

        public string MimeType {
            get {
                return this["mime_type"] as string;
            }
            set {
                this["mime_type"] = value;
            }
        }

        public string Url {
            get {
                return this["url"] as string;
            }
            set {
                this["url"] = value;
            }
        }

        public Dictionary<string, object> MetaData {
            get {
                return this["metaData"] as Dictionary<string, object>;
            }
            set {
                this["metaData"] = value;
            }
        }

        readonly Stream stream;

        public ParseFile() : base(CLASS_NAME) {
            MetaData = new Dictionary<string, object>();
        }

        public ParseFile(string name, byte[] bytes) : this() {
            Name = name;
            stream = new MemoryStream(bytes);
        }

        public ParseFile(string name, string path) : this() {
            Name = name;
            MimeType = ParseMimeTypeMap.GetMimeType(path);
            stream = new FileStream(path, FileMode.Open);
        }

        public ParseFile(string name, Uri url) : this() {
            Name = name;
            Url = url.AbsoluteUri;
        }

        public void AddMetaData(string key, object value) {
            MetaData[key] = value;
        }

        public async Task<ParseFile> Save(Action<long, long> onProgress = null) {
            if (!string.IsNullOrEmpty(Url)) {
                // 外链方式
                await base.Save();
            } else {
                // 上传文件
                Dictionary<string, object> ret = await ParseClient.HttpClient.PostStream<Dictionary<string, object>>($"files/{Name}",
                    stream, onProgress);
                string url = ret["url"] as string;
                Url = url;
                await base.Save();
            }
            return this;
        }

        public new async Task Delete() {
            if (string.IsNullOrEmpty(ObjectId)) {
                return;
            }
            string path = $"files/{ObjectId}";
            await ParseClient.HttpClient.Delete(path);
        }

        public string GetThumbnailUrl(int width, int height, int quality = 100, bool scaleToFit = true, string format = "png") {
            int mode = scaleToFit ? 2 : 1;
            return $"{Url}?imageView/{mode}/w/{width}/h/{height}/q/{quality}/format/{format}";
        }

        async Task<Dictionary<string, object>> GetUploadToken() {
            Dictionary<string, object> data = new Dictionary<string, object> {
                { "name", Name },
                { "key", Guid.NewGuid().ToString() },
                { "__type", "File" },
                { "mime_type", MimeType },
                { "metaData", MetaData }
            };
            return await ParseClient.HttpClient.Post<Dictionary<string, object>>("fileTokens", data: data);
        }

        public static ParseQuery<ParseFile> GetQuery() {
            return new ParseQuery<ParseFile>(CLASS_NAME);
        }
    }
}
