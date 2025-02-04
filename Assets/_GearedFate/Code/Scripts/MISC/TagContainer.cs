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
		public List<Tag> GetTags => tags;
		
		public bool HasTag(Tag _tag) => tags.Contains(_tag);

		public bool HasTag(string _tag) => tags.Exists(t => t.Name.Equals(_tag, System.StringComparison.InvariantCultureIgnoreCase));

	}
}
