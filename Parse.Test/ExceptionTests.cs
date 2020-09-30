using NUnit.Framework;

namespace Parse.Test {
    public class ExceptionTests {
        [Test]
        public void LeanCloudException() {
            try {
                throw new ParseException(123, "hello, world");
            } catch (ParseException e) {
                TestContext.WriteLine($"{e.Code} : {e.Message}");
                Assert.AreEqual(e.Code, 123);
            }
        }
    }
}
