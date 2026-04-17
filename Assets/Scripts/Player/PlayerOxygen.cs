using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float drainPerSecond = 2.2f;

    private float hazardDrainMultiplier = 1f;

    public float CurrentOxygen { get; private set; }
    public float NormalizedOxygen => maxOxygen <= 0f ? 0f : CurrentOxygen / maxOxygen;

    private void Start()
    {
        CurrentOxygen = maxOxygen;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
        {
            return;
        }

        CurrentOxygen = Mathf.Max(0f, CurrentOxygen - drainPerSecond * hazardDrainMultiplier * Time.deltaTime);
        HUDController.Instance?.SetOxygen(NormalizedOxygen, CurrentOxygen);

        if (CurrentOxygen <= 0f)
        {
            GameManager.Instance?.Lose("Suit oxygen depleted");
        }
    }

    public void AddOxygen(float amount)
    {
        CurrentOxygen = Mathf.Clamp(CurrentOxygen + amount, 0f, maxOxygen);
        HUDController.Instance?.SetOxygen(NormalizedOxygen, CurrentOxygen);
    }

    public void SetHazardMultiplier(float multiplier)
    {
        hazardDrainMultiplier = Mathf.Max(1f, multiplier);
    }
}
