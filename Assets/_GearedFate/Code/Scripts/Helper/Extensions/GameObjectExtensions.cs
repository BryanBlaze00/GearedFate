// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// GameobjectExtensions
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// returns true if the gameobject contains a certain tag
        /// </summary>
        /// <param name="tag">The tag that needs to be checked if the gameobject contains it or not</param>
        /// <returns></returns>
        public static bool HasTag(this GameObject gameObject, Tag tag)
        {
            return gameObject.TryGetComponent<TagContainer>(out var tagContainer) && tagContainer.HasTag(tag);
        }

        /// <summary>
        /// returns true if the gameobject contains a certain tag passed as string
        /// </summary>
        /// <param name="tag">The tag name tjat needs to be checked if the gameobject contains it or not</param>
        /// <returns></returns>
        public static bool HasTag(this GameObject gameObject, string tag)
        {
            return gameObject.TryGetComponent<TagContainer>(out var tagContainer) && tagContainer.HasTag(tag);
        }

        public static async void SetInactive(this GameObject gameObject, float delay)
        {
            await Task.Delay((int)(delay * 1000));
            gameObject.SetActive(false);
        }
    }
}
