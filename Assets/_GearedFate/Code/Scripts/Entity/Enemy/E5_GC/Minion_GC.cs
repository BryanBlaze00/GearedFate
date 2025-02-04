//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Minion_GC
    /// </summary>
    public class Minion_GC : MonoBehaviour, IDataPersistence, ISaveableInstance
    {
        [SerializeField] private float MoveSpeed;
        [SerializeField] private float Health;

        [SerializeField] private PooledObjectType _pooledObjectType;

        public PooledObjectType PooledObjectType => this._pooledObjectType;

        private void Start()
        {
        }

        public void LoadData(GameData data)
        {
            var minionData = data.GetNextMinionData();
            this.Health = minionData.Health;
            this.transform.position = minionData.Position;
        }

        public void SaveData(ref GameData data)
        {
            data.MinionsData.Add(new MinionSaveStruct(this.transform.position, this.Health));
        }
    }
}
