using UnityEngine;

namespace FAILSAFE.Managers
{
    /// <summary>
    /// Persists and applies player-configurable settings (graphics, audio, controls).
    /// </summary>
    public class SettingsManager : Singleton<SettingsManager>
    {
        [System.Serializable]
        public class Settings
        {
            // Audio
            public float masterVolume = 1f;
            public float musicVolume = 0.8f;
            public float sfxVolume = 1f;
            public float dialogueVolume = 1f;

            // Graphics
            public int qualityLevel = 2;
            public bool fullscreen = true;
            public int resolutionIndex = 0;
            public float brightness = 1f;

            // Controls
            public float mouseSensitivity = 3f;
            public bool invertY = false;
            public bool subtitlesEnabled = true;
        }

        public Settings Current { get; private set; } = new Settings();

        private const string SettingsKey = "FAILSAFE_Settings";

        private void Start() => Load();

        public void Save()
        {
            string json = JsonUtility.ToJson(Current);
            PlayerPrefs.SetString(SettingsKey, json);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(SettingsKey))
                Current = JsonUtility.FromJson<Settings>(PlayerPrefs.GetString(SettingsKey));
            Apply();
        }

        public void Apply()
        {
            QualitySettings.SetQualityLevel(Current.qualityLevel);
            Screen.fullScreen = Current.fullscreen;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.masterVolume = Current.masterVolume;
                AudioManager.Instance.musicVolume = Current.musicVolume;
                AudioManager.Instance.sfxVolume = Current.sfxVolume;
                AudioManager.Instance.ApplyVolumeSettings();
            }
        }

        public void ResetToDefaults()
        {
            Current = new Settings();
            Apply();
        }
    }
}
