// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// Custom Tag
    /// </summary>
    [CreateAssetMenu(fileName = "newTag", menuName = "Data/Tag", order = 0)]
    public class Tag : ScriptableObject
    {
        public string Name => name;
    }
}
