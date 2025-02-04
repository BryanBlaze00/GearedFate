using System;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Structure used to save all data related to the main character.
    /// </summary>
    [Serializable]
    public struct MinionSaveStruct
    {
        [SerializeField]
        private Vector3 _position;

        [SerializeField]
        private float _health;

        public Vector3 Position => _position;

        public float Health => _health;

        public MinionSaveStruct(Vector3 position, float health)
        {
            _position = position;
            _health = health;
        }
    }
}
