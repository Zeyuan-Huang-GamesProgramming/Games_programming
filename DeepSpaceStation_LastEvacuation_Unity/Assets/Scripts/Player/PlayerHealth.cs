using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float NormalizedHealth => maxHealth <= 0f ? 0f : CurrentHealth / maxHealth;

    private void Start()
    {
        CurrentHealth = maxHealth;
        HUDController.Instance?.SetHealth(NormalizedHealth, CurrentHealth);
    }

    public void TakeDamage(float amount)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        HUDController.Instance?.SetHealth(NormalizedHealth, CurrentHealth);

        if (CurrentHealth <= 0f)
        {
            GameManager.Instance?.Lose("Suit integrity failed");
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
        HUDController.Instance?.SetHealth(NormalizedHealth, CurrentHealth);
    }
}
