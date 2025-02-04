using System.Collections.Generic;
using UnityEngine;

namespace BTG
{
    // Create a Dictionary. The Unity serializer doesn't support Dictionary types.
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new();

        [SerializeField] private List<TValue> _values = new();

        public void OnBeforeSerialize()
        {
            this._keys.Clear();
            this._values.Clear();

            // For each key/value pair in the dictionary, add the key to the keys list and the value to the values list
            foreach (var pair in this)
            {
                this._keys.Add(pair.Key);
                this._values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            // Loop through the list of keys and values and add each key/value pair to the dictionary
            for (var i = 0; i < this._keys.Count; i++)
            {
                this.Add(this._keys[i], this._values[i]);
            }
        }
    }
}
