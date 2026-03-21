using System.Collections;
using UnityEngine;
using TMPro;

namespace FAILSAFE.UI
{
    /// <summary>
    /// Displays timed subtitle lines. Subtitles can be toggled off in Settings.
    /// </summary>
    public class SubtitleSystem : MonoBehaviour
    {
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;

        private Coroutine _current;

        public void ShowSubtitle(string text, float duration)
        {
            if (!Managers.SettingsManager.Instance?.Current.subtitlesEnabled ?? false) return;
            if (_current != null) StopCoroutine(_current);
            _current = StartCoroutine(ShowRoutine(text, duration));
        }

        public void Clear()
        {
            if (_current != null) StopCoroutine(_current);
            canvasGroup.alpha = 0f;
        }

        private IEnumerator ShowRoutine(string text, float duration)
        {
            subtitleText.text = text;
            yield return StartCoroutine(Fade(0f, 1f));
            yield return new WaitForSeconds(duration);
            yield return StartCoroutine(Fade(1f, 0f));
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }
            canvasGroup.alpha = to;
        }
    }
}
