// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    /// <summary>
    /// Stores minimum and maximum floating point values
    /// </summary>
    [System.Serializable]
    public class MinMaxFloat
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public float Min { get; private set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public float Max { get; private set; }

        public MinMaxFloat()
        {
            Min = 0;
            Max = 0;
        }

        /// <summary>
        /// Constructor taking in min and max
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public MinMaxFloat(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
