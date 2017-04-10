using System;

namespace LIFE.Components.ESC.SpatialAPI.Entities.Transformation
{
    /// <summary>
    ///   Direction class, holds pitch and yaw value and provides some conversions.
    /// </summary>
    public class Direction
    {
        /// <summary>
        ///   Direction (lateral axis).
        /// </summary>
        public double Pitch { get; private set; }


        /// <summary>
        ///   Direction (vertical axis).
        /// </summary>
        public double Yaw { get; private set; }


        /// <summary>
        ///   Set the agent's pitch value [-90° ≤ pitch ≤ 90°].
        /// </summary>
        /// <param name="pitch">New pitch value.</param>
        public void SetPitch(double pitch)
        {
            if (pitch > 90) pitch = 90;
            if (pitch < -90) pitch = -90;
            Pitch = pitch;
        }


        /// <summary>
        ///   Set the agent's orientation (compass heading, [0° ≤ yaw lt. 360°].
        /// </summary>
        /// <param name="yaw">New heading.</param>
        public void SetYaw(double yaw)
        {
            yaw %= 360;
            if (yaw < 0) yaw += 360;
            Yaw = yaw;
        }


        /// <summary>
        ///   Create a directional vector based on pitch and yaw values.
        /// </summary>
        /// <returns>A directional vector as structure.</returns>
        public Vector3 GetDirectionalVector()
        {
            var pitchRad = DegToRad(Pitch);
            var yawRad = DegToRad(Yaw);
            return new Vector3
            (Math.Cos(pitchRad) * Math.Sin(yawRad),
                Math.Cos(pitchRad) * Math.Cos(yawRad),
                Math.Sin(pitchRad)).Normalize();
        }


        /// <summary>
        ///   Use a directional vector to create pitch and yaw values.
        /// </summary>
        /// <param name="vector">The vector to set.</param>
        public void SetDirectionalVector(Vector3 vector)
        {
            SetPitch(RadToDeg(Math.Asin(vector.Y / vector.Length))); // Y is height above ground
            var yaw = Yaw;

            // Check 90° and 270° (arctan infinite) first.      
            if (Math.Abs(vector.Z) <= double.Epsilon)
            {
                if (vector.X > 0f) yaw = 90f;
                else if (vector.X < 0f) yaw = 270f;
            }

            // Arctan argument fine? Calculate heading then.    
            else
            {
                yaw = RadToDeg(Math.Atan(vector.X / -vector.Z));
                if (vector.Z < 0f) yaw += 180f; // Range  90° to 270° correction. 
                if (yaw < 0f) yaw += 360f; // Range 270° to 360° correction.        
            }
            SetYaw(yaw);
        }


        /// <summary>
        ///   Degree → radians conversion (π/180).
        /// </summary>
        /// <param name="angle">Angle in degree.</param>
        /// <returns>Angle in radians.</returns>
        public static double DegToRad(double angle)
        {
            return angle * 0.0174532925f;
        }


        /// <summary>
        ///   Radians → degree conversion (180/π).
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Angle in degree.</returns>
        public static double RadToDeg(double angle)
        {
            return angle * 57.295779f;
        }

        protected bool Equals(Direction other)
        {
            return Pitch.Equals(other.Pitch) && Yaw.Equals(other.Yaw);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Direction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Pitch.GetHashCode() * 397) ^ Yaw.GetHashCode();
            }
        }
    }
}