using System;
using System.Collections.Generic;
using System.Text;


namespace Modelthulhu.Geom
{
    using CommonTypes.DataTypes;
    using CommonTypes.TransportTypes;

    // Struct representing a line in 3D space
    public struct Line
    {
        public TVector origin;             // any point on the line
        public TVector direction;          // any nonzero vector along the line

        // Checks whether two lines are equal... returns a value, closer to zero = closer to being equal
        public static double CheckEquality(Line a, Line b)
        {
            float magprodsq = a.direction.ComputeMagnitudeSquared() * b.direction.ComputeMagnitudeSquared();
            float dot = TVector.Dot(a.direction, b.direction);
            float aparallelness = (float) (1.0 - Math.Sqrt((dot * dot) / magprodsq));                    // closer to parallel yields smaller values of this
            aparallelness *= aparallelness;

            Plane plane_a = Plane.FromPositionNormal(a.origin, a.direction);
            Plane plane_b = Plane.FromPositionNormal(b.origin, b.direction);
            float a_from_b = plane_b.PointDistance(a.origin);
            float b_from_a = plane_a.PointDistance(b.origin);
            TVector a_on_b = a.origin + plane_a.normal * a_from_b;
            TVector b_on_a = b.origin + plane_b.normal * b_from_a;
            float distsq1 = (a_on_b - b.origin).ComputeMagnitudeSquared();                     // colinear --> same point
            float distsq2 = (b_on_a - a.origin).ComputeMagnitudeSquared();                     // colinear --> same point
            return aparallelness + distsq1 + distsq2;                                           // sum of 3 squared quantities... anything big --> big result
        }

        // Finds the intersection of the line and plane, and returns true if there is one
        // If there is no intersection, or the line is entirely within the plane, it returns false and the output position is the origin of the line
        public static bool IntersectPlane(Line line, Plane plane, out TVector pos)
        {
            TVector dir = line.direction.Normalize();
            float dir_dot = TVector.Dot(ref dir, ref plane.normal);
            if (dir_dot == 0.0)
            {
                pos = line.origin;
                return false;
            }
            else
            {
                float origin_dot = TVector.Dot(ref line.origin, ref plane.normal);
                float tti = (plane.offset - origin_dot) / dir_dot;

                pos = line.origin + dir * tti;
                return true;
            }
        }
    }
}