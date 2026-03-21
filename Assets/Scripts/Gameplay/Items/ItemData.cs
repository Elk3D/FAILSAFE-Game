using UnityEngine;

namespace FAILSAFE.Gameplay.Items
{
    /// <summary>
    /// ScriptableObject that defines an item's metadata.
    /// Referenced by ID strings throughout the codebase.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "FAILSAFE/Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public string itemID;
        public string displayName;
        [TextArea(2, 4)] public string description;

        [Header("Visuals")]
        public Sprite icon;
        public GameObject worldPrefab;

        [Header("Notes")]
        [TextArea(4, 10)] public string noteContent;
        public bool isReadable;
    }
}
