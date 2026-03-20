using UnityEngine;
using UnityEngine.Events;

namespace FAILSAFE.Gameplay.Triggers
{
    /// <summary>
    /// Fires narrative events (dialogue, subtitles, epigraphs) when the player
    /// walks into a zone. Can be one-shot or repeatable.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NarrativeTrigger : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private string playerTag = "Player";

        [Header("Narrative")]
        [TextArea(2, 5)]
        [SerializeField] private string narratorLine;
        [SerializeField] private AudioClip narratorClip;
        [SerializeField] private float subtitleDuration = 4f;

        [Header("Events")]
        public UnityEvent onTriggered;

        private bool _triggered;

        private void Awake() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered && triggerOnce) return;
            if (!other.CompareTag(playerTag)) return;

            _triggered = true;

            if (narratorClip != null)
                Managers.AudioManager.Instance?.PlayDialogue(narratorClip);

            // TODO: Show subtitle via SubtitleSystem

            onTriggered?.Invoke();
        }
    }
}
