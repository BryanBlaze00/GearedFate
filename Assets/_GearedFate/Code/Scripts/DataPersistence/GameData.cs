namespace BTG
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GameData
    {
        [SerializeField]
        private SerializableDictionary<PooledObjectType, int> _assetsToInstantiateIds = new();

        // Incremented each time it's used, allows tracking which minion save struct is loaded next in MinionsData.
        private int _minionCurrentLoadedIndex;

        public PlayerSaveStruct PlayerData;

        // Contains save data for each minion that need to be reloaded.
        public List<MinionSaveStruct> MinionsData = new();

        /// <summary>
        /// Use the key to know which asset to load, and the value to know how much.
        /// </summary>
        public ICollection<KeyValuePair<PooledObjectType, int>> AssetToInstantiates => _assetsToInstantiateIds;

        public GameData()
        {
            // Starting values for player on new game
            PlayerData = new PlayerSaveStruct(Vector3.zero, 100, 100);
        }

        /// <summary>
        /// Register one item that will need to be loaded, thanks to its PooledObjectType.
        /// </summary>
        public void AddAssetIndexToInstantiate(PooledObjectType id)
        {
            _assetsToInstantiateIds.TryGetValue(id, out var currentCount);
            _assetsToInstantiateIds[id] = currentCount + 1;
        }

        /// <summary>
        /// Return useful values to load saved minions. Never return twice the same.
        /// </summary>
        public MinionSaveStruct GetNextMinionData()
        {
            var data = MinionsData[_minionCurrentLoadedIndex];
            _minionCurrentLoadedIndex++;
            return data;
        }

        /// <summary>
        /// Call this right after the game is fully loaded.
        /// </summary>
        public void CleanAfterLoad()
        {
            _assetsToInstantiateIds.Clear();
            MinionsData.Clear();
        }
    }
}
