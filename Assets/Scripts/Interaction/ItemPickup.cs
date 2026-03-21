using UnityEngine;
using FAILSAFE.Managers;

namespace FAILSAFE.Interaction
{
    /// <summary>
    /// Adds an item to the player's inventory on interaction, then disables itself.
    /// </summary>
    public class ItemPickup : Interactable
    {
        [Header("Item")]
        [SerializeField] private string itemID;
        [SerializeField] private string itemName;
        [TextArea(2, 4)]
        [SerializeField] private string pickupMessage;
        [SerializeField] private AudioClip pickupSFX;

        protected override void OnInteract()
        {
            var inventory = FindObjectOfType<Player.PlayerInventory>();
            if (inventory == null) return;

            if (inventory.AddItem(itemID))
            {
                // TODO: Show HUD notification with itemName / pickupMessage
                if (pickupSFX != null)
                    AudioManager.Instance?.PlaySFX(pickupSFX);

                gameObject.SetActive(false);
            }
        }
    }
}
