using System.Collections;
using UnityEngine;
using TMPro;

namespace FAILSAFE.UI
{
    /// <summary>
    /// Displays readable notes/documents the player picks up.
    /// Pauses movement while open.
    /// </summary>
    public class NotesViewer : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private float fadeSpeed = 3f;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool _open;

        public void OpenNote(string title, string body)
        {
            titleText.text = title;
            bodyText.text = body;
            panel.SetActive(true);
            _open = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            StartCoroutine(Fade(0f, 1f));
        }

        public void Close()
        {
            if (!_open) return;
            _open = false;
            StartCoroutine(CloseRoutine());
        }

        private IEnumerator CloseRoutine()
        {
            yield return StartCoroutine(Fade(1f, 0f));
            panel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                canvasGroup.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }
            canvasGroup.alpha = to;
        }
    }
}
