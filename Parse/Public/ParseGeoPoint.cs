﻿using System;
namespace ParseSDK {
    public class ParseGeoPoint {
        public double Latitude {
            get;
        }

        public double Longitude {
            get;
        }

        public ParseGeoPoint(double latitude, double longtitude) {
            Latitude = latitude;
            Longitude = longtitude;
        }

        public static ParseGeoPoint Origin {
            get {
                return new ParseGeoPoint(0, 0);
            }
        }

        public double KilometersTo(ParseGeoPoint point) {
            if (point == null) {
                throw new ArgumentNullException(nameof(point));
            }
            return RadiansTo(point) * 6371.0;
        }

        public double MilesTo(ParseGeoPoint point) {
            if (point == null) {
                throw new ArgumentNullException(nameof(point));
            }
            return RadiansTo(point) * 3958.8;
        }

        public double RadiansTo(ParseGeoPoint point) {
            if (point == null) {
                throw new ArgumentNullException(nameof(point));
            }
            double d2r = Math.PI / 180.0;
            double lat1rad = Latitude * d2r;
            double long1rad = Longitude * d2r;
            double lat2rad = point.Latitude * d2r;
            double long2rad = point.Longitude * d2r;
            double deltaLat = lat1rad - lat2rad;
            double deltaLong = long1rad - long2rad;
            double sinDeltaLatDiv2 = Math.Sin(deltaLat / 2);
            double sinDeltaLongDiv2 = Math.Sin(deltaLong / 2);
            double a = sinDeltaLatDiv2 * sinDeltaLatDiv2 +
                Math.Cos(lat1rad) * Math.Cos(lat2rad) * sinDeltaLongDiv2 * sinDeltaLongDiv2;
            a = Math.Min(1.0, a);
            return 2 * Math.Sin(Math.Sqrt(a));
        }
    }
}
