//
// Copyright (c) BTG. All rights reserved.
//


using System.Collections.Generic;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Contains Custom Tag(s)
    /// </summary>
    public class TagContainer : MonoBehaviour
    {
        [SerializeField] private List<Tag> tags;
        public List<Tag> GetTags => this.tags;

        public bool HasTag(Tag _tag)
        {
            return this.tags.Contains(_tag);
        }

        public bool HasTag(string _tag)
        {
            return this.tags.Exists(t => t.Name.Equals(_tag, System.StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
