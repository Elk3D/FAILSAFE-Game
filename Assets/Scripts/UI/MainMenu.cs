using UnityEngine;
using UnityEngine.SceneManagement;

namespace FAILSAFE.UI
{
    /// <summary>
    /// Main menu buttons: New Game, Continue, Settings, Quit.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject mainPanel;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void NewGame()
        {
            Managers.SaveManager sm = FindObjectOfType<Managers.SaveManager>();
            sm?.DeleteSave();
            SceneManager.LoadScene(Utilities.Constants.SCENE_CHAPTER_01);
        }

        public void Continue()
        {
            Managers.SaveManager sm = FindObjectOfType<Managers.SaveManager>();
            var data = sm?.Load();
            if (data != null)
            {
                // TODO: Route to correct chapter scene
                SceneManager.LoadScene(Utilities.Constants.SCENE_CHAPTER_01);
            }
            else
            {
                NewGame();
            }
        }

        public void OpenSettings()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void CloseSettings()
        {
            settingsPanel.SetActive(false);
            mainPanel.SetActive(true);
        }

        public void Quit() => Managers.GameManager.Instance?.QuitGame();
    }
}
