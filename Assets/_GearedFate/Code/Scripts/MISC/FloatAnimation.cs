//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// FloatAnimation class to provide floating animation to the object.
    /// </summary>
    public class FloatAnimation : MonoBehaviour
    {
        public float amplitude = 0.1f; // How high/low the object should float
        public float frequency = 1f; // How fast the object should float
        public float smoothTime = 0.1f; // Smoothness of the float animation

        private Vector3 startPosition;
        private float currentVelocityY;
        private float startTimeOffset;

        private void Start()
        {
            startPosition = transform.localPosition;
            startTimeOffset = RandomUtilily.RandomFloat(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            var targetY = startPosition.y + Mathf.Sin(Time.time * frequency + startTimeOffset) * amplitude;
            var currentY = transform.localPosition.y;

            var smoothedY = Mathf.SmoothDamp(currentY, targetY, ref currentVelocityY, smoothTime);

            transform.localPosition = new Vector3(transform.localPosition.x, smoothedY, transform.localPosition.z);
        }
    }
}
