using System;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Structure used to save all data related to the main character.
    /// </summary>
    [Serializable]
    public struct PlayerSaveStruct
    {
        [SerializeField] private Vector3 _position;

        [SerializeField] private float _fuel;

        [SerializeField] private float _health;

        public Vector3 Position => this._position;

        public float Fuel => this._fuel;

        public float Health => this._health;

        public PlayerSaveStruct(Vector3 position, float fuel, float health)
        {
            this._position = position;
            this._fuel = fuel;
            this._health = health;
        }
    }
}
