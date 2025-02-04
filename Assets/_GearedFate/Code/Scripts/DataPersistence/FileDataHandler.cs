using System.IO;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Class managing the serialization / deserialization of game data and save said data on the disk.
    /// </summary>
    public class FileDataHandler
    {
        private string _dataDirPath = "";

        private string _dataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            this._dataDirPath = dataDirPath;
            this._dataFileName = dataFileName;
        }

        public GameData Load()
        {
            var fullPath = Path.Combine(this._dataDirPath, this._dataFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            if (!File.Exists(fullPath))
            {
                return null;
            }

            string dataToLoad;

            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            return JsonUtility.FromJson<GameData>(dataToLoad);
        }

        public void Save(GameData data)
        {
            var fullPath = Path.Combine(this._dataDirPath, this._dataFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            var dataToStore = JsonUtility.ToJson(data, true);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
    }
}
