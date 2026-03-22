using System.Collections;
using UnityEngine;

/*
    WindowInteraction.cs — Blocked Curtain Interaction + Ambient Child Laugh

    The player CANNOT open the curtains. Interacting shows examine text about
    heavy, permanently drawn curtains. Periodically (random 60-120 sec interval),
    a distant child laughing audio plays through the window. No UI indicator.

    SETUP:
    - Place on window/curtain GameObject with a Collider
    - Tag as "Interactable"
    - Assign childLaughClip (distant child laughing audio)
    - Assign audioSource (AudioSource on or near the window, 3D spatial blend)
    - Set audioSource volume low (~0.15-0.3) for subtle ambient effect
    - Requires ExamineUI to exist in the scene
*/

public class WindowInteraction : MonoBehaviour
{
    [Header("Examine Settings")]
    [Tooltip("Text shown when player tries to interact with the curtains")]
    [TextArea(2, 5)]
    public string examineText = "Heavy curtains, permanently drawn. The fabric is thick — almost industrial. A faint orange glow seeps through the edges.";

    [Tooltip("Hover prompt text")]
    public string hoverPrompt = "Open Curtains";

    [Header("Ambient Child Laugh")]
    [Tooltip("Distant child laughing audio clip")]
    public AudioClip childLaughClip;
    [Tooltip("AudioSource for playing the laugh (set to 3D spatial blend)")]
    public AudioSource audioSource;
    [Tooltip("Minimum seconds between laughs")]
    public float minInterval = 60f;
    [Tooltip("Maximum seconds between laughs")]
    public float maxInterval = 120f;

    private float laughTimer;
    private GameObject MainCam;
    private Interact InteractionScript;

    void Start()
    {
        MainCam = GameObject.FindWithTag("MainCamera");
        if (MainCam == null)
            MainCam = GameObject.FindObjectOfType<Camera>().gameObject;
        InteractionScript = MainCam.GetComponent<Interact>();

        laughTimer = Random.Range(minInterval, maxInterval);
    }

    public void Hovering()
    {
        InteractionScript.message = hoverPrompt;
    }

    public void Interacting()
    {
        if (ExamineUI.Instance != null)
            ExamineUI.Instance.Show(examineText, 4f);
    }

    void Update()
    {
        if (childLaughClip == null || audioSource == null) return;

        laughTimer -= Time.deltaTime;
        if (laughTimer <= 0f)
        {
            audioSource.PlayOneShot(childLaughClip);
            laughTimer = Random.Range(minInterval, maxInterval);
        }
    }
}
