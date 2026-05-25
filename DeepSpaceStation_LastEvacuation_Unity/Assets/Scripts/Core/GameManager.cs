using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int requiredRepairs = 7;
    [SerializeField] private float evacuationTimeLimit = 620f;
    [SerializeField] private float instabilityGrowthPerSecond = 0.0017f;
    [SerializeField] private float reactorInstabilityBonus = 0.0012f;
    [SerializeField] private float firstRadiationPulseDelay = 42f;
    [SerializeField] private float radiationPulseInterval = 78f;
    [SerializeField] private float radiationPulseDuration = 16f;

    private int completedRepairs;
    private float remainingTime;
    private float stationInstability;
    private float radiationPulseTimer;
    private float radiationPulseRemaining;
    private bool lifeSupportOnline;
    private bool reactorOnline;
    private int detectionCount;

    public bool IsGameEnded { get; private set; }
    public bool IsPaused { get; private set; }
    public bool AllRepairsComplete => completedRepairs >= requiredRepairs;
    public int CompletedRepairs => completedRepairs;
    public int RequiredRepairs => requiredRepairs;
    public float GlobalOxygenDrainMultiplier { get; private set; } = 1f;
    public float StationInstability => stationInstability;
    public bool RadiationPulseActive => radiationPulseRemaining > 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        remainingTime = evacuationTimeLimit;
        radiationPulseTimer = firstRadiationPulseDelay;
    }

    private void Start()
    {
        HUDController.Instance?.SetObjective("Open the pod bay exit with E, restore corridor systems, then unlock deeper rooms.");
        HUDController.Instance?.SetRepairs(completedRepairs, requiredRepairs);
        HUDController.Instance?.SetPressure(GlobalOxygenDrainMultiplier, stationInstability, false);
        HUDController.Instance?.SetInventory("No items");
        HUDController.Instance?.ShowMessage("Open the pod bay bulkhead with E. The station gets worse over time.", 4f);
    }

    private void Update()
    {
        if (IsGameEnded)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!IsPaused);
        }

        if (IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartScene();
            }

            return;
        }

        remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
        UpdateStationPressure();
        HUDController.Instance?.SetTimer(remainingTime);
        HUDController.Instance?.SetPressure(GlobalOxygenDrainMultiplier, stationInstability, RadiationPulseActive);

        if (remainingTime <= 0f)
        {
            Lose("Evacuation window closed");
        }
    }

    public void RegisterRepair(string systemName)
    {
        if (IsGameEnded)
        {
            return;
        }

        completedRepairs = Mathf.Clamp(completedRepairs + 1, 0, requiredRepairs);
        MarkSystemOnline(systemName);
        stationInstability = Mathf.Max(0f, stationInstability - 0.08f);
        HUDController.Instance?.SetRepairs(completedRepairs, requiredRepairs);
        HUDController.Instance?.ShowMessage(systemName + " restored", 3f);

        if (AllRepairsComplete)
        {
            HUDController.Instance?.SetObjective("All systems restored. Reach the escape pod.");
            HUDController.Instance?.ShowMessage("Escape pod authorization unlocked", 4f);
        }
        else
        {
            HUDController.Instance?.SetObjective("Systems restored: " + completedRepairs + "/" + requiredRepairs + ". Keep moving before oxygen pressure spikes.");
        }
    }

    public void AddInstability(float amount, string reason)
    {
        if (IsGameEnded)
        {
            return;
        }

        stationInstability = Mathf.Clamp01(stationInstability + amount);
        UpdateStationPressure();

        if (!string.IsNullOrEmpty(reason))
        {
            HUDController.Instance?.ShowMessage(reason, 2.5f);
        }
    }

    public void Win()
    {
        if (IsGameEnded)
        {
            return;
        }

        IsGameEnded = true;
        Time.timeScale = 1f;
        UnlockCursor();
        HUDController.Instance?.ShowEndScreen("EVACUATION SUCCESSFUL", "You restored the derelict station long enough to launch.\nDetections: " + detectionCount);
    }

    public void Lose(string reason)
    {
        if (IsGameEnded)
        {
            return;
        }

        IsGameEnded = true;
        Time.timeScale = 1f;
        UnlockCursor();
        HUDController.Instance?.ShowEndScreen("MISSION FAILED", reason);
    }

    private static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetPaused(bool paused)
    {
        if (IsGameEnded)
        {
            return;
        }

        IsPaused = paused;
        Time.timeScale = IsPaused ? 0f : 1f;
        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsPaused;
        HUDController.Instance?.SetPauseOpen(IsPaused);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void RegisterDetection()
    {
        detectionCount++;
        HUDController.Instance?.ShowMessage("Security robot detected you", 2f);
    }

    private void UpdateStationPressure()
    {
        float growth = instabilityGrowthPerSecond;
        if (!reactorOnline)
        {
            growth += reactorInstabilityBonus;
        }

        if (lifeSupportOnline)
        {
            growth *= 0.42f;
        }

        stationInstability = Mathf.Clamp01(stationInstability + growth * Time.deltaTime);

        if (radiationPulseRemaining > 0f)
        {
            radiationPulseRemaining = Mathf.Max(0f, radiationPulseRemaining - Time.deltaTime);
        }
        else
        {
            radiationPulseTimer -= Time.deltaTime;
            if (radiationPulseTimer <= 0f && !reactorOnline)
            {
                radiationPulseRemaining = radiationPulseDuration;
                radiationPulseTimer = radiationPulseInterval;
                AddInstability(0.06f, "Radiation surge: global oxygen drain increased");
            }
        }

        float multiplier = Mathf.Lerp(1.05f, 2.05f, stationInstability);
        if (!lifeSupportOnline)
        {
            multiplier += 0.28f;
        }

        if (!reactorOnline)
        {
            multiplier += 0.18f;
        }

        if (RadiationPulseActive)
        {
            multiplier += 0.75f;
        }

        if (lifeSupportOnline)
        {
            multiplier *= 0.82f;
        }

        GlobalOxygenDrainMultiplier = Mathf.Clamp(multiplier, 0.8f, 3.1f);
    }

    private void MarkSystemOnline(string systemName)
    {
        string lower = systemName.ToLowerInvariant();
        if (lower.Contains("life support"))
        {
            lifeSupportOnline = true;
        }

        if (lower.Contains("reactor"))
        {
            reactorOnline = true;
            radiationPulseRemaining = 0f;
            radiationPulseTimer = radiationPulseInterval * 1.4f;
        }
    }
}
