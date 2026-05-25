using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [SerializeField] private Text oxygenText;
    [SerializeField] private Image oxygenFill;
    [SerializeField] private Text healthText;
    [SerializeField] private Image healthFill;
    [SerializeField] private Text batteryText;
    [SerializeField] private Image batteryFill;
    [SerializeField] private Text timerText;
    [SerializeField] private Text repairText;
    [SerializeField] private Text pressureText;
    [SerializeField] private Text inventoryText;
    [SerializeField] private GameObject backpackPanel;
    [SerializeField] private Text backpackItemsText;
    [SerializeField] private Text objectiveText;
    [SerializeField] private Text promptText;
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject scanPanel;
    [SerializeField] private Text scanText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text endTitleText;
    [SerializeField] private Text endBodyText;

    private float messageTimer;
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
        SetPauseOpen(false);
    }

    private void Update()
    {
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

    public void SetRepairs(int completed, int required)
    {
        if (repairText != null)
        {
            repairText.text = "SYSTEMS " + completed + "/" + required;
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

    public void ShowEndScreen(string title, string body)
    {
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
            endBodyText.text = body + "\n\nPress R to restart.";
        }
    }
}
