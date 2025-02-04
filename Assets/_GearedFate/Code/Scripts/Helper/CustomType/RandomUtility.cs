// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// RandomUtility class to provide random utility functions. 
    /// </summary>
    public static class RandomUtilily
    {
        /// <summary>
        /// Returns a random integer between min [inclusive] and max [inclusive].
        /// </summary>
        public static int RandomInt(int min, int max)
        {
            return Random.Range(min, max + 1); // +1 to include max in the range
        }

        /// <summary>
        /// Returns a random float between min [inclusive] and max [exclusive].
        /// </summary>
        public static float RandomFloat(float min, float max)
        {
            return Random.Range(min, max);
        }

        /// <summary>
        /// Returns a random boolean value based on the given percentage chance.
        /// </summary>
        public static bool Chance(float percentage)
        {
            return Random.value <= percentage / 100f;
        }

        /// <summary>
        /// Returns a random direction in 2D space.
        /// </summary>
        public static Vector2 RandomDirection2D()
        {
            return Random.insideUnitCircle.normalized;
        }

        /// <summary>
        /// Returns a random element from the given array.
        /// </summary>
        public static T GetRandomElement<T>(T[] array)
        {
            if (array.Length == 0)
            {
                Debug.LogError("Array is empty.");
                return default;
            }

            var randomIndex = Random.Range(0, array.Length);
            return array[randomIndex];
        }

        /// <summary>
        /// Returns a random point in a circle with the given center and radius.
        /// </summary>
        public static Vector2 RandomPointInCircle(Vector2 center, float radius)
        {
            var randomPoint = Random.insideUnitCircle * radius;
            return center + randomPoint;
        }

        /// <summary>
        /// Returns a random point on the edge of a circle with the given center and radius.
        /// </summary>
        public static Vector2 RandomPointOnCircleEdge(Vector2 center, float radius)
        {
            return center + (Random.insideUnitCircle.normalized * radius);
        }
    }
}
