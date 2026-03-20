using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FAILSAFE.Player
{
    /// <summary>
    /// Manages the player's collected items. Items are referenced by string ID
    /// so they survive scene loads without holding asset references.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        private readonly HashSet<string> _collectedItems = new HashSet<string>();

        public UnityEvent<string> onItemAdded;
        public UnityEvent<string> onItemRemoved;

        public bool HasItem(string itemID) => _collectedItems.Contains(itemID);

        public bool AddItem(string itemID)
        {
            if (!_collectedItems.Add(itemID)) return false;
            onItemAdded?.Invoke(itemID);
            return true;
        }

        public bool RemoveItem(string itemID)
        {
            if (!_collectedItems.Remove(itemID)) return false;
            onItemRemoved?.Invoke(itemID);
            return true;
        }

        public string[] GetAllItems() => new List<string>(_collectedItems).ToArray();

        public void LoadFromSave(string[] itemIDs)
        {
            _collectedItems.Clear();
            if (itemIDs != null)
                foreach (var id in itemIDs) _collectedItems.Add(id);
        }
    }
}
