//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    /// <summary>
    /// ParallaxScroller is used to create a parallax effect on a RawImage.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class ParallaxScroller : MonoBehaviour
    {
        [Header("Parallax Settings")] [SerializeField]
        private RawImage _img;

        [SerializeField] private float _x, _y;

        private void Update()
        {
            this.ScrollingParallax();
        }

        private void ScrollingParallax()
        {
            this._img.uvRect = new Rect(this._img.uvRect.position + new Vector2(this._x, this._y) * Time.deltaTime, this._img.uvRect.size);
        }
    }
}
