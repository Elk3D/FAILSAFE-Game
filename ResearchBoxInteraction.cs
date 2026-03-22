using System.Collections;
using UnityEngine;

/*
    ResearchBoxInteraction.cs — Classified Box Examine + Hidden Proximity Music Box Note

    Standard examine interaction showing classified STRATOS / PROJECT FAILSAFE text.
    Hidden mechanic: after examining, if the player stands within proximityRadius
    for idleTimeRequired seconds without pressing Interact, a single crystalline
    music box note plays. Doesn't repeat. No UI. Isaac doesn't react.

    SETUP:
    - Place on box/crate GameObject with a Collider
    - Tag as "Interactable"
    - Assign musicBoxNote (single crystalline note AudioClip)
    - Assign audioSource (AudioSource component, can be on this object)
    - Set examineTexts, proximityRadius, idleTimeRequired in Inspector
    - Requires ExamineUI to exist in the scene
*/

[RequireComponent(typeof(AudioSource))]
public class ResearchBoxInteraction : MonoBehaviour
{
    [Header("Examine Settings")]
    [Tooltip("Text shown when examining the box")]
    [TextArea(2, 5)]
    public string[] examineTexts = {
        "STRATOS — CLASSIFIED — PROJECT FAILSAFE",
        "DESTROY AFTER REVIEW — DO NOT DUPLICATE"
    };

    [Tooltip("Hover prompt text")]
    public string hoverPrompt = "Examine";

    [Header("Hidden Music Box")]
    [Tooltip("Single crystalline music box note")]
    public AudioClip musicBoxNote;
    [Tooltip("AudioSource for playing the note")]
    public AudioSource audioSource;
    [Tooltip("Player must be within this radius for the idle trigger")]
    public float proximityRadius = 2f;
    [Tooltip("Seconds player must idle nearby before the note plays")]
    public float idleTimeRequired = 10f;

    private bool hasExamined = false;
    private bool musicBoxPlayed = false;
    private float idleTimer = 0f;
    private Transform playerTransform;
    private GameObject MainCam;
    private Interact InteractionScript;

    void Start()
    {
        MainCam = GameObject.FindWithTag("MainCamera");
        if (MainCam == null)
            MainCam = GameObject.FindObjectOfType<Camera>().gameObject;
        InteractionScript = MainCam.GetComponent<Interact>();

        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            playerTransform = player.transform;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void Hovering()
    {
        InteractionScript.message = hoverPrompt;
    }

    public void Interacting()
    {
        if (ExamineUI.Instance == null) return;

        string fullText = string.Join("\n", examineTexts);
        ExamineUI.Instance.Show(fullText, 5f);
        hasExamined = true;
        idleTimer = 0f;
    }

    void Update()
    {
        if (!hasExamined || musicBoxPlayed) return;
        if (playerTransform == null) return;

        float dist = Vector3.Distance(playerTransform.position, transform.position);

        if (dist <= proximityRadius)
        {
            // Only reset timer on Interact button press
            if (Input.GetButtonDown("Interact"))
            {
                idleTimer = 0f;
                return;
            }

            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeRequired)
            {
                if (musicBoxNote != null && audioSource != null)
                    audioSource.PlayOneShot(musicBoxNote);
                musicBoxPlayed = true;
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }
}
