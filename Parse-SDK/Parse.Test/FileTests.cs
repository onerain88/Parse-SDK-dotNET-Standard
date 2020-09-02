using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Parse.Test {
    public class FileTests {
        [Test]
        public async Task SaveFromPath() {
            string path = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            string filePath = $"{path}/Assets/avatar.jpg";
            ParseFile file = new ParseFile("avatar.jpg", filePath);
            await file.Save((progress, total) => {
                TestContext.WriteLine($"{progress}/{total}");
            });
        }

        [Test]
        public async Task SaveFromUrl() {
            ParseFile file = new ParseFile("avatar.jpg", new Uri("http://img95.699pic.com/photo/50015/9034.jpg_wh300.jpg"));
            await file.Save();
        }

        [Test]
        public async Task SaveFromMemory() {
            string text = "hello, world";
            byte[] data = Encoding.UTF8.GetBytes(text);
            ParseFile file = new ParseFile("text", data);
            await file.Save();
        }
    }
}
