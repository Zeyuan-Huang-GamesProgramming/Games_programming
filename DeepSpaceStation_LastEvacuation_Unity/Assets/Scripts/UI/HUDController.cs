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
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TMP_Text mainMenuInfoText;
    [SerializeField] private GameObject mainMenuPopupPanel;
    [SerializeField] private TMP_Text mainMenuPopupTitleText;
    [SerializeField] private TMP_Text mainMenuPopupBodyText;
    [SerializeField] private GameObject mainMenuOptionsControls;
    [SerializeField] private TMP_Text mainMenuVolumeValueText;
    [SerializeField] private TMP_Text mainMenuMouseSensitivityValueText;
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

    private static readonly Vector2 MainMenuPopupPosition = new Vector2(-54f, -72f);
    private static readonly Vector2 MainMenuPopupSize = new Vector2(382f, 302f);
    private static readonly Vector2 MainMenuPopupBodySize = new Vector2(336f, 178f);
    private static readonly Vector2 MainMenuOptionsBodySize = new Vector2(336f, 96f);
    private static readonly Vector2 EndCardSize = new Vector2(780f, 560f);

    public bool HasMainMenu => mainMenuPanel != null;
    public bool IsMainMenuPopupOpen => mainMenuPopupPanel != null && mainMenuPopupPanel.activeSelf;

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
        ApplyMainMenuPopupLayout(false);
        CloseMainMenuPopup();
        RefreshSettingsText();
        SetPauseOpen(false);
    }

    private void Update()
    {
        if ((mainMenuPanel != null && mainMenuPanel.activeSelf)
            || (modeSelectPanel != null && modeSelectPanel.activeSelf))
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

        if (mainMenuVolumeValueText != null)
        {
            mainMenuVolumeValueText.text = Mathf.RoundToInt(GameSettings.MasterVolume * 100f).ToString("00") + "%";
        }

        if (mouseSensitivityValueText != null)
        {
            mouseSensitivityValueText.text = GameSettings.GetMouseSensitivity(2.2f).ToString("0.0");
        }

        if (mainMenuMouseSensitivityValueText != null)
        {
            mainMenuMouseSensitivityValueText.text = GameSettings.GetMouseSensitivity(2.2f).ToString("0.0");
        }
    }

    public void SetModeSelectionOpen(bool open)
    {
        if (modeSelectPanel != null)
        {
            modeSelectPanel.SetActive(open);
        }
    }

    public void SetMainMenuOpen(bool open)
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(open);
            if (open)
            {
                ApplyMainMenuPopupLayout(false);
            }
        }
    }

    public void ShowMainMenuWelcome()
    {
        CloseMainMenuPopup();
        SetMainMenuInfo(
            "MISSION STATUS\n"
            + "Station systems are offline.\n"
            + "Oxygen reserve is unstable.\n"
            + "Security robots are active.\n\n"
            + "Select NEW GAME to begin evacuation protocol.");
    }

    public void ShowMainMenuOptions()
    {
        ShowMainMenuPopup(
            "OPTIONS",
            "Adjust mission settings before deployment.\nSettings are saved automatically.",
            true);
    }

    public void ShowMainMenuHowToPlay()
    {
        ShowMainMenuPopup(
            "HOW TO PLAY",
            "WASD     Move\nMouse    Look around\nE        Interact\nTAB / I  Backpack\nQ        Scanner\nH        Use medkit\nB        Use battery\n\nRepair critical terminals, manage oxygen and suit health, avoid security robots, then reach the escape pod.",
            false);
    }

    public void ShowMainMenuCredits()
    {
        ShowMainMenuPopup(
            "CREDITS",
            "Deep Space Station: Last Evacuation\nCreated by Zeyuan Huang\nBuilt with Unity 2022.3 LTS\n\nExternal free assets are documented in the GitHub repository credits and asset-use notes.",
            false);
    }

    public void CloseMainMenuPopup()
    {
        SetMainMenuPopupOpen(false);
    }

    private void SetMainMenuInfo(string value)
    {
        if (mainMenuInfoText != null)
        {
            mainMenuInfoText.text = value;
        }
    }

    private void ShowMainMenuPopup(string title, string body, bool showOptionsControls)
    {
        if (mainMenuPopupTitleText != null)
        {
            mainMenuPopupTitleText.text = title;
        }

        if (mainMenuPopupBodyText != null)
        {
            mainMenuPopupBodyText.text = body;
        }

        if (mainMenuOptionsControls != null)
        {
            mainMenuOptionsControls.SetActive(showOptionsControls);
        }

        ApplyMainMenuPopupLayout(showOptionsControls);
        RefreshSettingsText();
        SetMainMenuPopupOpen(true);
    }

    private void SetMainMenuPopupOpen(bool open)
    {
        if (mainMenuPopupPanel != null)
        {
            mainMenuPopupPanel.SetActive(open);
        }
    }

    private void ApplyMainMenuPopupLayout(bool showOptionsControls)
    {
        if (mainMenuPopupPanel == null)
        {
            return;
        }

        RectTransform popupRect = mainMenuPopupPanel.GetComponent<RectTransform>();
        if (popupRect != null)
        {
            popupRect.anchorMin = new Vector2(1f, 1f);
            popupRect.anchorMax = new Vector2(1f, 1f);
            popupRect.pivot = new Vector2(1f, 1f);
            popupRect.anchoredPosition = MainMenuPopupPosition;
            popupRect.sizeDelta = MainMenuPopupSize;
        }

        SetTopLeftRect(mainMenuPopupTitleText == null ? null : mainMenuPopupTitleText.rectTransform, new Vector2(22f, -20f), showOptionsControls ? new Vector2(330f, 42f) : new Vector2(244f, 42f));
        SetTopLeftRect(mainMenuPopupBodyText == null ? null : mainMenuPopupBodyText.rectTransform, new Vector2(22f, -70f), showOptionsControls ? MainMenuOptionsBodySize : MainMenuPopupBodySize);

        RectTransform optionsRect = mainMenuOptionsControls == null ? null : mainMenuOptionsControls.GetComponent<RectTransform>();
        SetTopLeftRect(optionsRect, new Vector2(20f, -164f), new Vector2(342f, 84f));
        SetTopLeftRect(FindMainMenuPopupRect("MainMenuVolumeDownButton"), new Vector2(246f, -6f), new Vector2(34f, 28f));
        SetTopLeftRect(FindMainMenuPopupRect("MainMenuVolumeUpButton"), new Vector2(292f, -6f), new Vector2(34f, 28f));
        SetTopLeftRect(FindMainMenuPopupRect("MainMenuSensitivityDownButton"), new Vector2(246f, -45f), new Vector2(34f, 28f));
        SetTopLeftRect(FindMainMenuPopupRect("MainMenuSensitivityUpButton"), new Vector2(292f, -45f), new Vector2(34f, 28f));
        Vector2 backPosition = showOptionsControls ? new Vector2(246f, -254f) : new Vector2(274f, -18f);
        Vector2 backSize = showOptionsControls ? new Vector2(98f, 34f) : new Vector2(86f, 30f);
        SetTopLeftRect(FindMainMenuPopupRect("MainMenuBackButton"), backPosition, backSize);
        SetCenterRect(FindMainMenuPopupRect("MainMenuBackButtonLabel"), Vector2.zero, backSize);
    }

    private RectTransform FindMainMenuPopupRect(string objectName)
    {
        if (mainMenuPopupPanel == null)
        {
            return null;
        }

        Transform[] children = mainMenuPopupPanel.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == objectName)
            {
                return child.GetComponent<RectTransform>();
            }
        }

        return null;
    }

    private static void SetTopLeftRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void SetCenterRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
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
        SetBackpackOpen(false);
        SetScan(false, "");
        SetPauseOpen(false);
        ApplyEndScreenLayout();

        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        if (endTitleText != null)
        {
            endTitleText.text = FormatEndTitle(title);
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

    private void ApplyEndScreenLayout()
    {
        if (endPanel == null)
        {
            return;
        }

        endPanel.transform.SetAsLastSibling();

        RectTransform panelRect = endPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        Image overlay = endPanel.GetComponent<Image>();
        if (overlay != null)
        {
            overlay.color = new Color(0.004f, 0.012f, 0.028f, 0.96f);
        }

        RectTransform cardRect = EnsureEndCard();
        if (cardRect != null)
        {
            SetCenterRect(cardRect, Vector2.zero, EndCardSize);
            cardRect.SetAsFirstSibling();
            MoveToEndCard(endTitleText, cardRect);
            MoveToEndCard(endBodyText, cardRect);
            MoveToEndCard(endRecordText, cardRect);
            MoveToEndCard(endHintText, cardRect);
        }

        SetCenterRect(endTitleText == null ? null : endTitleText.rectTransform, new Vector2(0f, 202f), new Vector2(700f, 96f));
        SetCenterRect(endBodyText == null ? null : endBodyText.rectTransform, new Vector2(0f, 8f), new Vector2(680f, 300f));
        SetCenterRect(endRecordText == null ? null : endRecordText.rectTransform, new Vector2(0f, -204f), new Vector2(680f, 38f));
        SetCenterRect(endHintText == null ? null : endHintText.rectTransform, new Vector2(0f, -246f), new Vector2(560f, 30f));

        if (endTitleText != null)
        {
            endTitleText.alignment = TextAlignmentOptions.Center;
            endTitleText.fontSize = 31f;
            endTitleText.fontStyle = FontStyles.Bold;
            endTitleText.characterSpacing = 3f;
            endTitleText.lineSpacing = -10f;
            endTitleText.color = new Color(0.08f, 0.82f, 1f);
            endTitleText.enableWordWrapping = true;
        }

        if (endBodyText != null)
        {
            endBodyText.alignment = TextAlignmentOptions.TopLeft;
            endBodyText.fontSize = 17f;
            endBodyText.fontStyle = FontStyles.Normal;
            endBodyText.characterSpacing = 1.5f;
            endBodyText.lineSpacing = -6f;
            endBodyText.color = new Color(0.78f, 0.92f, 1f);
            endBodyText.enableWordWrapping = true;
            endBodyText.overflowMode = TextOverflowModes.Overflow;
        }

        if (endRecordText != null)
        {
            endRecordText.alignment = TextAlignmentOptions.Center;
            endRecordText.fontSize = 19f;
            endRecordText.fontStyle = FontStyles.Bold;
            endRecordText.characterSpacing = 2.4f;
            endRecordText.color = new Color(1f, 0.69f, 0.2f);
        }

        if (endHintText != null)
        {
            endHintText.alignment = TextAlignmentOptions.Center;
            endHintText.fontSize = 17f;
            endHintText.fontStyle = FontStyles.Bold;
            endHintText.characterSpacing = 2f;
            endHintText.color = new Color(0.42f, 0.68f, 0.8f);
        }
    }

    private RectTransform EnsureEndCard()
    {
        Transform card = endPanel.transform.Find("EndCard");
        if (card == null)
        {
            GameObject cardObject = new GameObject("EndCard", typeof(RectTransform), typeof(Image));
            cardObject.transform.SetParent(endPanel.transform, false);
            card = cardObject.transform;
        }

        Image cardImage = card.GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.color = new Color(0.008f, 0.027f, 0.055f, 0.92f);
            cardImage.raycastTarget = false;
        }

        return card.GetComponent<RectTransform>();
    }

    private static void MoveToEndCard(TMP_Text text, RectTransform cardRect)
    {
        if (text == null || cardRect == null || text.transform.parent == cardRect)
        {
            return;
        }

        text.transform.SetParent(cardRect, false);
    }

    private static string FormatEndTitle(string title)
    {
        return string.IsNullOrWhiteSpace(title) ? string.Empty : title.Replace(" // ", "\n");
    }
}
