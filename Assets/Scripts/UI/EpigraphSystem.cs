using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Plays a full-screen epigraph sequence at the start of a scene:
///   1. Screen is black and input is blocked
///   2. Quote text fades in
///   3. Author text fades in (after authorDelay seconds)
///   4. Both hold for holdDuration seconds
///   5. Text fades out
///   6. Next scene loads (black overlay stays, hiding the transition)
///
/// SETUP:
///   1. Create a Canvas (Screen Space - Overlay, Sort Order 99)
///   2. Add a full-screen black Image child  → assign to "backgroundOverlay"
///   3. Add a TextMeshProUGUI for the quote  → assign to "quoteText"
///   4. Add a TextMeshProUGUI for the author → assign to "authorText"
///   5. Attach this script to a GameObject in the scene
///   6. Fill in quoteContent, authorName, and nextSceneName in the Inspector
///
/// TIP: Set quoteText and authorText anchors to center-center for best results.
/// Make sure your next scene is added to File > Build Settings.
/// </summary>
public class EpigraphSystem : MonoBehaviour
{
    [Header("Content")]
    [TextArea(2, 6)]
    [Tooltip("The epigraph / quote to display.")]
    public string quoteContent = "Not everything that is faced can be changed,\nbut nothing can be changed until it is faced.";

    [Tooltip("Who said it. Leave blank to skip the author line.")]
    public string authorName = "— James Baldwin";

    [Header("Scene Transition")]
    [Tooltip("Name of the scene to load after the sequence. Must be added to Build Settings.")]
    public string nextSceneName = "";

    [Header("UI References")]
    [Tooltip("Full-screen Image set to solid black (alpha 1).")]
    public Image backgroundOverlay;

    [Tooltip("TextMeshPro component for the main quote.")]
    public TMP_Text quoteText;

    [Tooltip("TextMeshPro component for the author attribution.")]
    public TMP_Text authorText;

    [Header("Timing (seconds)")]
    [Tooltip("How long to fade the quote text in.")]
    [Range(0.1f, 3f)] public float quoteFadeInDuration = 1.5f;

    [Tooltip("Delay after quote starts fading before the author fades in.")]
    [Range(0f, 4f)] public float authorDelay = 1f;

    [Tooltip("How long to fade the author text in.")]
    [Range(0.1f, 3f)] public float authorFadeInDuration = 1f;

    [Tooltip("How long both texts stay fully visible before fading out.")]
    [Range(1f, 15f)] public float holdDuration = 6f;

    [Tooltip("How long to fade both texts out.")]
    [Range(0.1f, 3f)] public float textFadeOutDuration = 1f;

    // -------------------------------------------------------------------------

    private void Start()
    {
        // Validate references before doing anything
        if (backgroundOverlay == null || quoteText == null || authorText == null)
        {
            Debug.LogError("[EpigraphSystem] One or more UI references are missing. " +
                           "Please assign backgroundOverlay, quoteText, and authorText in the Inspector.");
            return;
        }

        // Initialise visual state: overlay fully opaque, texts invisible
        SetAlpha(backgroundOverlay, 1f);
        SetTextAlpha(quoteText,  0f);
        SetTextAlpha(authorText, 0f);

        // Populate text content
        quoteText.text  = quoteContent;
        authorText.text = string.IsNullOrWhiteSpace(authorName) ? "" : authorName;

        StartCoroutine(PlaySequence());
    }

    // -------------------------------------------------------------------------

    private IEnumerator PlaySequence()
    {
        // --- 1. Fade quote in ---
        yield return StartCoroutine(FadeText(quoteText, 0f, 1f, quoteFadeInDuration));

        // --- 2. Fade author in (with a head-start delay) ---
        if (!string.IsNullOrWhiteSpace(authorName))
        {
            yield return new WaitForSeconds(authorDelay);
            yield return StartCoroutine(FadeText(authorText, 0f, 1f, authorFadeInDuration));
        }

        // --- 3. Hold ---
        yield return new WaitForSeconds(holdDuration);

        // --- 4. Fade both texts out simultaneously ---
        Coroutine fadeQuote  = StartCoroutine(FadeText(quoteText,  1f, 0f, textFadeOutDuration));
        Coroutine fadeAuthor = StartCoroutine(FadeText(authorText, 1f, 0f, textFadeOutDuration));
        yield return fadeQuote;
        yield return fadeAuthor;

        // --- 5. Load next scene (black overlay stays, masking the transition) ---
        if (!string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[EpigraphSystem] nextSceneName is empty — set it in the Inspector.");
        }
    }

    // -------------------------------------------------------------------------
    // Helpers

    private IEnumerator FadeText(TMP_Text target, float from, float to, float duration)
    {
        float elapsed = 0f;
        SetTextAlpha(target, from);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetTextAlpha(target, Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }

        SetTextAlpha(target, to);
    }

    private static void SetTextAlpha(TMP_Text t, float a)
    {
        Color c = t.color;
        c.a = a;
        t.color = c;
    }

    private static void SetAlpha(Graphic g, float a)
    {
        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}
