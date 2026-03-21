using System.Collections;
using UnityEngine;

namespace FAILSAFE.Audio
{
    /// <summary>
    /// Crossfades between music tracks. Attach to the persistent audio GameObject.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource sourceA;
        [SerializeField] private AudioSource sourceB;
        [SerializeField] private float crossfadeDuration = 2f;

        private bool _usingA = true;

        private AudioSource Active  => _usingA ? sourceA : sourceB;
        private AudioSource Passive => _usingA ? sourceB : sourceA;

        public void CrossfadeTo(AudioClip newClip, bool loop = true)
        {
            if (Active.clip == newClip) return;
            Passive.clip = newClip;
            Passive.loop = loop;
            Passive.volume = 0f;
            Passive.Play();
            StartCoroutine(DoCrossfade());
        }

        private IEnumerator DoCrossfade()
        {
            float t = 0f;
            float startVol = Active.volume;
            while (t < 1f)
            {
                t += Time.deltaTime / crossfadeDuration;
                Active.volume  = Mathf.Lerp(startVol, 0f, t);
                Passive.volume = Mathf.Lerp(0f, startVol, t);
                yield return null;
            }
            Active.Stop();
            _usingA = !_usingA;
        }
    }
}
