using System.Collections;
using UnityEngine;

/*
    MirrorAnomaly.cs — 2-Frame Subliminal Reflection Change

    When the player enters the trigger zone in front of the bathroom mirror and
    looks directly at it (raycast check), the mirror's material swaps to an
    anomaly material for exactly 2 frames, then swaps back. One-time only.
    No sound, no UI, no acknowledgment. Pure subliminal dread.

    SETUP:
    - Create an empty GameObject in front of the bathroom mirror
    - Add Box Collider, set isTrigger = true, size to cover mirror viewing area
    - DO NOT tag as "Interactable" — this is trigger-based, not interaction-based
    - Assign normalReflectionMaterial (default mirror material)
    - Assign anomalyReflectionMaterial (the disturbing 2-frame flash)
    - Assign mirrorRenderer (the mirror mesh Renderer)
    - Player GameObject must have tag "Player"
*/

public class MirrorAnomaly : MonoBehaviour
{
    [Header("Materials")]
    [Tooltip("Normal mirror/reflection material")]
    public Material normalReflectionMaterial;
    [Tooltip("Anomaly material shown for exactly 2 frames")]
    public Material anomalyReflectionMaterial;

    [Header("References")]
    [Tooltip("The mirror mesh Renderer")]
    public Renderer mirrorRenderer;

    private bool hasTriggered = false;
    private Camera mainCam;

    void Start()
    {
        GameObject camObj = GameObject.FindWithTag("MainCamera");
        if (camObj == null)
            camObj = GameObject.FindObjectOfType<Camera>().gameObject;
        mainCam = camObj.GetComponent<Camera>();
    }

    void OnTriggerStay(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        // Check if player is looking at the mirror
        RaycastHit hit;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, 5f))
        {
            if (hit.collider.gameObject == mirrorRenderer.gameObject)
            {
                hasTriggered = true;
                StartCoroutine(AnomalyFlash());
            }
        }
    }

    private IEnumerator AnomalyFlash()
    {
        // Swap to anomaly material
        mirrorRenderer.material = anomalyReflectionMaterial;

        // Hold for exactly 2 frames
        yield return null; // Frame 1
        yield return null; // Frame 2

        // Revert to normal
        mirrorRenderer.material = normalReflectionMaterial;
    }
}
