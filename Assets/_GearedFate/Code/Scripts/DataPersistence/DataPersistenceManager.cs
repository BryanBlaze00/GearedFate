using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils;

namespace BTG
{
    /// <summary>
    /// Manager for handling loading and saving game data.
    /// </summary>
    public class DataPersistenceManager : PersistentSingleton<DataPersistenceManager>
    {
        // Name of the saved file.
        [SerializeField] private string _saveFileName;

        // Full state of the game is saved in GameData
        private GameData _gameData;

        // List of all objects that need to be saved and loaded
        private List<IDataPersistence> _dataPersistencesObjects;

        // Helper class to serialize / unserialize data and save it to file.
        private FileDataHandler _dataHandler;

        private void Start()
        {
            this._dataHandler = new FileDataHandler(Application.persistentDataPath, this._saveFileName);
            this.LoadGame();
        }

        private void OnApplicationQuit()
        {
            this.SaveGame();
        }

        // TODO : Call that from a menu "Start" button
        public void NewGame()
        {
            this._gameData = new GameData();
        }

        // TODO : Call that from a menu "Continue" button
        public void LoadGame()
        {
            this._gameData = this._dataHandler.Load();
            if (this._gameData == null) this.NewGame();

            foreach (var pair in this._gameData.AssetToInstantiates)
                for (var i = 0; i < pair.Value; i++)
                {
                    var obj = ObjectPool.Instance.GetPooledObject(pair.Key);
                    obj.SetActive(true);
                }

            this.FindAllDataPersistence();

            foreach (var dataPersistenceObject in this._dataPersistencesObjects) dataPersistenceObject.LoadData(this._gameData);

            this._gameData.CleanAfterLoad();
        }

        // TODO : Call that from a menu "Save" button or Save from time to time (checkpoints ?)
        public void SaveGame()
        {
            this.FindAllDataPersistence();
            foreach (var dataPersistenceObject in this._dataPersistencesObjects)
            {
                // Instance that need to be saved register in the game data using their pooled object type.
                if (dataPersistenceObject is ISaveableInstance saveableInstance) this._gameData.AddAssetIndexToInstantiate(saveableInstance.PooledObjectType);
                dataPersistenceObject.SaveData(ref this._gameData);
            }

            this._dataHandler.Save(this._gameData);
        }

        private void FindAllDataPersistence()
        {
            this._dataPersistencesObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .OfType<IDataPersistence>()
                    .ToList();
        }
    }
}
