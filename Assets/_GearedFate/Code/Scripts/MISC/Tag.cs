//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Custom Tag
    /// </summary>
    [CreateAssetMenu(fileName = "newTag", menuName = "Data/Tag", order = 0)]
    public class Tag : ScriptableObject
    {
        public string Name => this.name;
    }
}
