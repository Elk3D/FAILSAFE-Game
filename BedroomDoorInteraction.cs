using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

/*
    BedroomDoorInteraction.cs — 3-Attempt Bedroom Door with Cinemachine Camera Blending

    Each attempt activates a pre-placed CinemachineCamera. CinemachineBrain on MainCamera
    handles smooth EaseInOut blends between the player's view and the scripted angle.
    The result is seamless, cinematic transitions — no hard cuts.

    - Attempt 1: Camera blends to a "reaching toward door" angle. Pauses. Blends back.
    - Attempt 2: Camera blends closer to handle. Isaac's involuntary sound. "Not tonight."
    - Attempt 3: Camera blends to looking at the floor. "Not ever." Permanent lockout.

    SETUP:
    - Place on bedroom door GameObject with a Collider, tag "Interactable"
    - CinemachineBrain on MainCamera — Default Blend: EaseInOut, 0.75s. Starts DISABLED.
    - Create 3 CinemachineCamera objects near the door (all start with GameObject INACTIVE):
      VCam_Door_Reach  — Eye height, slightly forward toward door handle. Priority 15.
      VCam_Door_Touch  — Closer to door, similar angle but more intimate. Priority 15.
      VCam_Door_Dip    — Same area, rotated down ~40° looking at floor. Priority 15.
    - All 3 cameras: no Body/Aim extensions (fixed world-space cameras).
    - Create 1 shared VCam_ReturnProxy (CinemachineCamera, Priority 0, starts INACTIVE,
      no Body/Aim). This is repositioned at runtime to match the player's current view.
      Shared across all sequence scripts — only one sequence runs at a time.
    - Assign all 4 cameras + audioSource + involuntarySound in Inspector
    - Requires ExamineUI in the scene
    - Player must have PlayerMovement + MouseLook components

    NOTE: If using Cinemachine 2.x, change "using Unity.Cinemachine" to "using Cinemachine"
    and change CinemachineCamera references to CinemachineVirtualCamera.
*/

