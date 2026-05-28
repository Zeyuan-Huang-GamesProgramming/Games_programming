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
                messageText.text = "";
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
        if (oxygenText != null)
        {
            oxygenText.text = "O2 " + Mathf.CeilToInt(value) + "%";
        }

        if (oxygenFill != null)
        {
            oxygenFill.fillAmount = Mathf.Clamp01(normalized);
            oxygenFill.color = normalized < 0.25f ? new Color(1f, 0.2f, 0.12f) : new Color(0.05f, 0.95f, 0.8f);
        }
    }

    public void SetHealth(float normalized, float value)
    {
        if (healthText != null)
        {
            healthText.text = "HEALTH " + Mathf.CeilToInt(value) + "/100";
        }

        if (healthFill != null)
        {
            healthFill.fillAmount = Mathf.Clamp01(normalized);
            healthFill.color = normalized < 0.3f ? new Color(1f, 0.22f, 0.15f) : new Color(0.08f, 0.5f, 1f);
        }
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
        timerText.text = "TIME " + minutes.ToString("00") + ":" + secs.ToString("00");
    }

    public void SetTimerText(string value)
    {
        if (timerText != null)
        {
            timerText.text = value;
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
