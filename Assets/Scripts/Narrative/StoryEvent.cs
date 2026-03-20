using UnityEngine;
using UnityEngine.Events;

namespace FAILSAFE.Narrative
{
    /// <summary>
    /// A discrete story beat that can be fired from anywhere.
    /// Useful for tracking which narrative milestones the player has reached.
    /// </summary>
    [CreateAssetMenu(fileName = "StoryEvent", menuName = "FAILSAFE/Narrative/Story Event")]
    public class StoryEvent : ScriptableObject
    {
        [TextArea(2, 4)] public string description;

        private event System.Action _listeners;

        public void Raise() => _listeners?.Invoke();
        public void Register(System.Action listener) => _listeners += listener;
        public void Unregister(System.Action listener) => _listeners -= listener;
    }
}
