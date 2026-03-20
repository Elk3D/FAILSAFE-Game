using UnityEngine;

namespace FAILSAFE.Interaction
{
    /// <summary>
    /// An interactable that shows a close-up examination view and optional
    /// descriptive text when the player looks at it.
    /// </summary>
    public class ExaminableObject : Interactable
    {
        [Header("Examination")]
        [SerializeField] private string examinationText;
        [TextArea(3, 8)]
        [SerializeField] private string detailedDescription;
        [SerializeField] private AudioClip examineSFX;

        protected override void OnInteract()
        {
            // TODO: Show examination UI with detailedDescription
            // TODO: Optionally rotate object in examine camera
        }
    }
}
