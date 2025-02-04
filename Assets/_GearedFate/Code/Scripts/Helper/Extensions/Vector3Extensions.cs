//
// Copyright (c) Dayen Creation. All rights reserved.
//

using UnityEngine;

namespace DayenCreation
{
    /// <summary>
    /// Extension Methods for UnityEngine.Vector3
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Inverts a scale Vector3 by dividing 1 by each component
        /// </summary>
        public static void InvertSelf(this ref Vector3 vec)
        {
            vec.x = 1 / vec.x;
            vec.y = 1 / vec.y;
            vec.z = 1 / vec.z;
        }

        /// <summary>
        /// Returns a inverted Vector3 by dividing 1 by each component
        /// </summary>
        public static Vector3 Invert(this Vector3 vec)
        {
            vec.InvertSelf();
            return vec;
        }

        /// <summary>
        /// Sets absolute value of each component of a Vector3
        /// </summary>
        public static void AbsSelf(this ref Vector3 vec)
        {
            vec.x = Mathf.Abs(vec.x);
            vec.y = Mathf.Abs(vec.y);
            vec.z = Mathf.Abs(vec.z);
        }

        /// <summary>
        /// Returns a Vector3 with absolute value of each component
        /// </summary>
        public static Vector3 Abs(this Vector3 vec)
        {
            vec.AbsSelf();
            return vec;
        }

        /// <summary>
        /// Sets each component of a Vector3 with sign of them
        /// </summary>
        public static void SignSelf(this ref Vector3 vec)
        {
            vec.x = Mathf.Sign(vec.x);
            vec.y = Mathf.Sign(vec.y);
            vec.z = Mathf.Sign(vec.z);
        }

        /// <summary>
        /// Returns a Vector3 with sign of each component
        /// </summary>
        public static Vector3 Sign(this Vector3 vec)
        {
            vec.SignSelf();
            return vec;
        }

        /// <summary>
        /// Divides itself by another Vector3
        /// </summary>
        /// <param name="divisor">The Vector3 by which this Vector3 is to be divided</param>
        public static void DivideSelf(this ref Vector3 vec, Vector3 divisor)
        {
            vec = Vector3.Scale(vec, divisor.Invert());
        }

        /// <summary>
        /// Returns a Vector3 with the division by dividing itself by another Vector3
        /// </summary>
        /// <param name="divisor">The Vector3 by which this Vector3 is to be divided</param>
        public static Vector3 Divide(this Vector3 vec, Vector3 divisor)
        {
            return Vector3.Scale(vec, divisor.Invert());
        }


        /// <summary>
        /// Adds to any x y values of a Vector3
        /// </summary>
        public static void AddSelf(this ref Vector3 vec, float x = 0, float y = 0, float z = 0)
        {
            vec.x += x;
            vec.y += y;
            vec.z += z;
        }

        /// <summary>
        /// Adds to any x y values of a Vector3 and returns a new one
        /// </summary>
        public static Vector3 Add(this Vector3 vec, float x = 0, float y = 0, float z = 0)
        {
            vec.AddSelf(x, y, z);
            return vec;
        }

        /// <summary>
        /// Sets any x y values of a Vector3
        /// </summary>
        public static void WithSelf(this ref Vector3 vec, float? x = null, float? y = null, float? z = null)
        {
            vec.x = x ?? vec.x;
            vec.y = y ?? vec.y;
            vec.z = z ?? vec.z;
        }

        /// <summary>
        /// Sets any x y values of a Vector3
        /// </summary>
        public static Vector3 With(this Vector3 vec, float? x = null, float? y = null, float? z = null)
        {
            vec.WithSelf(x, y, z);
            return vec;
        }

        /// <summary>
        /// Returns a Boolean indicating whether the current Vector3 is in a given range from another Vector3
        /// </summary>
        /// <param name="current">The current Vector3 position</param>
        /// <param name="target">The Vector3 position to compare against</param>
        /// <param name="range">The range value to compare against</param>
        /// <returns>True if the current Vector3 is in the given range from the target Vector3, false otherwise</returns>
        public static bool InRangeOf(this Vector3 current, Vector3 target, float range)
        {
            return (current - target).sqrMagnitude <= range * range;
        }

        /// <summary>
        /// Computes a random point in an annulus (a ring-shaped area) based on minimum and 
        /// maximum radius values around a central Vector3 point (origin).
        /// </summary>
        /// <param name="origin">The center Vector3 point of the annulus.</param>
        /// <param name="minRadius">Minimum radius of the annulus.</param>
        /// <param name="maxRadius">Maximum radius of the annulus.</param>
        /// <returns>A random Vector3 point within the specified annulus.</returns>
        public static Vector3 RandomPointInAnnulus(this Vector3 origin, float minRadius, float maxRadius)
        {
            var angle = Random.value * Mathf.PI * 2f;
            Vector3 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));

            // Squaring and then square-rooting radii to ensure uniform distribution within the annulus
            var minRadiusSquared = minRadius * minRadius;
            var maxRadiusSquared = maxRadius * maxRadius;
            var distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);

            // Calculate the position vector
            var position = direction * distance;
            return origin + position;
        }
    }
}
