using UnityEngine;
using UnityEngine.SceneManagement;

namespace FAILSAFE.Gameplay.Triggers
{
    /// <summary>
    /// Loads a new scene (optionally via EpigraphSystem) when the player enters.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SceneTrigger : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] private string targetScene;
        [SerializeField] private bool useEpigraph = true;
        [SerializeField] private string playerTag = "Player";

        private bool _triggered;

        private void Awake() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;
            if (!other.CompareTag(playerTag)) return;
            _triggered = true;

            if (useEpigraph)
            {
                // TODO: Pass targetScene to EpigraphSystem and let it handle the transition
            }
            else
            {
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}
