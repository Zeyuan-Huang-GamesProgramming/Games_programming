using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [SerializeField] private TMP_Text oxygenText;
    [SerializeField] private Image oxygenFill;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text batteryText;
    [SerializeField] private Image batteryFill;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text repairText;
    [SerializeField] private TMP_Text pressureText;
    [SerializeField] private TMP_Text inventoryText;
    [SerializeField] private GameObject backpackPanel;
    [SerializeField] private TMP_Text backpackItemsText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject scanPanel;
    [SerializeField] private TMP_Text scanText;
    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private TMP_Text recordsText;
    [SerializeField] private GameObject briefingPanel;
    [SerializeField] private TMP_Text briefingTitleText;
    [SerializeField] private TMP_Text briefingBodyText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text volumeValueText;
    [SerializeField] private TMP_Text mouseSensitivityValueText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitleText;
    [SerializeField] private TMP_Text endBodyText;
    [SerializeField] private TMP_Text endRecordText;
    [SerializeField] private TMP_Text endHintText;

    private float messageTimer;
    private float briefingTimer;
    private bool backpackOpen;
    private bool oxygenCriticalActive;
    private bool healthCriticalActive;
    private bool timeCriticalActive;

    private const float OxygenCriticalThreshold = 0.1f;
    private const float OxygenWarningThreshold = 0.25f;
    private const float HealthCriticalThreshold = 0.3f;
    private const float TimerWarningSeconds = 60f;
    private const float WarningFlashSpeed = 8f;

    private readonly Color oxygenNormalColor = new Color(0.05f, 0.95f, 0.8f);
    private readonly Color oxygenWarningColor = new Color(1f, 0.68f, 0.08f);
    private readonly Color criticalColor = new Color(1f, 0.16f, 0.08f);
    private readonly Color healthNormalColor = new Color(0.08f, 0.5f, 1f);
    private readonly Color timerNormalColor = new Color(0.6f, 0.95f, 1f);
    private readonly Color timerWarningColor = new Color(1f, 0.22f, 0.12f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetPrompt("");
        ShowMessage("", 0f);

        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        SetBackpackOpen(false);
        SetScan(false, "");
        SetBriefingOpen(false);
        RefreshSettingsText();
        SetPauseOpen(false);
    }

    private void Update()
    {
        if (modeSelectPanel != null && modeSelectPanel.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            SetBackpackOpen(!backpackOpen);
        }

        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f && messageText != null)
            {
                messageTimer = 0f;
                UpdatePersistentWarning();
            }
        }

        if (briefingTimer > 0f)
        {
            briefingTimer -= Time.unscaledDeltaTime;
            if (briefingTimer <= 0f)
            {
                SetBriefingOpen(false);
            }
        }
    }

    public void SetOxygen(float normalized, float value)
    {
        float clamped = Mathf.Clamp01(normalized);
        int roundedValue = Mathf.CeilToInt(value);
        oxygenCriticalActive = clamped < OxygenCriticalThreshold;
        bool oxygenWarningActive = clamped < OxygenWarningThreshold;
        Color oxygenColor = oxygenNormalColor;

        if (oxygenCriticalActive)
        {
            oxygenColor = GetFlashingColor(criticalColor, Color.white, 0.9f);
        }
        else if (oxygenWarningActive)
        {
            oxygenColor = GetFlashingColor(oxygenWarningColor, criticalColor, 0.6f);
        }

        if (oxygenText != null)
        {
            if (oxygenCriticalActive)
            {
                oxygenText.text = "O2 CRITICAL " + roundedValue + "%";
            }
            else if (oxygenWarningActive)
            {
                oxygenText.text = "O2 WARNING " + roundedValue + "%";
            }
            else
            {
                oxygenText.text = "O2 " + roundedValue + "%";
            }

            oxygenText.color = oxygenColor;
        }

        if (oxygenFill != null)
        {
            oxygenFill.fillAmount = clamped;
            oxygenFill.color = oxygenColor;
        }

        UpdatePersistentWarning();
    }

    public void SetHealth(float normalized, float value)
    {
        float clamped = Mathf.Clamp01(normalized);
        int roundedValue = Mathf.CeilToInt(value);
        healthCriticalActive = clamped < HealthCriticalThreshold;
        Color healthColor = healthCriticalActive ? GetFlashingColor(criticalColor, Color.white, 0.75f) : healthNormalColor;

        if (healthText != null)
        {
            healthText.text = healthCriticalActive ? "SUIT CRITICAL " + roundedValue + "/100" : "HEALTH " + roundedValue + "/100";
            healthText.color = healthColor;
        }

        if (healthFill != null)
        {
            healthFill.fillAmount = clamped;
            healthFill.color = healthColor;
        }

        UpdatePersistentWarning();
    }

    public void SetBattery(float normalized, float value)
    {
        if (batteryText != null)
        {
            batteryText.text = "BATTERY " + Mathf.CeilToInt(value) + "%";
        }

        if (batteryFill != null)
        {
            batteryFill.fillAmount = Mathf.Clamp01(normalized);
            batteryFill.color = normalized < 0.25f ? new Color(1f, 0.65f, 0.05f) : new Color(0.18f, 0.65f, 1f);
        }
    }

    public void SetTimer(float seconds)
    {
        if (timerText == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        timeCriticalActive = seconds < TimerWarningSeconds;
        timerText.text = (timeCriticalActive ? "TIME CRITICAL " : "TIME ") + minutes.ToString("00") + ":" + secs.ToString("00");
        timerText.color = timeCriticalActive ? GetFlashingColor(timerWarningColor, Color.white, 0.65f) : timerNormalColor;
        UpdatePersistentWarning();
    }

    public void SetTimerText(string value)
    {
        if (timerText != null)
        {
            timeCriticalActive = false;
            timerText.text = value;
            timerText.color = timerNormalColor;
            UpdatePersistentWarning();
        }
    }

    public void SetRepairs(int completed, int required)
    {
        if (repairText != null)
        {
            repairText.text = "SYSTEMS " + completed + "/" + required;
        }
    }

    public void SetRepairText(string value)
    {
        if (repairText != null)
        {
            repairText.text = value;
        }
    }

    public void SetPressure(float multiplier, float instability, bool pulseActive)
    {
        if (pressureText == null)
        {
            return;
        }

        string pulse = pulseActive ? " SURGE" : "";
        pressureText.text = "O2 DRAIN x" + multiplier.ToString("0.0") + pulse;
        pressureText.color = pulseActive || instability > 0.65f ? new Color(1f, 0.24f, 0.12f) : new Color(1f, 0.86f, 0.25f);
    }

    public void SetInventory(string inventory)
    {
        if (inventoryText != null)
        {
            inventoryText.text = "BAG [Tab]";
        }

        if (backpackItemsText != null)
        {
            backpackItemsText.text = string.IsNullOrEmpty(inventory) ? "No items" : inventory;
        }
    }

    public void SetBackpackOpen(bool open)
    {
        backpackOpen = open;
        if (backpackPanel != null)
        {
            backpackPanel.SetActive(backpackOpen);
        }
    }

    public void SetScan(bool open, string text)
    {
        if (scanPanel != null)
        {
            scanPanel.SetActive(open);
        }

        if (scanText != null)
        {
            scanText.text = text;
        }
    }

    public void SetPauseOpen(bool open)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(open);
        }

        if (open)
        {
            RefreshSettingsText();
        }
    }

    public void DecreaseMasterVolume()
    {
        GameSettings.SetMasterVolume(GameSettings.MasterVolume - 0.1f);
        GameAudio.Instance?.PlayCalibrationStep();
        RefreshSettingsText();
    }

    public void IncreaseMasterVolume()
    {
        GameSettings.SetMasterVolume(GameSettings.MasterVolume + 0.1f);
        GameAudio.Instance?.PlayCalibrationStep();
        RefreshSettingsText();
    }

    public void DecreaseMouseSensitivity()
    {
        float current = GameSettings.GetMouseSensitivity(2.2f);
        GameSettings.SetMouseSensitivity(current - 0.2f);
        GameAudio.Instance?.PlayCalibrationStep();
        RefreshSettingsText();
    }

    public void IncreaseMouseSensitivity()
    {
        float current = GameSettings.GetMouseSensitivity(2.2f);
        GameSettings.SetMouseSensitivity(current + 0.2f);
        GameAudio.Instance?.PlayCalibrationStep();
        RefreshSettingsText();
    }

    private void RefreshSettingsText()
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = Mathf.RoundToInt(GameSettings.MasterVolume * 100f).ToString("00") + "%";
        }

        if (mouseSensitivityValueText != null)
        {
            mouseSensitivityValueText.text = GameSettings.GetMouseSensitivity(2.2f).ToString("0.0");
        }
    }

    public void SetModeSelectionOpen(bool open)
    {
        if (modeSelectPanel != null)
        {
            modeSelectPanel.SetActive(open);
        }
    }

    public void SetRecordsSummary(string summary)
    {
        if (recordsText != null)
        {
            recordsText.text = summary;
        }
    }

    public void ShowBriefing(string title, string body, float seconds)
    {
        if (briefingTitleText != null)
        {
            briefingTitleText.text = title;
        }

        if (briefingBodyText != null)
        {
            briefingBodyText.text = body;
        }

        briefingTimer = seconds;
        SetBriefingOpen(true);
    }

    private void SetBriefingOpen(bool open)
    {
        if (briefingPanel != null)
        {
            briefingPanel.SetActive(open);
        }
    }

    public void SetObjective(string objective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }
    }

    public void SetPrompt(string prompt)
    {
        if (promptText != null)
        {
            promptText.text = prompt;
        }
    }

    public void ShowMessage(string message, float seconds)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }

        messageTimer = seconds;
        if (messageTimer <= 0f)
        {
            UpdatePersistentWarning();
        }
    }

    private Color GetFlashingColor(Color firstColor, Color secondColor, float speedMultiplier)
    {
        float pulse = Mathf.PingPong(Time.unscaledTime * WarningFlashSpeed * speedMultiplier, 1f);
        return Color.Lerp(firstColor, secondColor, pulse);
    }

    private void UpdatePersistentWarning()
    {
        if (messageText == null || messageTimer > 0f)
        {
            return;
        }

        if (oxygenCriticalActive)
        {
            messageText.text = "OXYGEN CRITICAL - find O2 or repair Life Support";
        }
        else if (healthCriticalActive)
        {
            messageText.text = "SUIT CRITICAL - avoid robots and use medkit";
        }
        else if (timeCriticalActive)
        {
            messageText.text = "EVACUATION WINDOW CLOSING";
        }
        else
        {
            messageText.text = "";
        }
    }

    public void ShowEndScreen(string title, string body, string recordBanner)
    {
        SetBriefingOpen(false);

        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        if (endTitleText != null)
        {
            endTitleText.text = title;
        }

        if (endBodyText != null)
        {
            endBodyText.text = body;
        }

        if (endRecordText != null)
        {
            endRecordText.text = recordBanner;
        }

        if (endHintText != null)
        {
            endHintText.text = "R  REDEPLOY";
        }
    }
}
