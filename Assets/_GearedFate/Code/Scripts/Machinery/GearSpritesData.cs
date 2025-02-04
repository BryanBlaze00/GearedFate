#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BTG
{
    using System.Linq;

    /// <summary>
    /// Simple Scriptable object script to allow storing data about gear sprites in a single location.
    /// </summary>
    [CreateAssetMenu(fileName = "GearSpritesData", menuName = "Scriptable Objects/GearSpritesData")]
    public class GearSpritesData : ScriptableObject
    {
        // Sprite sheet with the gear animations, sprites should be name with a number at the end.
        [SerializeField]
        private Texture2D _gearSpriteSheet;

        private Sprite[] _gearSprites;

        /// <summary>
        /// Simply returns the sprite in the sprite sheet at the provided index, if possible.
        /// The index should correspond to the numeric portion of the sprite name (e.g., 10 -> x_10)
        /// </summary>
        public bool TryGetSpriteAtIndex(int index, out Sprite sprite)
        {
            sprite = null;

            if (index < _gearSprites.Length && index >= 0)
            {
                sprite = _gearSprites[index];
                return true;
            }

            return false;
        }

        protected void OnValidate()
        {
#if UNITY_EDITOR
            // Load all sprites from the sprite sheet and order them by the end number in the name.
            _gearSprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(_gearSpriteSheet))
                .OfType<Sprite>()
                .OrderBy(sprite => ExtractNumberFromName(sprite.name))
                .ToArray();
#endif
        }

        private int ExtractNumberFromName(string name)
        {
            // Find the first numeric portion of the name (e.g., "x_10" -> 10)
            var numbers = new string(name.Where(char.IsDigit).ToArray());
            return int.TryParse(numbers, out var result) ? result : 0;
        }
    }
}
