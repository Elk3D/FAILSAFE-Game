using System.Collections;
using UnityEngine;

namespace FAILSAFE.Gameplay.Doors
{
    /// <summary>
    /// Slides open/closed along a local axis using a Coroutine (no Animator required).
    /// Useful for sci-fi/facility environments.
    /// </summary>
    public class SlidingDoor : Interactable
    {
        [Header("Slide Settings")]
        [SerializeField] private Vector3 openOffset = new Vector3(2f, 0f, 0f);
        [SerializeField] private float slideSpeed = 2f;
        [SerializeField] private AudioClip openSFX;
        [SerializeField] private AudioClip closeSFX;

        private Vector3 _closedPosition;
        private Vector3 _openPosition;
        private bool _isOpen;
        private Coroutine _slideRoutine;

        private void Start()
        {
            _closedPosition = transform.localPosition;
            _openPosition = _closedPosition + openOffset;
        }

        protected override void OnInteract()
        {
            if (_slideRoutine != null) StopCoroutine(_slideRoutine);
            _isOpen = !_isOpen;
            var target = _isOpen ? _openPosition : _closedPosition;
            _slideRoutine = StartCoroutine(SlideTo(target));
            Managers.AudioManager.Instance?.PlaySFX(_isOpen ? openSFX : closeSFX);
        }

        private IEnumerator SlideTo(Vector3 target)
        {
            while (Vector3.Distance(transform.localPosition, target) > 0.001f)
            {
                transform.localPosition = Vector3.MoveTowards(
                    transform.localPosition, target, slideSpeed * Time.deltaTime);
                yield return null;
            }
            transform.localPosition = target;
        }
    }
}
