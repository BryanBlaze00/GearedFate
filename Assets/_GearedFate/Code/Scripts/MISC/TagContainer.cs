// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Contains Custom Tag(s)
    /// </summary>
    public class TagContainer : MonoBehaviour
    {
        [SerializeField]
        private List<Tag> tags;

        public List<Tag> GetTags => tags;

        public bool HasTag(Tag tag)
        {
            return tags.Contains(tag);
        }

        public bool HasTag(string tag)
        {
            return tags.Exists(t => t.Name.Equals(tag, System.StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
