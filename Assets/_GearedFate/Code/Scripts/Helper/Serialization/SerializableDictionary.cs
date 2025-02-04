using System.Collections.Generic;
using UnityEngine;

namespace BTG
{
    // Create a Dictionary. The Unity serializer doesn't support Dictionary types.
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> _keys = new();

        [SerializeField]
        private List<TValue> _values = new();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            // For each key/value pair in the dictionary, add the key to the keys list and the value to the values list
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            // Loop through the list of keys and values and add each key/value pair to the dictionary
            for (int i = 0; i < _keys.Count; i++)
            {
                Add(_keys[i], _values[i]);
            }
        }
    }
}
