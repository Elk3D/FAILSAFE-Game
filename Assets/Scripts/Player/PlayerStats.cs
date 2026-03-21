using UnityEngine;
using UnityEngine.Events;

namespace FAILSAFE.Player
{
    /// <summary>
    /// Tracks player state: health, stamina, sanity (if applicable), and death.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegenRate = 15f;
        [SerializeField] private float currentStamina;

        [Header("Events")]
        public UnityEvent<float> onHealthChanged;
        public UnityEvent<float> onStaminaChanged;
        public UnityEvent onDeath;

        public float HealthPercent => currentHealth / maxHealth;
        public float StaminaPercent => currentStamina / maxStamina;

        private void Awake()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
        }

        private void Update()
        {
            RegenerateStamina();
        }

        public void TakeDamage(float amount)
        {
            currentHealth = Mathf.Max(0, currentHealth - amount);
            onHealthChanged?.Invoke(HealthPercent);
            if (currentHealth <= 0) onDeath?.Invoke();
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            onHealthChanged?.Invoke(HealthPercent);
        }

        public bool UseStamina(float amount)
        {
            if (currentStamina < amount) return false;
            currentStamina -= amount;
            onStaminaChanged?.Invoke(StaminaPercent);
            return true;
        }

        private void RegenerateStamina()
        {
            if (currentStamina < maxStamina)
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
                onStaminaChanged?.Invoke(StaminaPercent);
            }
        }
    }
}
