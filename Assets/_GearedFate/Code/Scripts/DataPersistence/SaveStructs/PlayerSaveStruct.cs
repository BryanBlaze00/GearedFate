namespace BTG
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Structure used to save all data related to the main character.
    /// </summary>
    [Serializable]
    public struct PlayerSaveStruct
    {
        [SerializeField]
        private Vector3 _position;

        [SerializeField]
        private float _fuel;

        [SerializeField]
        private float _health;

        public Vector3 Position => _position;

        public float Fuel => _fuel;

        public float Health => _health;

        public PlayerSaveStruct(Vector3 position, float fuel, float health)
        {
            _position = position;
            _fuel = fuel;
            _health = health;
        }
    }
}
