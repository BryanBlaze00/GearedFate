//
// Copyright (c) Dayen Creation. All rights reserved.
//

using UnityEngine;

namespace DayenCreation
{
	/// <summary>
	/// Extension Methods for UnityEngine.Transform
	/// </summary>
	public static class TransformExtensions 
   {
		public static void ResetTransformation(this Transform transform)
		{
			transform.position = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}
	}
}