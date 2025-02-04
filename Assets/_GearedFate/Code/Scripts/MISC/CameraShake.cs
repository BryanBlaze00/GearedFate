//
// Copyright (c) BTG. All rights reserved.
//

using Unity.Cinemachine;
using UnityEngine;


namespace BTG
{
    /// <summary>
    /// Adds Camera shake effect
    /// </summary>
    [RequireComponent(typeof(CinemachineBasicMultiChannelPerlin))]
    public class CameraShake : MonoBehaviour
    {
        private CinemachineBasicMultiChannelPerlin noise;

        private void Start()
        {
            this.noise = this.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void OnCamerShake()
        {
            this.StopAllCoroutines();
            this.ShakeCamera(0.6f, 2f, 50f);
        }

        public void ShakeCamera(float duration, float amplitude, float frequency)
        {
            if (this.noise != null)
            {
                this.noise.AmplitudeGain = amplitude;
                this.noise.FrequencyGain = frequency;
            }

            this.StartCoroutine(this.StopShakeAfterDelay(duration));
        }

        private System.Collections.IEnumerator StopShakeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (this.noise != null)
            {
                this.noise.AmplitudeGain = 0f;
                this.noise.FrequencyGain = 0f;
            }
        }
    }
}
