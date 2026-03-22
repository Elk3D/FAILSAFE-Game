using System.Collections;
using UnityEngine;

/*
    TVInteraction.cs — TV Toggle with 3-Frame Anomaly

    Toggles TV on/off. When turning ON (from off state), after a half-second delay
    the screen flickers, and for EXACTLY 3 frames swaps to a "structured static"
    material — an organized pattern, not random noise. Then reverts to normal static.

    SETUP:
    - Place on TV screen mesh GameObject with a Collider and Renderer
    - Tag as "Interactable"
    - Assign normalStaticMaterial (regular TV static)
    - Assign structuredStaticMaterial (the anomaly pattern)
    - Assign offMaterial (dark/black for TV off)
    - Assign tvLight (blue point light, color #4488CC)
    - Assign staticAudioSource (looping static noise AudioSource)
    - Assign tvRenderer (the TV screen Renderer)
    - TV starts ON by default
*/

public class TVInteraction : MonoBehaviour
{
    [Header("Materials")]
    [Tooltip("Normal TV static material")]
    public Material normalStaticMaterial;
    [Tooltip("Structured static anomaly material — shown for exactly 3 frames on power-on")]
    public Material structuredStaticMaterial;
    [Tooltip("Dark/black material when TV is off")]
    public Material offMaterial;

    [Header("References")]
    [Tooltip("The blue point light representing TV glow")]
    public Light tvLight;
    [Tooltip("AudioSource playing looping static noise")]
    public AudioSource staticAudioSource;
    [Tooltip("The TV screen Renderer")]
    public Renderer tvRenderer;

    [Header("Settings")]
    [Tooltip("Hover prompts: 0 = when on, 1 = when off")]
    public string[] prompts = { "Turn Off TV", "Turn On TV" };

    private bool isOn = true;
    private bool canInteract = true;
    private GameObject MainCam;
    private Interact InteractionScript;

    void Start()
    {
        MainCam = GameObject.FindWithTag("MainCamera");
        if (MainCam == null)
            MainCam = GameObject.FindObjectOfType<Camera>().gameObject;
        InteractionScript = MainCam.GetComponent<Interact>();

        // TV starts ON
        if (tvRenderer != null && normalStaticMaterial != null)
            tvRenderer.material = normalStaticMaterial;
        if (tvLight != null)
            tvLight.enabled = true;
        if (staticAudioSource != null)
            staticAudioSource.Play();
    }

    public void Hovering()
    {
        if (!canInteract) return;
        InteractionScript.message = isOn ? prompts[0] : prompts[1];
    }

    public void Interacting()
    {
        if (!canInteract) return;

        if (isOn)
        {
            // Turn OFF
            isOn = false;
            if (tvRenderer != null && offMaterial != null)
                tvRenderer.material = offMaterial;
            if (tvLight != null)
                tvLight.enabled = false;
            if (staticAudioSource != null)
                staticAudioSource.Stop();
        }
        else
        {
            // Turn ON with anomaly sequence
            isOn = true;
            StartCoroutine(TurnOnSequence());
        }
    }

    private IEnumerator TurnOnSequence()
    {
        canInteract = false;

        // Half-second delay before power on
        yield return new WaitForSeconds(0.5f);

        // Power on: normal static, light, audio
        if (tvRenderer != null && normalStaticMaterial != null)
            tvRenderer.material = normalStaticMaterial;
        if (tvLight != null)
            tvLight.enabled = true;
        if (staticAudioSource != null)
            staticAudioSource.Play();

        // Wait a random number of frames (5-15) for flicker feel
        int flickerFrames = Random.Range(5, 16);
        for (int i = 0; i < flickerFrames; i++)
            yield return null;

        // ANOMALY: Swap to structured static for exactly 3 frames
        if (tvRenderer != null && structuredStaticMaterial != null)
            tvRenderer.material = structuredStaticMaterial;

        yield return null; // Frame 1
        yield return null; // Frame 2
        yield return null; // Frame 3

        // Revert to normal static
        if (tvRenderer != null && normalStaticMaterial != null)
            tvRenderer.material = normalStaticMaterial;

        canInteract = true;
    }
}
