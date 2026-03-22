using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
    PhotoInteraction.cs — Face-Down Photo Viewing Sequence

    A photo frame that starts face-down on a shelf. When interacted with:
    1. Player is frozen
    2. Camera smoothly moves to view the photo (pickup feel)
    3. First shows back of frame (pause 1 sec)
    4. Rotates to show front — displays UI image (photoSprite) with examine text
    5. After viewing, Isaac AUTOMATICALLY sets it face-down. Player doesn't choose.
    6. Player unfreezes

    SETUP:
    - Place on photo frame GameObject with a Collider and Renderer
    - Tag as "Interactable"
    - Start the photo face-down in the scene (rotate Z or X by 180)
    - Assign photoSprite (the photo image to show in UI)
    - Assign photoUI (UI Image component on canvas for displaying the photo)
    - Assign frameRenderer (the photo frame Renderer for hover highlight)
    - Set examineText for the photo description
    - Requires ExamineUI to exist in the scene
    - Player must have PlayerMovement + MouseLook components
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

    private bool hasBeenViewed = false;
    private bool isViewing = false;
    private bool over = false;
    private Color originColor;
    private Quaternion faceDownRotation;
    private GameObject MainCam;
    private Interact InteractionScript;
    private PlayerMovement playerScript;
    private MouseLook[] lookScripts;

    void Start()
    {
        MainCam = GameObject.FindWithTag("MainCamera");
        if (MainCam == null)
            MainCam = GameObject.FindObjectOfType<Camera>().gameObject;
        InteractionScript = MainCam.GetComponent<Interact>();

        playerScript = FindFirstObjectByType<PlayerMovement>();
        lookScripts = FindObjectsByType<MouseLook>(FindObjectsSortMode.None);

        if (frameRenderer != null)
            originColor = frameRenderer.material.color;

        // Store the face-down rotation
        faceDownRotation = transform.localRotation;

        // Hide photo UI at start
        if (photoUI != null)
            photoUI.transform.parent.gameObject.SetActive(false);
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

        // Freeze player
        if (playerScript != null)
            playerScript.SetWorking(false);
        foreach (MouseLook look in lookScripts)
            look.working = false;

        // Play pickup sound
        if (pickupSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(pickupSound);
        }

        // Brief pickup pause
        yield return new WaitForSeconds(0.5f);

        // Show back of frame (blank/no sprite)
        if (photoUI != null)
        {
            photoUI.sprite = null;
            photoUI.color = new Color(0.3f, 0.25f, 0.2f, 1f); // Cardboard back color
            photoUI.transform.parent.gameObject.SetActive(true);
        }

        // Pause on back
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

        // Hide UI
        if (photoUI != null)
        {
            photoUI.sprite = null;
            photoUI.transform.parent.gameObject.SetActive(false);
        }

        // Brief pause before setting down
        yield return new WaitForSeconds(0.3f);

        // Set photo face-down (restore original face-down rotation)
        transform.localRotation = faceDownRotation;

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
}
