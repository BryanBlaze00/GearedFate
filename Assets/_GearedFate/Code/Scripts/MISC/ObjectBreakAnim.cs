//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using UnityEngine;

/// <summary>
/// ObjectBreakAnim --- TODO: FIX ME
/// </summary>
namespace BTG
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ObjectBreakAnim : MonoBehaviour
    {
        [Header("Animation Settings")] [Tooltip("Speed of the animation")] [SerializeField]
        private float _animSpeed = 1.0f;

        [Tooltip("Number of sprites in the animation")] [SerializeField]
        private int _animSpritesCount = 7;

        [Tooltip("Index of the starting sprite in the animation sprite sheet")] [SerializeField]
        private int _animStartIndex = 0;

        private Sprite[] _sprites; // Array of sprites from the spritesheet
        private SpriteRenderer _objBreakRenderer;
        private bool isBreaking = false;

        private void Awake()
        {
            _objBreakRenderer = GetComponent<SpriteRenderer>();
            _sprites = new Sprite[_animSpritesCount];


            for (var i = 0; i < _animSpritesCount; i++) _sprites[i] = _objBreakRenderer.sprite;

            // Debug.Log("Sprites: " + _sprites.Length);
        }

        public void TriggerBreak()
        {
            if (!isBreaking)
            {
                isBreaking = true;
                StartCoroutine(Explode());
            }
        }

        private IEnumerator Explode()
        {
            for (var i = _animStartIndex; i < _animSpritesCount + _animStartIndex; i++)
            {
                // Debug.Log("_sprites.Length: " + _sprites.Length);
                // Debug.Log("AnimStartIndex: " + _animStartIndex);
                // Debug.Log("AnimSpritesCount: " + _animSpritesCount);
                // Debug.Log("i: " + i);
                // Debug.Log("Total: " + (_animSpritesCount + _animStartIndex));

                _objBreakRenderer.sprite = _sprites[i];
                yield return new WaitForSeconds(_animSpeed);
            }

            Destroy(gameObject);
        }
    }
}