[RequireComponent(typeof(AudioSource))]
public class BedroomDoorInteraction : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Hover prompt text")]
    public string hoverPrompt = "Open Door";

    [Tooltip("Isaac's involuntary sound on attempt 2")]
    public AudioClip involuntarySound;

    [Tooltip("AudioSource for playing sounds")]
    public AudioSource audioSource;

    [Header("Cinemachine Sequence Cameras")]
    [Tooltip("Slight lean toward door — attempt 1")]
    public CinemachineCamera reachCamera;
    [Tooltip("Close to door handle — attempt 2")]
    public CinemachineCamera touchCamera;
    [Tooltip("Looking down at floor — attempt 3")]
    public CinemachineCamera dipCamera;
    [Tooltip("Return proxy — repositioned at player's current view before each sequence")]
    public CinemachineCamera returnCamera;

    [Header("Timing")]
    [Tooltip("Seconds for the camera to blend into the scripted angle")]
    public float blendInTime = 0.75f;
    [Tooltip("Seconds for the camera to blend back to the player's view")]
    public float blendOutTime = 0.6f;

    private int attemptCount = 0;
    private bool isAnimating = false;
    private Transform mainCamTransform;
    private Interact InteractionScript;
    private PlayerMovement playerScript;
    private MouseLook[] lookScripts;
    private CinemachineBrain brain;

    void Start()
    {
        GameObject camObj = GameObject.FindWithTag("MainCamera");
        if (camObj == null)
            camObj = FindObjectOfType<Camera>().gameObject;
        mainCamTransform = camObj.transform;
        InteractionScript = camObj.GetComponent<Interact>();
        brain = camObj.GetComponent<CinemachineBrain>();

        playerScript = FindFirstObjectByType<PlayerMovement>();
        lookScripts = FindObjectsByType<MouseLook>(FindObjectsSortMode.None);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // All sequence cameras start inactive
        SetCameraActive(reachCamera, false);
        SetCameraActive(touchCamera, false);
        SetCameraActive(dipCamera, false);
        SetCameraActive(returnCamera, false);
    }

    public void Hovering()
    {
        if (attemptCount >= 3 || isAnimating) return;
        InteractionScript.message = hoverPrompt;
    }

    public void Interacting()
    {
        if (attemptCount >= 3 || isAnimating) return;
        attemptCount++;

        switch (attemptCount)
        {
            case 1: StartCoroutine(Attempt1_Reach()); break;
            case 2: StartCoroutine(Attempt2_Touch()); break;
            case 3: StartCoroutine(Attempt3_Dip()); break;
        }
    }

    // --- Sequence lifecycle ---

    private void BeginSequence()
    {
        isAnimating = true;

        // Freeze player movement and mouse look FIRST
        // (prevents MouseLook from running in LateUpdate this frame)
        if (playerScript != null)
            playerScript.SetWorking(false);
        foreach (MouseLook look in lookScripts)
            look.working = false;

        // Position return proxy at the player's exact current view
        // When brain enables, it snaps to this camera = no visible change
        if (returnCamera != null)
        {
            returnCamera.transform.SetPositionAndRotation(
                mainCamTransform.position, mainCamTransform.rotation);
            SetCameraActive(returnCamera, true);
        }

        // Enable CinemachineBrain and set blend curve
        if (brain != null)
        {
            SetBrainBlend(blendInTime);
            brain.enabled = true;
        }
    }

    private IEnumerator EndSequence(CinemachineCamera activeCam)
    {
        // Set blend timing for the return transition (slightly faster feels natural)
        SetBrainBlend(blendOutTime);

        // Deactivate the scripted camera — brain auto-blends back to returnCamera
        SetCameraActive(activeCam, false);

        // Wait for the return blend to fully complete
        yield return new WaitForSeconds(blendOutTime + 0.05f);

        // Disable brain and return camera — hand control back to MouseLook
        if (brain != null)
            brain.enabled = false;
        SetCameraActive(returnCamera, false);

        // Unfreeze player
        if (playerScript != null)
            playerScript.SetWorking(true);
        foreach (MouseLook look in lookScripts)
            look.working = true;

        isAnimating = false;
    }

    // --- The three attempts ---

    // Attempt 1: Camera drifts to a "reaching toward door" angle, holds briefly, returns.
    // No text, no sound. Just Isaac's hesitation.
    private IEnumerator Attempt1_Reach()
    {
        BeginSequence();

        SetCameraActive(reachCamera, true);

        // Wait for blend to complete + brief hold at the door
        yield return new WaitForSeconds(blendInTime + 0.4f);

        yield return StartCoroutine(EndSequence(reachCamera));
    }

    // Attempt 2: Camera drifts closer (hand touching handle). Sound. "Not tonight."
    private IEnumerator Attempt2_Touch()
    {
        BeginSequence();

        SetCameraActive(touchCamera, true);

        // Wait for blend to finish — Isaac's hand reaches the handle
        yield return new WaitForSeconds(blendInTime);

        // The involuntary sound at the moment of contact
        if (involuntarySound != null && audioSource != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(involuntarySound);
        }

        if (ExamineUI.Instance != null)
            ExamineUI.Instance.Show("Not tonight.", 2.5f);

        // Hold at the door while text is visible
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(EndSequence(touchCamera));
    }

    // Attempt 3: Camera dips down to look at the floor. "Not ever." Permanent lockout.
    private IEnumerator Attempt3_Dip()
    {
        BeginSequence();

        SetCameraActive(dipCamera, true);

        // Wait for the downward blend — Isaac can't look at the door anymore
        yield return new WaitForSeconds(blendInTime);

        if (ExamineUI.Instance != null)
            ExamineUI.Instance.Show("Not ever.", 3f);

        // Long hold — let the weight of it sit
        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(EndSequence(dipCamera));

        // Permanently disable interaction
        gameObject.tag = "Untagged";
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    // --- Helpers ---

    private void SetBrainBlend(float duration)
    {
        if (brain == null) return;
        // Cinemachine 3.x API — for 2.x, use: brain.m_DefaultBlend.m_Time = duration;
        var blend = brain.DefaultBlend;
        blend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
        blend.Time = duration;
        brain.DefaultBlend = blend;
    }

    private void SetCameraActive(CinemachineCamera cam, bool active)
    {
        if (cam != null)
            cam.gameObject.SetActive(active);
    }
}
