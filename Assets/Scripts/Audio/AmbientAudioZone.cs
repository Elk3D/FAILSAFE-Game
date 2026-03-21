using UnityEngine;

namespace FAILSAFE.Audio
{
    /// <summary>
    /// Blends ambient audio in/out as the player enters and exits a trigger zone.
    /// Multiple zones can overlap; each controls its own AudioSource.
    /// </summary>
    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    public class AmbientAudioZone : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private AudioClip ambientClip;
        [SerializeField] private float targetVolume = 0.5f;
        [SerializeField] private float fadeSpeed = 1f;
        [SerializeField] private string playerTag = "Player";

        private AudioSource _source;
        private bool _playerInside;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            _source = GetComponent<AudioSource>();
            _source.clip = ambientClip;
            _source.loop = true;
            _source.volume = 0f;
            _source.spatialBlend = 0f; // 2D
            _source.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag)) _playerInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag)) _playerInside = false;
        }

        private void Update()
        {
            float target = _playerInside ? targetVolume : 0f;
            _source.volume = Mathf.MoveTowards(_source.volume, target, fadeSpeed * Time.deltaTime);
        }
    }
}
