// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    /// <summary>
    /// Stores minimum and maximum integers
    /// </summary>
    [System.Serializable]
    public class MinMaxInt
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public int Min;

        /// <summary>
        /// Maximum value
        /// </summary>
        public int Max;

        public MinMaxInt()
        {
            Min = 0;
            Max = 0;
        }

        /// <summary>
        /// Constructor taking in min and max
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public MinMaxInt(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}
