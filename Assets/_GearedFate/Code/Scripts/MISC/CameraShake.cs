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
        private CinemachineBasicMultiChannelPerlin noise;

        private void Start()
        {
            noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void OnCamerShake()
        {
            StopAllCoroutines();
            ShakeCamera(0.6f, 2f, 50f);
        }

        public void ShakeCamera(float duration, float amplitude, float frequency)
        {
            if (noise != null)
            {
                noise.AmplitudeGain = amplitude;
                noise.FrequencyGain = frequency;
            }

            StartCoroutine(StopShakeAfterDelay(duration));
        }

        private System.Collections.IEnumerator StopShakeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (noise != null)
            {
                noise.AmplitudeGain = 0f;
                noise.FrequencyGain = 0f;
            }
        }
    }
}
