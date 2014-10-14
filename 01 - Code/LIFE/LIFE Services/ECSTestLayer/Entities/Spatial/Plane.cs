namespace Modelthulhu.Geom {
    using CommonTypes.TransportTypes;

    #region Namespace imports

    using System;
    using CommonTypes.DataTypes;

    #endregion

    // Struct representing a plane in 3D space
    public struct Plane {
        public TVector normal; // Unit normal vector
        public float offset; // Distance from the plane to the origin

        // Creates a plane with the specified normal, containing the specified position
        public static Plane FromPositionNormal(TVector pos, TVector normal) {
            TVector uNorm = normal.Normalize();
            float dot = TVector.Dot(pos, uNorm);
            return new Plane {normal = uNorm, offset = dot};
        }

        public static double CheckEquality(Plane a, Plane b) {
            double magprodsq = a.normal.ComputeMagnitudeSquared()*b.normal.ComputeMagnitudeSquared();
                // although the magnitude of the normals SHOULD be 1... maybe somebody did something funky
            double dot = TVector.Dot(a.normal, b.normal);
            double aparallelness = 1.0 - Math.Sqrt((dot*dot)/magprodsq);
                // closer to parallel yields smaller values of this
            aparallelness *= aparallelness;

            float a_from_b = b.PointDistance(a.normal*a.offset);
            float b_from_a = a.PointDistance(b.normal*b.offset);
            TVector a_on_b = a.normal*(a.offset + a_from_b);
            TVector b_on_a = b.normal*(b.offset + b_from_a);
            double distsq1 = (a_on_b - b.normal*b.offset).ComputeMagnitudeSquared(); // coplanar --> same point
            double distsq2 = (b_on_a - a.normal*a.offset).ComputeMagnitudeSquared(); // coplanar --> same point

            return aparallelness + distsq1 + distsq1;
        }

        // Returns distance of the point from the plane
        // It's signed, so one side has negative values
        public float PointDistance(TVector point) {
            return TVector.Dot(normal, point) - offset;
        }

        // Creates a Plane object matching the plane of the specified triangle
        public static Plane FromTriangleVertices(TVector a, TVector b, TVector c) {
            TVector normal = TVector.Cross(b - a, c - a);
            return FromPositionNormal(a, normal);
        }

        // Static function to find the intersection of two planes
        // If they are parallel, returns false and outputs an invalid line struct
        // Otherwise, returns true and outputs the line of intersection
        public static bool Intersect(Plane a, Plane b, out Line result) {
            TVector cross = TVector.Cross(a.normal, b.normal);
            float magsq = cross.ComputeMagnitudeSquared();
            if (magsq == 0) {
                // failure! planes did not intersect, or planes were equal
                result = new Line { direction = TVector.Origin, origin = TVector.Origin }; // not a valid line!
                return false;
            }
            float invmag = (float) (1.0f/Math.Sqrt(magsq));
            TVector line_direction = cross*invmag;
            // using plane a to find intersection (also could try b?)
            TVector in_a_toward_edge = TVector.Cross(a.normal, line_direction).Normalize();
            TVector point_in_a = a.normal*a.offset;
            float dist = b.PointDistance(point_in_a);
            // seems this number could be either the positive or negative of what we want...
            float unsigned_r = dist*invmag;
            TVector positive = point_in_a + in_a_toward_edge*unsigned_r;
            TVector negative = point_in_a - in_a_toward_edge*unsigned_r;
            // figure out which one is actually at the intersection (or closest to it)
            double positive_check =
                new TVector(a.PointDistance(positive), b.PointDistance(positive)).ComputeMagnitudeSquared();
            double negative_check =
                new TVector(a.PointDistance(negative), b.PointDistance(negative)).ComputeMagnitudeSquared();
            // and use that one as a point on the line (for the out value)
            TVector point_on_line;
            if (positive_check < negative_check)
                point_on_line = positive;
            else
                point_on_line = negative;
            // success! planes intersectedx
            result = new Line {origin = point_on_line, direction = line_direction};
            return true;
        }
    }
}