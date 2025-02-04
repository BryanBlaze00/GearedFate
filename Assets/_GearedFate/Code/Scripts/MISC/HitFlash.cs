//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using System.Collections;

namespace BTG
{
    /// <summary>
    /// HitFlash is a script that will flash the sprite of the object it is attached to red and then white.
    /// </summary>
    public class HitFlash : MonoBehaviour
    {
        [SerializeField] private Color redFlashColor = Color.red;
        [SerializeField] private Color whiteFlashColor = Color.white;
        [SerializeField] private float redFlashDuration = 0.1f;
        [SerializeField] private float whiteFlashDuration = 0.2f;

        private Material material;

        private void Awake()
        {
            this.material = this.GetComponent<SpriteRenderer>().material;
        }

        public void SetFlashColor(Color color)
        {
            this.redFlashColor = color;
        }

        public void HitFlashRoutine()
        {
            this.StartCoroutine(this._ChainRoutine());
        }

        private IEnumerator _ChainRoutine()
        {
            yield return this.StartCoroutine(this._FlashRoutine(this.redFlashColor, this.redFlashDuration));
            yield return this.StartCoroutine(this._FlashRoutine(this.whiteFlashColor, this.whiteFlashDuration));
        }

        private IEnumerator _FlashRoutine(Color color, float duration)
        {
            this.material.SetColor("_FlashColor", color);

            float currentFlashAmount;
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                currentFlashAmount = Mathf.Lerp(1, 0f, elapsedTime / duration);
                this.material.SetFloat("_FlashAmount", currentFlashAmount);
                yield return null;
            }
        }
    }
}
