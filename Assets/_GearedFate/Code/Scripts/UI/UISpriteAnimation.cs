//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    /// <summary>
    /// UISpriteAnimation class to manage UI sprite animations.
    /// </summary>
    public class UISpriteAnimation : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _spriteArray;
        [SerializeField] private float _speed = .02f;

        public float AnimationSpeed
        {
            get => this._speed;
            private set => this._speed = Mathf.Clamp(value, 0.001f, 1f);
        }

        private Coroutine _coroutineAnim;
        private int _indexSprite;
        private bool IsDone;

        private void OnEnable()
        {
            if (GameManager.Instance.GetCurrentScene() == "MainMenu")
            {
                this.PlayUIAnim();
            }
        }

        /// <summary>
        /// Start Looping Animation
        /// </summary>
        public void PlayUIAnim()
        {
            this.IsDone = false;
            this._coroutineAnim = this.StartCoroutine(this.PlayAnimUI());
        }

        /// <summary>
        /// Stop Looping Animation
        /// </summary>
        public void StopUIAnim()
        {
            this.IsDone = true;
            this.StopCoroutine(this._coroutineAnim);
        }

        /// <summary>
        /// Option to reset the sprite.
        /// </summary>
        /// <param name="option"></param>
        public void PlayOnce(bool option)
        {
            this.StartCoroutine(this.PlayAnimOnce(option));
        }

        private IEnumerator PlayAnimOnce(bool option)
        {
            for (var i = 0; i < this._spriteArray.Length; i++)
            {
                this._image.sprite = this._spriteArray[i];
                yield return new WaitForSeconds(this._speed);
            }

            if (option)
                // Optional: Reset sprite after animation completes
            {
                this._image.sprite = this._spriteArray[0];
            }
        }

        /// <summary>
        /// Play the UI animation in a loop.
        /// </summary>
        /// <returns> Loops itself. </returns>
        private IEnumerator PlayAnimUI()
        {
            yield return new WaitForSeconds(this._speed);
            if (this._indexSprite >= this._spriteArray.Length)
            {
                this._indexSprite = 0;
            }

            this._image.sprite = this._spriteArray[this._indexSprite];
            this._indexSprite += 1;
            if (this.IsDone == false)
            {
                this._coroutineAnim = this.StartCoroutine(this.PlayAnimUI());
            }
        }
    }
}
