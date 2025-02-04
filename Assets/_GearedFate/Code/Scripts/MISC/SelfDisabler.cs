// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// SelfDisabler
    /// </summary>
    public class SelfDisabler : MonoBehaviour
    {
        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }
    }
}
