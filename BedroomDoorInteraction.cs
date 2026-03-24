using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

/*
    BedroomDoorInteraction.cs — 3-Attempt Bedroom Door with Permanent Lockout

    The bedroom door has a 3-attempt sequence:
    - Attempt 1: Hand reaches toward door, then withdraws. No text.
    - Attempt 2: Hand reaches, touches handle, Isaac makes involuntary sound.
                 Text: "Not tonight."
    - Attempt 3: Camera dips down (looking at floor). Text: "Not ever."
                 Interaction point DISAPPEARS permanently.

    Camera animations use direct transform lerping on the main camera's local
    position/rotation. Player is frozen during each sequence.

    SETUP:
    - Place on bedroom door GameObject with a Collider
    - Tag as "Interactable"
    - Assign involuntarySound (AudioClip — Isaac's involuntary noise)
    - Assign audioSource (AudioSource component)
    - Requires ExamineUI to exist in the scene
    - Player must have PlayerMovement + MouseLook components
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

    [Header("Camera Animation")]
    [Tooltip("How long the reach animation takes")]
    public float reachDuration = 1.5f;
    [Tooltip("How far forward the camera moves on reach (local Z)")]
    public float reachDistance = 0.3f;
    [Tooltip("How long the camera dip takes on attempt 3")]
    public float dipDuration = 2f;
    [Tooltip("How far down the camera dips in degrees on attempt 3")]
    public float dipAngle = 40f;

    private int attemptCount = 0;
    private bool isAnimating = false;
    private Transform cameraTransform;
    private Vector3 originalCamLocalPos;
    private Quaternion originalCamLocalRot;
    private GameObject MainCam;
    private Interact InteractionScript;
    private PlayerMovement playerScript;
    private MouseLook[] lookScripts;
    private CinemachineBrain cinemachineBrain;

    void Start()
    {
        MainCam = GameObject.FindWithTag("MainCamera");
        if (MainCam == null)
            MainCam = GameObject.FindObjectOfType<Camera>().gameObject;
        InteractionScript = MainCam.GetComponent<Interact>();
        cameraTransform = MainCam.transform;
        originalCamLocalPos = cameraTransform.localPosition;
        originalCamLocalRot = cameraTransform.localRotation;

        playerScript = FindFirstObjectByType<PlayerMovement>();
        lookScripts = FindObjectsByType<MouseLook>(FindObjectsSortMode.None);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        cinemachineBrain = MainCam.GetComponent<CinemachineBrain>();
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
            case 1:
                StartCoroutine(ReachAndWithdraw());
                break;
            case 2:
                StartCoroutine(ReachTouchWithdraw());
                break;
            case 3:
                StartCoroutine(FinalAttempt());
                break;
        }
    }

    private void FreezePlayer()
    {
        if (playerScript != null)
            playerScript.SetWorking(false);
        foreach (MouseLook look in lookScripts)
            look.working = false;
        if (cinemachineBrain != null)
            cinemachineBrain.enabled = false;
    }

    private void UnfreezePlayer()
    {
        if (cinemachineBrain != null)
            cinemachineBrain.enabled = true;
        if (playerScript != null)
            playerScript.SetWorking(true);
        foreach (MouseLook look in lookScripts)
            look.working = true;
    }

    private void RestoreCamera()
    {
        cameraTransform.localPosition = originalCamLocalPos;
        cameraTransform.localRotation = originalCamLocalRot;
    }

    // Attempt 1: Reach toward door and withdraw. No text.
    private IEnumerator ReachAndWithdraw()
    {
        isAnimating = true;
        FreezePlayer();

        Vector3 targetPos = originalCamLocalPos + Vector3.forward * reachDistance * 0.5f;

        // Reach forward
        float elapsed = 0f;
        float halfDuration = reachDuration * 0.4f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            cameraTransform.localPosition = Vector3.Lerp(originalCamLocalPos, targetPos, t);
            yield return null;
        }

        // Brief pause at apex
        yield return new WaitForSeconds(0.3f);

        // Withdraw
        elapsed = 0f;
        float withdrawDuration = reachDuration * 0.6f;
        Vector3 currentPos = cameraTransform.localPosition;
        while (elapsed < withdrawDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / withdrawDuration;
            cameraTransform.localPosition = Vector3.Lerp(currentPos, originalCamLocalPos, t);
            yield return null;
        }

        RestoreCamera();
        UnfreezePlayer();
        isAnimating = false;
    }

    // Attempt 2: Reach further, touch handle, involuntary sound, "Not tonight."
    private IEnumerator ReachTouchWithdraw()
    {
        isAnimating = true;
        FreezePlayer();

        Vector3 targetPos = originalCamLocalPos + Vector3.forward * reachDistance;

        // Reach forward (further than attempt 1)
        float elapsed = 0f;
        float reachTime = reachDuration * 0.4f;
        while (elapsed < reachTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / reachTime;
            cameraTransform.localPosition = Vector3.Lerp(originalCamLocalPos, targetPos, t);
            yield return null;
        }

        // Touch — play involuntary sound
        if (involuntarySound != null && audioSource != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(involuntarySound);
        }

        // Show text
        if (ExamineUI.Instance != null)
            ExamineUI.Instance.Show("Not tonight.", 2.5f);

        // Hold at door
        yield return new WaitForSeconds(1f);

        // Withdraw
        elapsed = 0f;
        float withdrawDuration = reachDuration * 0.6f;
        Vector3 currentPos = cameraTransform.localPosition;
        while (elapsed < withdrawDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / withdrawDuration;
            cameraTransform.localPosition = Vector3.Lerp(currentPos, originalCamLocalPos, t);
            yield return null;
        }

        RestoreCamera();
        UnfreezePlayer();
        isAnimating = false;
    }

    // Attempt 3: Camera dips down, "Not ever.", permanent lockout.
    private IEnumerator FinalAttempt()
    {
        isAnimating = true;
        FreezePlayer();

        // Camera dips down to look at floor
        Quaternion downRot = originalCamLocalRot * Quaternion.Euler(dipAngle, 0f, 0f);

        float elapsed = 0f;
        float dipTime = dipDuration * 0.4f;
        while (elapsed < dipTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dipTime;
            cameraTransform.localRotation = Quaternion.Lerp(originalCamLocalRot, downRot, t);
            yield return null;
        }

        // Show text
        if (ExamineUI.Instance != null)
            ExamineUI.Instance.Show("Not ever.", 3f);

        // Hold looking down
        yield return new WaitForSeconds(2.5f);

        // Slowly look back up
        elapsed = 0f;
        float returnTime = dipDuration * 0.6f;
        Quaternion currentRot = cameraTransform.localRotation;
        while (elapsed < returnTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnTime;
            cameraTransform.localRotation = Quaternion.Lerp(currentRot, originalCamLocalRot, t);
            yield return null;
        }

        RestoreCamera();
        UnfreezePlayer();
        isAnimating = false;

        // Permanently disable interaction
        gameObject.tag = "Untagged";
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
}
