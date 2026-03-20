using UnityEngine;

namespace FAILSAFE.Environment
{
    /// <summary>
    /// Marks a ladder volume. PlayerMovement.cs detects the "Ladder" tag
    /// and switches to ladder-climbing physics within this collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Ladder : MonoBehaviour
    {
        [Header("Ladder")]
        [SerializeField] private float climbSpeed = 3f;

        public float ClimbSpeed => climbSpeed;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            gameObject.tag = Utilities.Constants.TAG_LADDER;
        }
    }
}
