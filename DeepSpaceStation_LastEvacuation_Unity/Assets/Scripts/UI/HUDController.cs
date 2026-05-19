using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [SerializeField] private Text oxygenText;
    [SerializeField] private Image oxygenFill;
    [SerializeField] private Text timerText;
    [SerializeField] private Text repairText;
    [SerializeField] private Text objectiveText;
    [SerializeField] private Text promptText;
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text endTitleText;
    [SerializeField] private Text endBodyText;

    private float messageTimer;

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
    }

    private void Update()
    {
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
