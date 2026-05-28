using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float drainPerSecond = 0.42f;

    private float localDrainMultiplier = 1f;

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

        float globalMultiplier = GameManager.Instance == null ? 1f : GameManager.Instance.GlobalOxygenDrainMultiplier;
        CurrentOxygen = Mathf.Max(0f, CurrentOxygen - drainPerSecond * localDrainMultiplier * globalMultiplier * Time.deltaTime);
        HUDController.Instance?.SetOxygen(NormalizedOxygen, CurrentOxygen);
        GameAudio.Instance?.UpdateOxygenWarning(NormalizedOxygen);

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
        localDrainMultiplier = Mathf.Max(1f, multiplier);
    }
}
