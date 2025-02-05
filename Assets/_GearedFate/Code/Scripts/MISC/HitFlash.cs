// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// HitFlash is a script that will flash the sprite of the object it is attached to red and then white.
    /// </summary>
    public class HitFlash : MonoBehaviour
    {
        [SerializeField]
        private Color redFlashColor = Color.red;
        [SerializeField]
        private Color whiteFlashColor = Color.white;
        [SerializeField]
        private float redFlashDuration = 0.1f;
        [SerializeField]
        private float whiteFlashDuration = 0.2f;

        private Material material;

        private void Awake()
        {
            material = GetComponent<SpriteRenderer>().material;
        }

        public void SetFlashColor(Color color)
        {
            redFlashColor = color;
        }

        public void HitFlashRoutine()
        {
            StartCoroutine(ChainRoutine());
        }

        private IEnumerator ChainRoutine()
        {
            yield return StartCoroutine(FlashRoutine(redFlashColor, redFlashDuration));
            yield return StartCoroutine(FlashRoutine(whiteFlashColor, whiteFlashDuration));
        }

        private IEnumerator FlashRoutine(Color color, float duration)
        {
            material.SetColor("_FlashColor", color);

            float currentFlashAmount;
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                currentFlashAmount = Mathf.Lerp(1, 0f, elapsedTime / duration);
                material.SetFloat("_FlashAmount", currentFlashAmount);
                yield return null;
            }
        }
    }
}
