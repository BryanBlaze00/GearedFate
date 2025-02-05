// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// ParallaxScroller is used to create a parallax effect on a RawImage.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class ParallaxScroller : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [SerializeField]
        private RawImage _img;

        [SerializeField]
        private float _x, _y;

        private void Update()
        {
            ScrollingParallax();
        }

        private void ScrollingParallax()
        {
            _img.uvRect = new Rect(_img.uvRect.position + (new Vector2(_x, _y) * Time.deltaTime), _img.uvRect.size);
        }
    }
}
