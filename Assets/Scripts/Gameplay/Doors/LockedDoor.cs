using UnityEngine;

namespace FAILSAFE.Gameplay.Doors
{
    /// <summary>
    /// A door that requires a specific item (key/keycard/code) before it can be opened.
    /// </summary>
    public class LockedDoor : Door
    {
        [Header("Lock")]
        [SerializeField] private string requiredItemID;
        [SerializeField] private string lockedMessage = "It's locked.";
        [SerializeField] private AudioClip lockedSFX;

        protected override void OnInteract()
        {
            var inventory = FindObjectOfType<Player.PlayerInventory>();
            if (inventory != null && inventory.HasItem(requiredItemID))
            {
                inventory.RemoveItem(requiredItemID);
                Open();
            }
            else
            {
                // TODO: Show HUD message with lockedMessage
                Managers.AudioManager.Instance?.PlaySFX(lockedSFX);
            }
        }
    }
}
