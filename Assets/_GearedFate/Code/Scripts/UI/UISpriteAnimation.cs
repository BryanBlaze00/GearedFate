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
            get => _speed;
            private set => _speed = Mathf.Clamp(value, 0.001f, 1f);
        }

        private Coroutine _coroutineAnim;
        private int _indexSprite;
        private bool IsDone;

        private void OnEnable()
        {
            if (GameManager.Instance.GetCurrentScene() == "MainMenu") PlayUIAnim();
        }

        /// <summary>
        /// Start Looping Animation
        /// </summary>
        public void PlayUIAnim()
        {
            IsDone = false;
            _coroutineAnim = StartCoroutine(PlayAnimUI());
        }

        /// <summary>
        /// Stop Looping Animation
        /// </summary>
        public void StopUIAnim()
        {
            IsDone = true;
            StopCoroutine(_coroutineAnim);
        }

        /// <summary>
        /// Option to reset the sprite.
        /// </summary>
        /// <param name="option"></param>
        public void PlayOnce(bool option)
        {
            StartCoroutine(PlayAnimOnce(option));
        }

        private IEnumerator PlayAnimOnce(bool option)
        {
            for (var i = 0; i < _spriteArray.Length; i++)
            {
                _image.sprite = _spriteArray[i];
                yield return new WaitForSeconds(_speed);
            }

            if (option)
                // Optional: Reset sprite after animation completes
                _image.sprite = _spriteArray[0];
        }

        /// <summary>
        /// Play the UI animation in a loop.
        /// </summary>
        /// <returns> Loops itself. </returns>
        private IEnumerator PlayAnimUI()
        {
            yield return new WaitForSeconds(_speed);
            if (_indexSprite >= _spriteArray.Length) _indexSprite = 0;
            _image.sprite = _spriteArray[_indexSprite];
            _indexSprite += 1;
            if (IsDone == false)
                _coroutineAnim = StartCoroutine(PlayAnimUI());
        }
    }
}
