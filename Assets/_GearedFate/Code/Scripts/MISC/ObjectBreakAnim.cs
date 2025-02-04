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
            this._objBreakRenderer = this.GetComponent<SpriteRenderer>();
            this._sprites = new Sprite[this._animSpritesCount];


            for (var i = 0; i < this._animSpritesCount; i++)
            {
                this._sprites[i] = this._objBreakRenderer.sprite;
            }

            // Debug.Log("Sprites: " + _sprites.Length);
        }

        public void TriggerBreak()
        {
            if (!this.isBreaking)
            {
                this.isBreaking = true;
                this.StartCoroutine(this.Explode());
            }
        }

        private IEnumerator Explode()
        {
            for (var i = this._animStartIndex; i < this._animSpritesCount + this._animStartIndex; i++)
            {
                // Debug.Log("_sprites.Length: " + _sprites.Length);
                // Debug.Log("AnimStartIndex: " + _animStartIndex);
                // Debug.Log("AnimSpritesCount: " + _animSpritesCount);
                // Debug.Log("i: " + i);
                // Debug.Log("Total: " + (_animSpritesCount + _animStartIndex));

                this._objBreakRenderer.sprite = this._sprites[i];
                yield return new WaitForSeconds(this._animSpeed);
            }

            Destroy(this.gameObject);
        }
    }
}
