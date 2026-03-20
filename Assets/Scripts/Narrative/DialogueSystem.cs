using System.Collections;
using UnityEngine;
using TMPro;

namespace FAILSAFE.Narrative
{
    /// <summary>
    /// Displays typed-out dialogue lines with optional speaker attribution.
    /// Drives the subtitle/narrator display at the bottom of the screen.
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Typing")]
        [SerializeField] private float typeSpeed = 30f; // chars per second
        [SerializeField] private float fadeSpeed = 2f;

        private Coroutine _currentLine;

        public void ShowLine(string text, string speaker = "", float holdDuration = 3f)
        {
            if (_currentLine != null) StopCoroutine(_currentLine);
            _currentLine = StartCoroutine(PlayLine(text, speaker, holdDuration));
        }

        public void ClearImmediate()
        {
            if (_currentLine != null) StopCoroutine(_currentLine);
            canvasGroup.alpha = 0f;
        }

        private IEnumerator PlayLine(string text, string speaker, float holdDuration)
        {
            // Fade in
            speakerText.text = speaker;
            dialogueText.text = "";
            yield return StartCoroutine(FadeCanvas(0f, 1f));

            // Type text
            for (int i = 0; i <= text.Length; i++)
            {
                dialogueText.text = text[..i];
                yield return new WaitForSeconds(1f / typeSpeed);
            }

            yield return new WaitForSeconds(holdDuration);

            // Fade out
            yield return StartCoroutine(FadeCanvas(1f, 0f));
        }

        private IEnumerator FadeCanvas(float from, float to)
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
