namespace BTG
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityUtils;

    /// <summary>
    /// Manager for handling loading and saving game data.
    /// </summary>
    public class DataPersistenceManager : PersistentSingleton<DataPersistenceManager>
    {
        // Name of the saved file.
        [SerializeField]
        private string _saveFileName;

        // Full state of the game is saved in GameData
        private GameData _gameData;

        // List of all objects that need to be saved and loaded
        private List<IDataPersistence> _dataPersistencesObjects;

        // Helper class to serialize / unserialize data and save it to file.
        private FileDataHandler _dataHandler;

        private void Start()
        {
            _dataHandler = new FileDataHandler(Application.persistentDataPath, _saveFileName);
            LoadGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        // TODO : Call that from a menu "Start" button
        public void NewGame()
        {
            _gameData = new GameData();
        }

        // TODO : Call that from a menu "Continue" button
        public void LoadGame()
        {
            _gameData = _dataHandler.Load();
            if (_gameData == null)
            {
                NewGame();
            }

            foreach (var pair in _gameData.AssetToInstantiates)
            {
                for (var i = 0; i < pair.Value; i++)
                {
                    var obj = ObjectPool.Instance.GetPooledObject(pair.Key);
                    obj.SetActive(true);
                }
            }

            FindAllDataPersistence();

            foreach (var dataPersistenceObject in _dataPersistencesObjects)
            {
                dataPersistenceObject.LoadData(_gameData);
            }

            _gameData.CleanAfterLoad();
        }

        // TODO : Call that from a menu "Save" button or Save from time to time (checkpoints ?)
        public void SaveGame()
        {
            FindAllDataPersistence();
            foreach (var dataPersistenceObject in _dataPersistencesObjects)
            {
                // Instance that need to be saved register in the game data using their pooled object type.
                if (dataPersistenceObject is ISaveableInstance saveableInstance)
                {
                    _gameData.AddAssetIndexToInstantiate(saveableInstance.PooledObjectType);
                }

                dataPersistenceObject.SaveData(ref _gameData);
            }

            _dataHandler.Save(_gameData);
        }

        private void FindAllDataPersistence()
        {
            _dataPersistencesObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .OfType<IDataPersistence>()
                    .ToList();
        }
    }
}
