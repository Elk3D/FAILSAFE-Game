using UnityEngine;

namespace FAILSAFE.UI
{
    /// <summary>
    /// Opens/closes the pause menu and pauses Time.timeScale.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private string pauseButton = "Pause";

        private bool _paused;

        private void Update()
        {
            if (Input.GetButtonDown(pauseButton)) Toggle();
        }

        public void Toggle()
        {
            _paused = !_paused;
            pausePanel.SetActive(_paused);
            Time.timeScale = _paused ? 0f : 1f;
            Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _paused;
            Managers.GameManager.Instance?.SetState(_paused
                ? Managers.GameManager.GameState.Paused
                : Managers.GameManager.GameState.Playing);
        }

        public void Resume() { if (_paused) Toggle(); }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(Utilities.Constants.SCENE_MAIN_MENU);
        }
    }
}
