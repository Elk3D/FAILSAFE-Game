using System.Collections;
using UnityEngine;

/*
    FrontDoorInteraction.cs — Mail Pile Examination

    The front door is locked (deadbolt + chain). Player examines the pile of
    mail that's accumulated. Each interaction cycles through specific items:
    overdue electric bill, building notice, grocery circular, and the grief book.

    SETUP:
    - Place on front door or mail pile GameObject with a Collider
    - Tag as "Interactable"
    - Set examineTexts in Inspector (default values provided)
    - Requires ExamineUI to exist in the scene
*/

public class FrontDoorInteraction : MonoBehaviour
{
    [Header("Examine Settings")]
    [Tooltip("Hover prompt text")]
    public string hoverPrompt = "Examine Mail";

    [Tooltip("Cycleable examine texts — each interaction shows the next one")]
    [TextArea(2, 5)]
    public string[] examineTexts = {
        "Deadbolt locked. Chain engaged. Weeks of mail piled against the door.",
        "Overdue electric bill. Final notice. The date is three months old.",
        "Building management notice. 'Noise complaint filed by unit 4B.' Never opened.",
        "Grocery store circular. Coupons expired. Everything in it expired.",
        "'The Grief of Responsibility' — self-help book. Ordered two months ago. Spine uncracked."
    };

    private int currentIndex = 0;
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
        if (examineTexts.Length == 0) return;

        ExamineUI.Instance.Show(examineTexts[currentIndex], 4f);
        currentIndex = (currentIndex + 1) % examineTexts.Length;

        // After first interaction, update prompt to indicate more items
        if (currentIndex != 0)
            hoverPrompt = "Examine Next";
        else
            hoverPrompt = "Examine Mail";
    }
}
