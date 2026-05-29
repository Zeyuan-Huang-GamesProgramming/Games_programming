using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string EndlessHighScoreKey = "DSS.EndlessHighScore";
    private const string EndlessLongestRunKey = "DSS.EndlessLongestRun";
    private const string TimedBestRatingKey = "DSS.TimedBestRating";
    private const string TimedFastestEscapeKey = "DSS.TimedFastestEscape";

    public enum GameMode
    {
        TimedEvacuation,
        EndlessSurvival
    }

    public static GameManager Instance { get; private set; }

    [SerializeField] private int requiredRepairs = 7;
    [SerializeField] private float evacuationTimeLimit = 620f;
    [SerializeField] private float instabilityGrowthPerSecond = 0.0017f;
    [SerializeField] private float reactorInstabilityBonus = 0.0012f;
    [SerializeField] private float firstRadiationPulseDelay = 42f;
    [SerializeField] private float radiationPulseInterval = 78f;
    [SerializeField] private float radiationPulseDuration = 16f;
    [SerializeField] private int endlessBaseScorePerRepair = 100;
    [SerializeField] private int endlessScorePerThreatLevel = 300;
    [SerializeField] private float endlessThreatGrowthPerLevel = 0.14f;
    [SerializeField] private string timedSpawnPointName = "Timed Evacuation Spawn";
    [SerializeField] private string endlessSpawnPointName = "Endless Survival Spawn";

    private GameMode activeMode = GameMode.TimedEvacuation;
    private int completedRepairs;
    private float remainingTime;
    private float missionElapsedTime;
    private float stationInstability;
    private float radiationPulseTimer;
    private float radiationPulseRemaining;
    private bool lifeSupportOnline;
    private bool reactorOnline;
    private int detectionCount;
    private int endlessScore;
    private int endlessThreatLevel = 1;
    private bool waitingForModeSelection = true;

    public bool IsGameEnded { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsChoosingMode => waitingForModeSelection;
    public bool EndlessModeActive => !waitingForModeSelection && activeMode == GameMode.EndlessSurvival;
    public bool TimedModeActive => !waitingForModeSelection && activeMode == GameMode.TimedEvacuation;
    public bool AllRepairsComplete => TimedModeActive && completedRepairs >= requiredRepairs;
    public int CompletedRepairs => completedRepairs;
    public int RequiredRepairs => requiredRepairs;
    public int EndlessScore => endlessScore;
    public int EndlessThreatLevel => endlessThreatLevel;
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
        ShowModeSelection();
    }

    private void ShowModeSelection()
    {
        waitingForModeSelection = true;
        IsPaused = false;
        Time.timeScale = 0f;
        UnlockCursor();
        UpdateObjectiveText();
        HUDController.Instance?.SetRepairs(completedRepairs, requiredRepairs);
        HUDController.Instance?.SetPressure(GlobalOxygenDrainMultiplier, stationInstability, false);
        HUDController.Instance?.SetInventory("No items");
        HUDController.Instance?.SetModeSelectionOpen(true);
        HUDController.Instance?.SetRecordsSummary(BuildRecordSummary());
        HUDController.Instance?.ShowMessage("", 0f);
        GameAudio.Instance?.SetScannerActive(false);
    }

    private void Update()
    {
        if (waitingForModeSelection)
        {
            HUDController.Instance?.SetModeSelectionOpen(true);
            UnlockCursor();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartTimedEvacuationMode();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartEndlessMode();
            }

            return;
        }

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

        missionElapsedTime += Time.deltaTime;
        UpdateStationPressure();

        if (TimedModeActive)
        {
            remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
            HUDController.Instance?.SetTimer(remainingTime);
        }
        else
        {
            UpdateEndlessHUD();
        }

        HUDController.Instance?.SetPressure(GlobalOxygenDrainMultiplier, stationInstability, RadiationPulseActive);

        if (TimedModeActive && remainingTime <= 0f)
        {
            Lose("Evacuation window closed");
        }
    }

    public void StartTimedEvacuationMode()
    {
        BeginMode(GameMode.TimedEvacuation);
    }

    public void StartEndlessMode()
    {
        BeginMode(GameMode.EndlessSurvival);
    }

    private void BeginMode(GameMode mode)
    {
        activeMode = mode;
        waitingForModeSelection = false;
        IsPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HUDController.Instance?.SetModeSelectionOpen(false);
        GameAudio.Instance?.PlayMissionStart();

        if (EndlessModeActive)
        {
            MovePlayerToSpawn(endlessSpawnPointName);
            completedRepairs = 0;
            endlessScore = 0;
            endlessThreatLevel = 1;
            UpdateEndlessDifficulty();
            UpdateEndlessHUD();
            HUDController.Instance?.ShowBriefing(
                "SURVIVAL PROTOCOL ACTIVE",
                "Loop station repairs for score.\nEvery threat level increases robot pressure.",
                4.2f);
        }
        else
        {
            MovePlayerToSpawn(timedSpawnPointName);
            remainingTime = evacuationTimeLimit;
            HUDController.Instance?.SetTimer(remainingTime);
            HUDController.Instance?.SetRepairs(completedRepairs, requiredRepairs);
            HUDController.Instance?.ShowBriefing(
                "EVACUATION PROTOCOL ACTIVE",
                "Restore seven systems before the launch window closes.\nReach the escape pod after authorization.",
                4.2f);
        }

        UpdateObjectiveText();
    }

    private void MovePlayerToSpawn(string spawnName)
    {
        GameObject spawn = GameObject.Find(spawnName);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (spawn == null || player == null)
        {
            return;
        }

        CharacterController controller = player.GetComponent<CharacterController>();
        bool controllerWasEnabled = controller != null && controller.enabled;
        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);

        FirstPersonController movement = player.GetComponent<FirstPersonController>();
        if (movement != null)
        {
            movement.ResetView();
        }

        if (controller != null)
        {
            controller.enabled = controllerWasEnabled;
        }
    }

    public void RegisterRepair(string systemName)
    {
        if (IsGameEnded || waitingForModeSelection)
        {
            return;
        }

        int scoreGain = 0;
        bool threatRaised = false;
        if (EndlessModeActive)
        {
            completedRepairs++;
            scoreGain = CalculateEndlessRepairScore(systemName);
            endlessScore += scoreGain;
            threatRaised = UpdateEndlessDifficulty();
        }
        else
        {
            completedRepairs = Mathf.Clamp(completedRepairs + 1, 0, requiredRepairs);
        }

        string effectMessage = ApplySystemRepairEffect(systemName);
        stationInstability = Mathf.Max(0f, stationInstability - 0.08f);
        UpdateStationPressure();
        if (EndlessModeActive)
        {
            UpdateEndlessHUD();
        }
        else
        {
            HUDController.Instance?.SetRepairs(completedRepairs, requiredRepairs);
            HUDController.Instance?.SetTimer(remainingTime);
        }

        HUDController.Instance?.SetPressure(GlobalOxygenDrainMultiplier, stationInstability, RadiationPulseActive);

        string message = systemName + " restored";
        if (EndlessModeActive)
        {
            message += "\nScore +" + scoreGain + "  Total " + endlessScore;
        }

        if (!string.IsNullOrEmpty(effectMessage))
        {
            message += "\n" + effectMessage;
        }

        if (threatRaised)
        {
            message += "\nThreat level " + endlessThreatLevel + ": robots search harder.";
        }

        if (TimedModeActive && AllRepairsComplete)
        {
            message += "\nEscape pod authorization unlocked.";
        }

        HUDController.Instance?.ShowMessage(message, AllRepairsComplete ? 5f : 4f);
        GameAudio.Instance?.PlayRepairComplete();
        UpdateObjectiveText();
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
            if (reason.StartsWith("Radiation"))
            {
                GameAudio.Instance?.PlayWarningPulse();
            }
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
        string recordBanner = CommitRunRecords(true);
        GameAudio.Instance?.SetScannerActive(false);
        GameAudio.Instance?.PlayVictory();
        HUDController.Instance?.ShowEndScreen("EVACUATION SUCCESSFUL", BuildEndReport(true, "You restored the station long enough to launch."), recordBanner);
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
        string title = BuildFailureTitle(reason);
        string recordBanner = CommitRunRecords(false);
        GameAudio.Instance?.SetScannerActive(false);
        GameAudio.Instance?.PlayFailure();
        HUDController.Instance?.ShowEndScreen(title, BuildEndReport(false, BuildFailureSummary(reason)), recordBanner);
    }

    private static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetPaused(bool paused)
    {
        if (IsGameEnded || waitingForModeSelection)
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
        GameAudio.Instance?.PlayRobotAlert();
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

    private string ApplySystemRepairEffect(string systemName)
    {
        string lower = systemName.ToLowerInvariant();
        if (lower.Contains("corridor relay"))
        {
            return EndlessModeActive ? "Relay loop rerouted: score chain extended." : "Bulkhead relay online: first side rooms unlocked.";
        }

        if (lower.Contains("life support"))
        {
            lifeSupportOnline = true;
            PlayerOxygen oxygen = FindObjectOfType<PlayerOxygen>();
            if (oxygen != null)
            {
                oxygen.AddOxygen(18f);
            }

            return "Life Support online: oxygen drain reduced and O2 restored.";
        }

        if (lower.Contains("navigation"))
        {
            if (!EndlessModeActive)
            {
                remainingTime += 35f;
                return "Navigation route mapped: evacuation timer +35s.";
            }

            return "Navigation data banked: robot patrol AI adapted.";
        }

        if (lower.Contains("medical"))
        {
            PlayerHealth health = FindObjectOfType<PlayerHealth>();
            if (health != null)
            {
                health.Heal(28f);
            }

            return "Medical air mix online: suit integrity restored.";
        }

        if (lower.Contains("security"))
        {
            ApplySecurityOverride();
            return "Security override online: robots detect and hit weaker.";
        }

        if (lower.Contains("reactor"))
        {
            reactorOnline = true;
            radiationPulseRemaining = 0f;
            radiationPulseTimer = radiationPulseInterval * 1.4f;
            stationInstability = Mathf.Max(0f, stationInstability - 0.12f);
            return "Reactor stable: radiation surges temporarily halted.";
        }

        if (lower.Contains("comms"))
        {
            if (!EndlessModeActive)
            {
                remainingTime += 25f;
                return "Comms link online: evacuation signal boosted +25s.";
            }

            return "Comms packet scored: station threat matrix escalated.";
        }

        return "Station pressure stabilized.";
    }

    private int CalculateEndlessRepairScore(string systemName)
    {
        int score = endlessBaseScorePerRepair + Mathf.Max(0, endlessThreatLevel - 1) * 25;
        string lower = systemName.ToLowerInvariant();
        if (lower.Contains("reactor") || lower.Contains("security"))
        {
            score += 50;
        }
        else if (lower.Contains("life support") || lower.Contains("medical"))
        {
            score += 25;
        }

        return score;
    }

    private bool UpdateEndlessDifficulty()
    {
        if (!EndlessModeActive)
        {
            return false;
        }

        int previousLevel = endlessThreatLevel;
        endlessThreatLevel = Mathf.Max(1, 1 + Mathf.FloorToInt(endlessScore / Mathf.Max(1, endlessScorePerThreatLevel)));
        float threatMultiplier = 1f + (endlessThreatLevel - 1) * endlessThreatGrowthPerLevel;
        SecurityRobot[] robots = FindObjectsOfType<SecurityRobot>();
        foreach (SecurityRobot robot in robots)
        {
            robot.ApplyEndlessThreat(threatMultiplier);
        }

        stationInstability = Mathf.Clamp01(stationInstability + Mathf.Max(0, endlessThreatLevel - previousLevel) * 0.035f);
        return endlessThreatLevel > previousLevel;
    }

    private void UpdateEndlessHUD()
    {
        HUDController.Instance?.SetTimerText("SCORE " + endlessScore.ToString("0000"));
        HUDController.Instance?.SetRepairText("THREAT " + endlessThreatLevel + "  TASKS " + completedRepairs);
    }

    private void ApplySecurityOverride()
    {
        SecurityRobot[] robots = FindObjectsOfType<SecurityRobot>();
        foreach (SecurityRobot robot in robots)
        {
            robot.ApplySecurityOverride();
        }
    }

    private string BuildEndReport(bool success, string resultText)
    {
        PlayerHealth health = FindObjectOfType<PlayerHealth>();
        PlayerOxygen oxygen = FindObjectOfType<PlayerOxygen>();
        float healthValue = health == null ? 0f : health.CurrentHealth;
        float oxygenValue = oxygen == null ? 0f : oxygen.CurrentOxygen;

        if (EndlessModeActive)
        {
            return resultText
                + "\n\nFINAL SCORE     " + endlessScore
                + "\nTHREAT LEVEL    " + endlessThreatLevel
                + "\nTASKS COMPLETED " + completedRepairs
                + "\nSURVIVAL TIME   " + FormatTime(missionElapsedTime)
                + "\nHEALTH REMAINING " + Mathf.CeilToInt(healthValue) + "%"
                + "\nO2 REMAINING     " + Mathf.CeilToInt(oxygenValue) + "%"
                + "\nDETECTIONS       " + detectionCount
                + "\nRANK             " + CalculateEndlessRank()
                + "\nCAREER BEST      " + PlayerPrefs.GetInt(EndlessHighScoreKey, 0).ToString("0000");
        }

        string rank = success ? CalculateRank(healthValue, oxygenValue) : "F";

        return resultText
            + "\n\nMISSION TIME     " + FormatTime(missionElapsedTime)
            + "\nSYSTEMS RESTORED " + completedRepairs + "/" + requiredRepairs
            + "\nHEALTH REMAINING " + Mathf.CeilToInt(healthValue) + "%"
            + "\nO2 REMAINING     " + Mathf.CeilToInt(oxygenValue) + "%"
            + "\nDETECTIONS       " + detectionCount
            + "\nRANK             " + rank
            + "\nFASTEST ESCAPE   " + FormatRecordTime(PlayerPrefs.GetFloat(TimedFastestEscapeKey, -1f));
    }

    private string BuildFailureTitle(string reason)
    {
        string label = GetFailureLabel(reason);
        return EndlessModeActive ? "ENDLESS RUN COMPLETE // " + label : "MISSION FAILED // " + label;
    }

    private string BuildFailureSummary(string reason)
    {
        return "FAILURE REASON: " + GetFailureReadableReason(reason)
            + "\n" + GetFailureAdvice(reason);
    }

    private static string GetFailureLabel(string reason)
    {
        string normalized = NormalizeFailureReason(reason);
        if (normalized.Contains("oxygen"))
        {
            return "OXYGEN DEPLETED";
        }

        if (normalized.Contains("evacuation window") || normalized.Contains("time"))
        {
            return "TIME EXPIRED";
        }

        if (normalized.Contains("integrity") || normalized.Contains("health"))
        {
            return "SUIT INTEGRITY LOST";
        }

        return "MISSION LOST";
    }

    private static string GetFailureReadableReason(string reason)
    {
        string normalized = NormalizeFailureReason(reason);
        if (normalized.Contains("oxygen"))
        {
            return "Oxygen reached zero.";
        }

        if (normalized.Contains("evacuation window") || normalized.Contains("time"))
        {
            return "The evacuation timer reached zero.";
        }

        if (normalized.Contains("integrity") || normalized.Contains("health"))
        {
            return "Health reached zero after suit damage.";
        }

        return string.IsNullOrWhiteSpace(reason) ? "Unknown system failure." : reason;
    }

    private static string GetFailureAdvice(string reason)
    {
        string normalized = NormalizeFailureReason(reason);
        if (normalized.Contains("oxygen"))
        {
            return "NEXT ATTEMPT: collect O2 canisters, avoid radiation leaks, and repair Life Support earlier.";
        }

        if (normalized.Contains("evacuation window") || normalized.Contains("time"))
        {
            return "NEXT ATTEMPT: prioritize terminals, unlock bulkheads quickly, and use route shortcuts.";
        }

        if (normalized.Contains("integrity") || normalized.Contains("health"))
        {
            return "NEXT ATTEMPT: break line of sight from robots, crouch near patrols, and use medkits.";
        }

        return "NEXT ATTEMPT: review the final status readout and adjust your route.";
    }

    private static string NormalizeFailureReason(string reason)
    {
        return string.IsNullOrWhiteSpace(reason) ? string.Empty : reason.ToLowerInvariant();
    }

    private string CalculateEndlessRank()
    {
        if (endlessScore >= 2200)
        {
            return "S";
        }

        if (endlessScore >= 1500)
        {
            return "A";
        }

        if (endlessScore >= 900)
        {
            return "B";
        }

        if (endlessScore >= 450)
        {
            return "C";
        }

        return "D";
    }

    private string CalculateRank(float healthValue, float oxygenValue)
    {
        return RankForRating(CalculateTimedRating(healthValue, oxygenValue));
    }

    private int CalculateTimedRating(float healthValue, float oxygenValue)
    {
        int score = 100;
        score -= detectionCount * 10;
        score -= Mathf.FloorToInt(missionElapsedTime / 60f) * 2;
        score -= Mathf.FloorToInt((100f - healthValue) / 5f);
        score -= Mathf.FloorToInt((100f - oxygenValue) / 8f);
        score += Mathf.Max(0, completedRepairs - 5) * 3;
        return Mathf.Clamp(score, 0, 100);
    }

    private static string RankForRating(int score)
    {
        if (score >= 90)
        {
            return "S";
        }

        if (score >= 75)
        {
            return "A";
        }

        if (score >= 60)
        {
            return "B";
        }

        if (score >= 45)
        {
            return "C";
        }

        return "D";
    }

    private string CommitRunRecords(bool success)
    {
        if (EndlessModeActive)
        {
            int bestScore = PlayerPrefs.GetInt(EndlessHighScoreKey, 0);
            float longestRun = PlayerPrefs.GetFloat(EndlessLongestRunKey, 0f);
            bool isHighScore = endlessScore > bestScore;
            bool isLongestRun = missionElapsedTime > longestRun;

            if (isHighScore)
            {
                PlayerPrefs.SetInt(EndlessHighScoreKey, endlessScore);
            }

            if (isLongestRun)
            {
                PlayerPrefs.SetFloat(EndlessLongestRunKey, missionElapsedTime);
            }

            PlayerPrefs.Save();
            if (isHighScore)
            {
                return "NEW HIGH SCORE  //  " + endlessScore.ToString("0000");
            }

            if (isLongestRun)
            {
                return "NEW SURVIVAL RECORD  //  " + FormatTime(missionElapsedTime);
            }

            return "HIGH SCORE  //  " + bestScore.ToString("0000");
        }

        if (!success)
        {
            return "RECORD LOCKED  //  COMPLETE EXTRACTION TO QUALIFY";
        }

        PlayerHealth health = FindObjectOfType<PlayerHealth>();
        PlayerOxygen oxygen = FindObjectOfType<PlayerOxygen>();
        float healthValue = health == null ? 0f : health.CurrentHealth;
        float oxygenValue = oxygen == null ? 0f : oxygen.CurrentOxygen;
        int rating = CalculateTimedRating(healthValue, oxygenValue);
        int bestRating = PlayerPrefs.GetInt(TimedBestRatingKey, -1);
        float fastestEscape = PlayerPrefs.GetFloat(TimedFastestEscapeKey, -1f);
        bool isBestRating = rating > bestRating;
        bool isFastestEscape = fastestEscape < 0f || missionElapsedTime < fastestEscape;

        if (isBestRating)
        {
            PlayerPrefs.SetInt(TimedBestRatingKey, rating);
        }

        if (isFastestEscape)
        {
            PlayerPrefs.SetFloat(TimedFastestEscapeKey, missionElapsedTime);
        }

        PlayerPrefs.Save();
        if (isFastestEscape)
        {
            return "NEW EVACUATION RECORD  //  " + FormatTime(missionElapsedTime);
        }

        if (isBestRating)
        {
            return "NEW PERFORMANCE RECORD  //  RANK " + RankForRating(rating);
        }

        return "FASTEST ESCAPE  //  " + FormatRecordTime(fastestEscape);
    }

    private string BuildRecordSummary()
    {
        int bestRating = PlayerPrefs.GetInt(TimedBestRatingKey, -1);
        int highScore = PlayerPrefs.GetInt(EndlessHighScoreKey, 0);
        float fastestEscape = PlayerPrefs.GetFloat(TimedFastestEscapeKey, -1f);
        float longestRun = PlayerPrefs.GetFloat(EndlessLongestRunKey, 0f);
        string rank = bestRating < 0 ? "--" : RankForRating(bestRating);

        return "TIMED   FASTEST " + FormatRecordTime(fastestEscape) + "   BEST RANK " + rank
            + "\nENDLESS HIGH SCORE " + highScore.ToString("0000") + "   LONGEST " + FormatRecordTime(longestRun);
    }

    private static string FormatRecordTime(float seconds)
    {
        return seconds <= 0f ? "--:--" : FormatTime(seconds);
    }

    private static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return minutes.ToString("00") + ":" + secs.ToString("00");
    }

    private void UpdateObjectiveText()
    {
        if (HUDController.Instance == null)
        {
            return;
        }

        if (waitingForModeSelection)
        {
            HUDController.Instance.SetObjective("SELECT MODE\n1 Timed Evacuation\n2 Endless Survival");
            return;
        }

        if (EndlessModeActive)
        {
            HUDController.Instance.SetObjective("ENDLESS SURVIVAL\nRepair terminals for score.\nHigher score raises robot threat.");
            return;
        }

        if (AllRepairsComplete)
        {
            HUDController.Instance.SetObjective("NEXT: Reach the escape pod\nAll systems online. Avoid robots.");
            return;
        }

        string objective;
        switch (completedRepairs)
        {
            case 0:
                objective = "NEXT: Open pod bay door\nRepair Corridor Relay terminal.";
                break;
            case 1:
                objective = "NEXT: Restore side systems\nLife Support + Navigation unlocked.";
                break;
            case 2:
                objective = "NEXT: Enter Medical Bay\nFind Reactor Fuse while repairing systems.";
                break;
            case 3:
                objective = "NEXT: Open Security Office\nUse Security Keycard if required.";
                break;
            case 4:
                objective = "NEXT: Restore Reactor\nUse Reactor Fuse to clear the lock.";
                break;
            case 5:
                objective = "NEXT: Restore Comms Lab\nUse Comms Decoder if required.";
                break;
            default:
                objective = "NEXT: Restore remaining system\nThen return to escape pod.";
                break;
        }

        HUDController.Instance.SetObjective(objective);
    }
}
