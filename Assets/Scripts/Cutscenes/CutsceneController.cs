using UnityEngine;
using UnityEngine.Playables;

namespace FAILSAFE.Cutscenes
{
    /// <summary>
    /// Wraps Unity's PlayableDirector to integrate with GameManager state.
    /// Disables player control for the duration of the cutscene.
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public class CutsceneController : MonoBehaviour
    {
        [SerializeField] private bool disablePlayerOnPlay = true;

        private PlayableDirector _director;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _director.stopped += OnStopped;
        }

        public void Play()
        {
            if (disablePlayerOnPlay)
                Managers.GameManager.Instance?.SetState(Managers.GameManager.GameState.Cutscene);
            _director.Play();
        }

        public void Skip()
        {
            _director.time = _director.duration;
            _director.Evaluate();
            _director.Stop();
        }

        private void OnStopped(PlayableDirector _)
        {
            if (disablePlayerOnPlay)
                Managers.GameManager.Instance?.SetState(Managers.GameManager.GameState.Playing);
        }
    }
}
