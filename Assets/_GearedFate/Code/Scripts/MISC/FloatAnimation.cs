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
            this.startPosition = this.transform.localPosition;
            this.startTimeOffset = RandomUtilily.RandomFloat(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            var targetY = this.startPosition.y + Mathf.Sin(Time.time * this.frequency + this.startTimeOffset) * this.amplitude;
            var currentY = this.transform.localPosition.y;

            var smoothedY = Mathf.SmoothDamp(currentY, targetY, ref this.currentVelocityY, this.smoothTime);

            this.transform.localPosition = new Vector3(this.transform.localPosition.x, smoothedY, this.transform.localPosition.z);
        }
    }
}
