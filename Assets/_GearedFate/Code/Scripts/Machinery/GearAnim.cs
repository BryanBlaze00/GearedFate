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
      [SerializeField]
      private GearSpritesData _spritesData;

      /// Time in seconds before the animation updates. The shorter the faster.
      [SerializeField] [Range(0, 3)]
      private float _rotateSpeed = 1.0f;

      /// Index of the starting rotating sprite in the animation sprite sheet.
      [SerializeField]
      private int _startingSpriteIndex;

      /// Number of sprites to achieve 1/8th of a turn, before animation loop. Big and medium gears should have 6, small only 4.
      [SerializeField]
      private int _rotationNumbers;

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
      public int FullCircleRotationNumbers => 8*_rotationNumbers;

      protected void Start()
      {
         _previousUpdateTime = Time.time;
         _gearRenderer = GetComponent<SpriteRenderer>();
      }

      protected void Update()
      {
         if (_previousUpdateTime + _rotateSpeed > Time.time)
         {
            return;
         }

         _previousUpdateTime = Time.time;

         _rotateStep = _startingSpriteIndex + ((_rotateStep + 1) % _rotationNumbers);

         if (_spritesData.TryGetSpriteAtIndex(_rotateStep, out Sprite gearSprite))
         {
            _gearRenderer.sprite = gearSprite;
            OnGearMoved?.Invoke();
         }
      }
   }
}
