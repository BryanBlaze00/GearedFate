// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// FloatAnimation class to provide floating animation to the object.
    /// </summary>
    public class FloatAnimation : MonoBehaviour
    {
        [FormerlySerializedAs("amplitude")]
        [SerializeField]
        private float _amplitude = 0.1f; // How high/low the object should float

        [FormerlySerializedAs("frequency")]
        [SerializeField]
        private float _frequency = 1f; // How fast the object should float

        [FormerlySerializedAs("smoothTime")]
        [SerializeField]
        private float _smoothTime = 0.1f; // Smoothness of the float animation

        private Vector3 _startPosition;
        private float _currentVelocityY;
        private float _startTimeOffset;

        private void Start()
        {
            _startPosition = transform.localPosition;
            _startTimeOffset = RandomUtility.RandomFloat(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            var targetY = _startPosition.y + (Mathf.Sin((Time.time * _frequency) + _startTimeOffset) * _amplitude);
            var currentY = transform.localPosition.y;

            var smoothedY = Mathf.SmoothDamp(currentY, targetY, ref _currentVelocityY, _smoothTime);

            transform.localPosition = new Vector3(transform.localPosition.x, smoothedY, transform.localPosition.z);
        }
    }
}
