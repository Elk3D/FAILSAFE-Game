using UnityEngine;
using FAILSAFE.Interaction;

namespace FAILSAFE.Gameplay.Doors
{
    /// <summary>
    /// Base door. Plays an open/close animation and optional audio.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Door : Interactable
    {
        [Header("Door")]
        [SerializeField] protected AudioClip openSFX;
        [SerializeField] protected AudioClip closeSFX;
        [SerializeField] protected bool startOpen = false;

        protected Animator _animator;
        protected bool _isOpen;

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();
            _isOpen = startOpen;
        }

        protected override void OnInteract() => Toggle();

        public virtual void Open()
        {
            if (_isOpen) return;
            _isOpen = true;
            _animator.SetBool(Utilities.Constants.ANIM_OPEN, true);
            PlayAudio(openSFX);
        }

        public virtual void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;
            _animator.SetBool(Utilities.Constants.ANIM_OPEN, false);
            PlayAudio(closeSFX);
        }

        public void Toggle() { if (_isOpen) Close(); else Open(); }

        private void PlayAudio(AudioClip clip)
        {
            if (clip != null)
                Managers.AudioManager.Instance?.PlaySFX(clip);
        }
    }
}
