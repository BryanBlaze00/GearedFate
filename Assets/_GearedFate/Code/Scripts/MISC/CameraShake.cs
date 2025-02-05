// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using Unity.Cinemachine;
    using UnityEngine;

    /// <summary>
    /// Adds Camera shake effect
    /// </summary>
    [RequireComponent(typeof(CinemachineBasicMultiChannelPerlin))]
    public class CameraShake : MonoBehaviour
    {
        private CinemachineBasicMultiChannelPerlin _noise;

        public void OnCamerShake()
        {
            StopAllCoroutines();
            ShakeCamera(0.6f, 2f, 50f);
        }

        public void ShakeCamera(float duration, float amplitude, float frequency)
        {
            if (_noise != null)
            {
                _noise.AmplitudeGain = amplitude;
                _noise.FrequencyGain = frequency;
            }

            StartCoroutine(StopShakeAfterDelay(duration));
        }

        private void Start()
        {
            _noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private System.Collections.IEnumerator StopShakeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_noise != null)
            {
                _noise.AmplitudeGain = 0f;
                _noise.FrequencyGain = 0f;
            }
        }
    }
}
