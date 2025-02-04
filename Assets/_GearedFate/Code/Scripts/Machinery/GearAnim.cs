//
// Copyright (c) BTG. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GearAnim
    ///
    /// This script is used to animate the gears in the game.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class GearAnim : MonoBehaviour
    {
        [SerializeField] private GearSpritesData _spritesData;

        /// Time in seconds before the animation updates. The shorter the faster.
        [SerializeField] [Range(0, 3)] private float _rotateSpeed = 1.0f;

        /// Index of the starting rotating sprite in the animation sprite sheet.
        [SerializeField] private int _startingSpriteIndex;

        /// Number of sprites to achieve 1/8th of a turn, before animation loop. Big and medium gears should have 6, small only 4.
        [SerializeField] private int _rotationNumbers;

        private float _previousUpdateTime;

        private SpriteRenderer _gearRenderer;

        private int _rotateStep;

        /// <summary>
        /// Event invoked each time the gear moves.
        /// </summary>
        public event Action OnGearMoved;

        /// <summary>
        /// Since an animation loop does 1/8th of a turn,
        /// a full circle rotation is 8 times the rotation numbers to complete the animation loop.
        /// </summary>
        public int FullCircleRotationNumbers => 8 * this._rotationNumbers;

        protected void Start()
        {
            this._previousUpdateTime = Time.time;
            this._gearRenderer = this.GetComponent<SpriteRenderer>();
        }

        protected void Update()
        {
            if (this._previousUpdateTime + this._rotateSpeed > Time.time) return;

            this._previousUpdateTime = Time.time;

            this._rotateStep = this._startingSpriteIndex + (this._rotateStep + 1) % this._rotationNumbers;

            if (this._spritesData.TryGetSpriteAtIndex(this._rotateStep, out var gearSprite))
            {
                this._gearRenderer.sprite = gearSprite;
                this.OnGearMoved?.Invoke();
            }
        }
    }
}
