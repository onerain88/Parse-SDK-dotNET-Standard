using NUnit.Framework;

namespace Parse.Test {
    public class GeoTests {
        [Test]
        public void CaParseulate() {
            ParseGeoPoint p1 = new ParseGeoPoint(20.0059, 110.3665);
            ParseGeoPoint p2 = new ParseGeoPoint(20.0353, 110.3645);
            double kilometers = p1.KilometersTo(p2);
            TestContext.WriteLine(kilometers);
            Assert.Less(kilometers - 3.275, 0.01);

            double miles = p1.MilesTo(p2);
            TestContext.WriteLine(miles);
            Assert.Less(miles - 2.035, 0.01);

            double radians = p1.RadiansTo(p2);
            TestContext.WriteLine(radians);
            Assert.Less(radians - 0.0005, 0.0001);
        }
    }
}
