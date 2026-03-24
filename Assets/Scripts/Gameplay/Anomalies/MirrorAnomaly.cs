using UnityEngine;

namespace FAILSAFE.Gameplay.Anomalies
{
    /// <summary>
    /// Mirror Anomaly — the reflection doesn't show Isaac.
    /// Instead, an abstract scribble sprite (mental health metaphor) mimics
    /// his lateral movement, with red glowing eyes as the anomaly tell.
    ///
    /// Prefab hierarchy expected:
    ///   MirrorAnomaly (this script on MirrorSurface Quad)
    ///   └── ReflectionRoot
    ///       ├── ScribbleSprite  (SpriteRenderer + Animator)
    ///       └── EyesSprite      (SpriteRenderer, layer above scribble)
    /// </summary>
    public class MirrorAnomaly : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Animator reflectionAnimator;
        [SerializeField] private SpriteRenderer eyesRenderer;
        [SerializeField] private Transform reflectionRoot;

        [Header("Movement Mirroring")]
        [Tooltip("How far the reflection shifts horizontally per unit of player offset.")]
        [SerializeField] private float horizontalScale = 1f;
        [Tooltip("Lerp speed for the reflection following player movement.")]
        [SerializeField] private float smoothing = 8f;

        [Header("Eyes")]
        [SerializeField] private float eyePulseSpeed = 2f;
        [SerializeField] private float eyeMinAlpha = 0.6f;
        [SerializeField] private float eyeMaxAlpha = 1.0f;
        [Tooltip("Distance at which the eyes lock to full intensity and scale up.")]
        [SerializeField] private float eyeIntensifyDistance = 3f;
        [SerializeField] private float eyeStareScale = 1.3f;

        // Animator parameter hashes
        private static readonly int ParamIsWalking  = Animator.StringToHash("IsWalking");
        private static readonly int ParamVelocityX  = Animator.StringToHash("VelocityX");

        private CharacterController _playerCC;
        private Vector3 _reflectionBaseLocalPos;

        private void Start()
        {
            if (player != null)
                _playerCC = player.GetComponent<CharacterController>();

            if (reflectionRoot != null)
                _reflectionBaseLocalPos = reflectionRoot.localPosition;
        }

        private void Update()
        {
            if (player == null) return;

            MirrorMovement();
            DriveAnimator();
            PulseEyes();
        }

        /// <summary>
        /// Offset the reflection sprite opposite to the player's lateral position
        /// relative to the mirror centre, so it feels like a true mirror.
        /// </summary>
        private void MirrorMovement()
        {
            // Player X in the mirror's local space — positive means player is to the right.
            float playerLocalX = transform.InverseTransformPoint(player.position).x;

            // Negate: player right → reflection left.
            float targetX = -playerLocalX * horizontalScale;

            Vector3 target = _reflectionBaseLocalPos;
            target.x = targetX;
            reflectionRoot.localPosition = Vector3.Lerp(
                reflectionRoot.localPosition,
                target,
                Time.deltaTime * smoothing
            );
        }

        /// <summary>
        /// Drive the Animator from the player's actual velocity (negated for the mirror).
        /// </summary>
        private void DriveAnimator()
        {
            if (reflectionAnimator == null) return;

            float velocityX = _playerCC != null ? _playerCC.velocity.x : 0f;

            // Negate so the reflection walks opposite to Isaac.
            reflectionAnimator.SetFloat(ParamVelocityX, -velocityX);
            reflectionAnimator.SetBool(ParamIsWalking, Mathf.Abs(velocityX) > 0.1f);
        }

        /// <summary>
        /// Pulse the red eyes with a sine wave; intensify when the player is close.
        /// </summary>
        private void PulseEyes()
        {
            if (eyesRenderer == null) return;

            float dist = Vector3.Distance(player.position, transform.position);
            bool isClose = dist < eyeIntensifyDistance;

            float alpha;
            float scale;

            if (isClose)
            {
                // Lock to max intensity when the player is near — stare effect.
                alpha = eyeMaxAlpha;
                scale = eyeStareScale;
            }
            else
            {
                float t = (Mathf.Sin(Time.time * eyePulseSpeed) + 1f) * 0.5f;
                alpha = Mathf.Lerp(eyeMinAlpha, eyeMaxAlpha, t);
                scale = 1f;
            }

            Color c = eyesRenderer.color;
            c.a = alpha;
            eyesRenderer.color = c;
            eyesRenderer.transform.localScale = Vector3.one * scale;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw the intensify-distance sphere so designers can tune it in scene view.
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawSphere(transform.position, eyeIntensifyDistance);
        }
#endif
    }
}
