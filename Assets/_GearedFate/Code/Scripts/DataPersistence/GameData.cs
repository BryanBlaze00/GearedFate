namespace BTG
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GameData
    {
        [SerializeField]
        private SerializableDictionary<PooledObjectType, int> _assetsToInstantiateIds = new ();

        // Incremented each time it's used, allows tracking which minion save struct is loaded next in MinionsData.
        private int _minionCurrentLoadedIndex;

        private PlayerSaveStruct _playerData;

        // Contains save data for each minion that need to be reloaded.
        private List<MinionSaveStruct> _minionsData = new ();

        public GameData()
        {
            // Starting values for player on new game
            _playerData = new PlayerSaveStruct(Vector3.zero, 100, 100);
        }

        public PlayerSaveStruct PlayerData => _playerData;


        /// <summary>
        /// Use the key to know which asset to load, and the value to know how much.
        /// </summary>
        public ICollection<KeyValuePair<PooledObjectType, int>> AssetToInstantiates => _assetsToInstantiateIds;

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
            var data = _minionsData[_minionCurrentLoadedIndex];
            _minionCurrentLoadedIndex++;
            return data;
        }

        public void AddMinionData(MinionSaveStruct minion)
        {
            _minionsData.Add(minion);
        }

        public void SavePlayerData(PlayerSaveStruct playerData)
        {
            _playerData = playerData;
        }

        /// <summary>
        /// Call this right after the game is fully loaded.
        /// </summary>
        public void CleanAfterLoad()
        {
            _assetsToInstantiateIds.Clear();
            _minionsData.Clear();
        }
    }
}
