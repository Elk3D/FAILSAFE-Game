using UnityEngine;
using TMPro;
using System.Collections;

namespace FAILSAFE.UI
{
    /// <summary>
    /// Controls the in-game HUD: item pickup notifications, objective hints,
    /// and crosshair state. Health/stamina bars if applicable.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Notifications")]
        [SerializeField] private TMP_Text notificationText;
        [SerializeField] private CanvasGroup notificationGroup;
        [SerializeField] private float notificationDuration = 3f;
        [SerializeField] private float fadeDuration = 0.5f;

        [Header("Crosshair")]
        [SerializeField] private GameObject crosshairDefault;
        [SerializeField] private GameObject crosshairInteract;

        private Coroutine _notifRoutine;

        public void ShowNotification(string message)
        {
            if (_notifRoutine != null) StopCoroutine(_notifRoutine);
            _notifRoutine = StartCoroutine(NotificationRoutine(message));
        }

        public void SetCrosshairInteractable(bool interactable)
        {
            crosshairDefault.SetActive(!interactable);
            crosshairInteract.SetActive(interactable);
        }

        private IEnumerator NotificationRoutine(string message)
        {
            notificationText.text = message;
            notificationGroup.alpha = 1f;
            yield return new WaitForSeconds(notificationDuration);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                notificationGroup.alpha = 1f - t;
                yield return null;
            }
            notificationGroup.alpha = 0f;
        }
    }
}
