using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

/*
    PhotoInteraction.cs — Face-Down Photo with Cinemachine Camera Blend

    When interacted with, the camera smoothly blends (via Cinemachine) to a close-up
    angle looking at the photo on the shelf. The UI overlay then shows the back of
    the frame, pauses, flips to the front (photo sprite + examine text), then the
    camera blends back to the player's view. One-time interaction.

    The Cinemachine blend makes the "picking up and looking at the photo" feel
    cinematic and grounded — the camera drifts to the shelf naturally rather than
    snapping to a scripted position.

    SETUP:
    - Place on photo frame GameObject with a Collider and Renderer, tag "Interactable"
    - Start the photo face-down in the scene (rotate X or Z by 180)
    - Create 1 CinemachineCamera near the photo:
      VCam_PhotoView — Positioned above/in front of the photo, angled to look down at it.
      Priority 15. No Body/Aim extensions. Starts with GameObject INACTIVE.
    - Assign returnCamera: shared VCam_ReturnProxy (same one used by BedroomDoorInteraction).
      Priority 0, no Body/Aim, starts INACTIVE.
    - CinemachineBrain on MainCamera — Default Blend: EaseInOut, 0.75s. Starts DISABLED.
    - Assign photoSprite, photoUI (UI Image on canvas), frameRenderer, audioSource
    - Requires ExamineUI in the scene
    - Player must have PlayerMovement + MouseLook components

    NOTE: If using Cinemachine 2.x, change "using Unity.Cinemachine" to "using Cinemachine"
    and change CinemachineCamera references to CinemachineVirtualCamera.
*/

public class PhotoInteraction : MonoBehaviour
{
    [Header("Examine Settings")]
    [Tooltip("Hover prompt text")]
    public string hoverPrompt = "Pick Up Photo";

    [Tooltip("The photo sprite to display (front of frame)")]
    public Sprite photoSprite;

    [Tooltip("Text displayed while viewing the photo")]
    [TextArea(2, 3)]
    public string examineText = "Isaac and Alex. Holiday, 2018.";

    [Header("UI References")]
    [Tooltip("UI Image component for displaying the photo")]
    public Image photoUI;

    [Header("Visual Settings")]
    [Tooltip("Photo frame Renderer for hover highlight")]
    public Renderer frameRenderer;
    [Tooltip("Color when hovered over")]
    public Color targetColor = Color.yellow;
    [Tooltip("How long the photo is shown before auto-put-down")]
    public float viewDuration = 4f;

    [Header("Audio")]
    [Tooltip("Optional pickup sound")]
    public AudioClip pickupSound;
    [Tooltip("AudioSource for sounds")]
    public AudioSource audioSource;

    [Header("Cinemachine")]
    [Tooltip("Camera positioned to view the photo close-up")]
    public CinemachineCamera photoViewCamera;
    [Tooltip("Return proxy — shared with other sequence scripts")]
    public CinemachineCamera returnCamera;

    [Header("Timing")]
    [Tooltip("Seconds for camera to blend to the photo view")]
    public float blendInTime = 0.8f;
    [Tooltip("Seconds for camera to blend back to player view")]
    public float blendOutTime = 0.6f;

    private bool hasBeenViewed = false;
    private bool isViewing = false;
    private bool over = false;
    private Color originColor;
    private Quaternion faceDownRotation;
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

        if (frameRenderer != null)
            originColor = frameRenderer.material.color;

        // Store the face-down rotation
        faceDownRotation = transform.localRotation;

        // Hide photo UI at start
        if (photoUI != null)
            photoUI.transform.parent.gameObject.SetActive(false);

        // Sequence cameras start inactive
        SetCameraActive(photoViewCamera, false);
        SetCameraActive(returnCamera, false);
    }

    public void Hovering()
    {
        if (isViewing) return;
        over = true;
        StartCoroutine(Fadeout());
        InteractionScript.message = hoverPrompt;
    }

    public void Interacting()
    {
        if (isViewing || hasBeenViewed) return;
        StartCoroutine(ViewSequence());
    }

    void FixedUpdate()
    {
        if (frameRenderer == null) return;

        if (over && !isViewing)
        {
            frameRenderer.material.color = Color.Lerp(frameRenderer.material.color, targetColor, Time.deltaTime * 4);
        }
        else
        {
            frameRenderer.material.color = Color.Lerp(frameRenderer.material.color, originColor, Time.deltaTime * 2);
        }
    }

    private IEnumerator ViewSequence()
    {
        isViewing = true;

        // --- Begin Cinemachine sequence ---

        // Freeze player and mouse look
        if (playerScript != null)
            playerScript.SetWorking(false);
        foreach (MouseLook look in lookScripts)
            look.working = false;

        // Position return proxy at current view
        if (returnCamera != null)
        {
            returnCamera.transform.SetPositionAndRotation(
                mainCamTransform.position, mainCamTransform.rotation);
            SetCameraActive(returnCamera, true);
        }

        // Enable brain — snaps to return proxy (no visible change)
        if (brain != null)
        {
            SetBrainBlend(blendInTime);
            brain.enabled = true;
        }

        // --- Blend to photo view ---

        SetCameraActive(photoViewCamera, true);

        // Play pickup sound during the blend (sounds like Isaac reaching for it)
        if (pickupSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(pickupSound);
        }

        // Wait for blend to complete + a beat of looking at the shelf
        yield return new WaitForSeconds(blendInTime + 0.3f);

        // --- Show photo UI overlay ---

        // Back of frame first (cardboard color, no sprite)
        if (photoUI != null)
        {
            photoUI.sprite = null;
            photoUI.color = new Color(0.3f, 0.25f, 0.2f, 1f);
            photoUI.transform.parent.gameObject.SetActive(true);
        }

        // Pause on the back — Isaac hesitating
        yield return new WaitForSeconds(1f);

        // Flip to front — show photo sprite and examine text
        if (photoUI != null && photoSprite != null)
        {
            photoUI.sprite = photoSprite;
            photoUI.color = Color.white;
        }

        if (ExamineUI.Instance != null)
            ExamineUI.Instance.Show(examineText, viewDuration);

        // Hold for viewing
        yield return new WaitForSeconds(viewDuration);

        // Hide photo UI
        if (photoUI != null)
        {
            photoUI.sprite = null;
            photoUI.transform.parent.gameObject.SetActive(false);
        }

        // Brief pause before setting down
        yield return new WaitForSeconds(0.3f);

        // Restore photo face-down
        transform.localRotation = faceDownRotation;

        // --- End Cinemachine sequence — blend back to player ---

        SetBrainBlend(blendOutTime);
        SetCameraActive(photoViewCamera, false);

        // Wait for return blend to complete
        yield return new WaitForSeconds(blendOutTime + 0.05f);

        // Disable brain, cleanup
        if (brain != null)
            brain.enabled = false;
        SetCameraActive(returnCamera, false);

        // Unfreeze player
        if (playerScript != null)
            playerScript.SetWorking(true);
        foreach (MouseLook look in lookScripts)
            look.working = true;

        hasBeenViewed = true;
        isViewing = false;
    }

    private IEnumerator Fadeout()
    {
        yield return new WaitForSeconds(1);
        over = false;
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
