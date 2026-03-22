using System.Collections;
using UnityEngine;

/*
    ExamineObject.cs — Generic Examine Text Display

    For objects that just display text when interacted with (couch, kitchen sink,
    spice rack, fridge, calendar, coat hook, grief book, etc.).

    SETUP:
    - Place on any GameObject with a Collider
    - Tag the GameObject as "Interactable"
    - Set examineTexts in Inspector (one or more lines)
    - Set hoverPrompt for the hover UI text
    - Requires ExamineUI to exist in the scene
*/

public class ExamineObject : MonoBehaviour
{
    [Header("Examine Settings")]
    [Tooltip("Text lines to display when examined. Multiple lines are joined with line breaks.")]
    [TextArea(2, 5)]
    public string[] examineTexts = { "Nothing interesting." };

    [Tooltip("Text shown when hovering over the object")]
    public string hoverPrompt = "Examine";

    [Tooltip("How long the examine text stays on screen before fading")]
    public float displayDuration = 4f;

    private GameObject MainCam;
    private Interact InteractionScript;

    void Start()
    {
        MainCam = GameObject.FindWithTag("MainCamera");
        if (MainCam == null)
            MainCam = GameObject.FindObjectOfType<Camera>().gameObject;
        InteractionScript = MainCam.GetComponent<Interact>();
    }

    public void Hovering()
    {
        InteractionScript.message = hoverPrompt;
    }

    public void Interacting()
    {
        if (ExamineUI.Instance == null) return;

        string fullText = string.Join("\n", examineTexts);
        ExamineUI.Instance.Show(fullText, displayDuration);
    }
}
