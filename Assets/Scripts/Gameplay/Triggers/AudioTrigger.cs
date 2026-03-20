using UnityEngine;

namespace FAILSAFE.Gameplay.Triggers
{
    /// <summary>
    /// Triggers a music or ambience change when the player enters a zone.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AudioTrigger : MonoBehaviour
    {
        public enum AudioType { Music, Ambience, SFX }

        [Header("Settings")]
        [SerializeField] private bool triggerOnce = false;
        [SerializeField] private string playerTag = "Player";

        [Header("Audio")]
        [SerializeField] private AudioType audioType = AudioType.Ambience;
        [SerializeField] private AudioClip clip;

        private bool _triggered;

        private void Awake() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered && triggerOnce) return;
            if (!other.CompareTag(playerTag)) return;

            _triggered = true;
            var am = Managers.AudioManager.Instance;
            if (am == null) return;

            switch (audioType)
            {
                case AudioType.Music:    am.PlayMusic(clip);    break;
                case AudioType.Ambience: am.PlayAmbience(clip); break;
                case AudioType.SFX:      am.PlaySFX(clip);      break;
            }
        }
    }
}
