using System.Collections;
using UnityEngine;
using TMPro;

/*
    ExamineUI.cs — Shared Examine Text Display System

    Centralized UI panel for showing examine/inspection text with smooth fade transitions.
    Multiple interaction scripts call ExamineUI.Instance.Show() to display text.

    SETUP:
    - Create a Canvas in the scene (or use existing HUD canvas)
    - Add child "ExaminePanel" with Image (dark semi-transparent), CanvasGroup component
    - Add child TextMeshPro - Text (UI) inside ExaminePanel for the examine text
    - Add this script to ExaminePanel
    - Assign examineText field to the TMP_Text component
    - ExaminePanel starts with CanvasGroup alpha = 0
*/

public class ExamineUI : MonoBehaviour
{
    public static ExamineUI Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The TMP_Text component that displays examine text")]
    public TMP_Text examineText;

    [Header("Settings")]
    [Tooltip("Duration of fade in/out transitions")]
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Coroutine activeCoroutine;
    private bool showing = false;

    public bool IsShowing { get { return showing; } }

    void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void Show(string text, float duration = 4f)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ShowCoroutine(text, duration));
    }

    public void ShowMultiple(string[] texts, System.Action onComplete = null)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ShowMultipleCoroutine(texts, onComplete));
    }

    public void Hide()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator ShowCoroutine(string text, float duration)
    {
        examineText.text = text;
        showing = true;

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(duration);

        // Fade out
        yield return FadeOut();
    }

    private IEnumerator ShowMultipleCoroutine(string[] texts, System.Action onComplete)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            examineText.text = texts[i];
            showing = true;

            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            // Wait for Interact press to advance (except last one)
            if (i < texts.Length - 1)
            {
                // Wait one frame so the current Interact press doesn't immediately advance
                yield return null;
                while (!Input.GetButtonDown("Interact"))
                    yield return null;

                // Brief fade between texts
                elapsed = 0f;
                while (elapsed < fadeDuration * 0.5f)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(1f, 0.5f, elapsed / (fadeDuration * 0.5f));
                    yield return null;
                }
            }
            else
            {
                // Last text: hold then fade out
                yield return new WaitForSeconds(3f);
            }
        }

        yield return FadeOut();

        if (onComplete != null)
            onComplete();
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        examineText.text = "";
        showing = false;
        activeCoroutine = null;
    }
}
