using UnityEngine;

/// <summary>
/// Collectable assignment improvement that temporarily increases the player's fire rate.
/// </summary>
public class RapidFirePowerUp : MonoBehaviour
{
    [Tooltip("How long the rapid fire effect lasts.")]
    public float duration = 6f;
    [Tooltip("Multiplier applied to the player's fire rate. Lower means faster shots.")]
    public float fireRateMultiplier = 0.35f;
    [Tooltip("How quickly the pick-up spins.")]
    public float spinSpeed = 120f;
    [Tooltip("How much the pick-up pulses while waiting.")]
    public float pulseAmount = 0.15f;

    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        float pulse = 1f + Mathf.Sin(Time.time * 5f) * pulseAmount;
        transform.localScale = startScale * pulse;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool hitPlayer = other.CompareTag("Player") || other.GetComponentInParent<Controller>() != null;
        if (!hitPlayer || GameManager.instance == null)
        {
            return;
        }

        GameManager.instance.ActivateRapidFirePowerUp(duration, fireRateMultiplier);
        GameManager.instance.NotifyPowerUpCollected();
        Destroy(gameObject);
    }
}
