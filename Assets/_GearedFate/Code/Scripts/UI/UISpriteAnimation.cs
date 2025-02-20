// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// UISpriteAnimation class to manage UI sprite animations.
    /// </summary>
    public class UISpriteAnimation : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Sprite[] _spriteArray;
        [SerializeField]
        private float _speed = .02f;

        private Coroutine _coroutineAnim;
        private int _indexSprite;
        private bool _isDone;

        public float AnimationSpeed
        {
            get => _speed;
            private set => _speed = Mathf.Clamp(value, 0.001f, 1f);
        }

        /// <summary>
        /// Start Looping Animation
        /// </summary>
        public void PlayUIAnim()
        {
            _isDone = false;
            _coroutineAnim = StartCoroutine(PlayAnimUI());
        }

        /// <summary>
        /// Stop Looping Animation
        /// </summary>
        public void StopUIAnim()
        {
            _isDone = true;
            StopCoroutine(_coroutineAnim);
        }

        /// <summary>
        /// True/False option to reset the sprite to default image.
        /// </summary>
        /// <param name="option"> True/False. </param>
        public void PlayOnce(bool option)
        {
            StartCoroutine(PlayAnimOnce(option));
        }

        private void OnEnable()
        {
            if (GameManager.Instance.GetCurrentScene() == "MainMenu")
            {
                PlayUIAnim();
            }
        }

        private IEnumerator PlayAnimOnce(bool option)
        {
            for (var i = 0; i < _spriteArray.Length; i++)
            {
                _image.sprite = _spriteArray[i];
                yield return new WaitForSeconds(_speed);
            }

            // Optional: Reset sprite after animation completes
            if (option)
            {
                _image.sprite = _spriteArray[0];
            }
        }

        private IEnumerator PlayAnimUI()
        {
            yield return new WaitForSeconds(_speed);
            if (_indexSprite >= _spriteArray.Length)
            {
                _indexSprite = 0;
            }

            _image.sprite = _spriteArray[_indexSprite];
            _indexSprite += 1;
            if (_isDone == false)
            {
                _coroutineAnim = StartCoroutine(PlayAnimUI());
            }
        }
    }
}
