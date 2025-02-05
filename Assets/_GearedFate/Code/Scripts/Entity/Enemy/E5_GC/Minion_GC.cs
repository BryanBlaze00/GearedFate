// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Minion_GC
    /// </summary>
    public class Minion_GC : MonoBehaviour, IDataPersistence, ISaveableInstance
    {
        [FormerlySerializedAs("MoveSpeed")]
        [SerializeField]
        private float _moveSpeed;

        [FormerlySerializedAs("Health")]
        [SerializeField]
        private float _health;

        [SerializeField]
        private PooledObjectType _pooledObjectType;

        public PooledObjectType PooledObjectType => _pooledObjectType;

        public void LoadData(GameData data)
        {
            var minionData = data.GetNextMinionData();
            _health = minionData.Health;
            transform.position = minionData.Position;
        }

        public void SaveData(ref GameData data)
        {
            data.AddMinionData(new MinionSaveStruct(transform.position, _health));
        }
    }
}
