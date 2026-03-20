using UnityEngine;
using UnityEngine.Events;

namespace FAILSAFE.Interaction
{
    /// <summary>
    /// Base component for any object the player can interact with.
    /// Requires the GameObject to have the "Interactable" tag.
    /// Receives callbacks from Interact.cs via SendMessage.
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private string promptText = "Interact";
        [SerializeField] private bool singleUse = false;
        [SerializeField] private bool requiresItem = false;
        [SerializeField] private string requiredItemID = "";

        [Header("Events")]
        public UnityEvent onInteract;
        public UnityEvent onHover;
        public UnityEvent onUnhover;

        private bool _used = false;

        public string PromptText => promptText;

        // Called by Interact.cs via SendMessage
        private void Interacting()
        {
            if (singleUse && _used) return;

            if (requiresItem)
            {
                // TODO: Check PlayerInventory for requiredItemID
            }

            _used = true;
            OnInteract();
            onInteract?.Invoke();
        }

        private void Hovering()  => onHover?.Invoke();
        private void UnHover()   => onUnhover?.Invoke();

        /// <summary>Override in subclasses to define interaction behaviour.</summary>
        protected virtual void OnInteract() { }
    }
}
