using UnityEngine;
using System.IO;

namespace FAILSAFE.Managers
{
    /// <summary>
    /// Handles saving and loading game progress.
    /// Persists chapter progress, collected items, and puzzle states.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        [System.Serializable]
        public class SaveData
        {
            public int currentChapter;
            public string[] collectedItemIDs;
            public string[] completedPuzzleIDs;
            public float totalPlaytime;
            public System.DateTime lastSaved;
        }

        private static readonly string SavePath = Application.persistentDataPath + "/failsafe_save.json";

        public void Save(SaveData data)
        {
            data.lastSaved = System.DateTime.Now;
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }

        public SaveData Load()
        {
            if (!File.Exists(SavePath)) return null;
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}
