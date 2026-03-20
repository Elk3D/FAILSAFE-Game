using UnityEngine;
using UnityEngine.Playables;

namespace FAILSAFE.Gameplay.Triggers
{
    /// <summary>
    /// Plays a Unity Timeline cutscene when the player enters the trigger zone.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CutsceneTrigger : MonoBehaviour
    {
        [Header("Cutscene")]
        [SerializeField] private PlayableDirector director;
        [SerializeField] private bool disablePlayerControl = true;
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private string playerTag = "Player";

        private bool _triggered;

        private void Awake() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered && triggerOnce) return;
            if (!other.CompareTag(playerTag)) return;

            _triggered = true;

            if (disablePlayerControl)
                Managers.GameManager.Instance?.SetState(Managers.GameManager.GameState.Cutscene);

            director?.Play();
            director.stopped += OnCutsceneStopped;
        }

        private void OnCutsceneStopped(PlayableDirector _)
        {
            if (disablePlayerControl)
                Managers.GameManager.Instance?.SetState(Managers.GameManager.GameState.Playing);
        }
    }
}
