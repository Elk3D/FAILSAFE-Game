using UnityEngine;

namespace FAILSAFE.Managers
{
    /// <summary>
    /// Central game state manager. Controls overall game flow, chapter progression,
    /// and acts as a hub for accessing other managers.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public enum GameState { MainMenu, Playing, Paused, Cutscene, Loading, GameOver }

        [Header("State")]
        public GameState CurrentState { get; private set; }

        [Header("References")]
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private ChapterManager chapterManager;

        protected override void Awake()
        {
            base.Awake();
            // TODO: Initialize subsystems
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            // TODO: Handle state transitions
        }

        public void StartNewGame()
        {
            // TODO: Load chapter 1
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
