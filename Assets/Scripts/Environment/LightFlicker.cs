using System.Collections;
using UnityEngine;

namespace FAILSAFE.Environment
{
    /// <summary>
    /// Makes a Light component flicker irregularly for atmospheric effect.
    /// Useful for broken fluorescent lights, candles, or faulty equipment.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        [Header("Flicker")]
        [SerializeField] private float minIntensity = 0f;
        [SerializeField] private float maxIntensity = 1.5f;
        [SerializeField] private float minDelay = 0.05f;
        [SerializeField] private float maxDelay = 0.3f;
        [SerializeField] private float flickerProbability = 0.4f;

        private Light _light;
        private float _baseIntensity;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _baseIntensity = _light.intensity;
        }

        private void OnEnable()  => StartCoroutine(Flicker());
        private void OnDisable() => StopAllCoroutines();

        private IEnumerator Flicker()
        {
            while (true)
            {
                if (Random.value < flickerProbability)
                    _light.intensity = Random.Range(minIntensity, maxIntensity);
                else
                    _light.intensity = _baseIntensity;

                yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            }
        }
    }
}
