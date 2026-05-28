using UnityEngine;

public class TerminalTask : MonoBehaviour, IInteractable
{
    [SerializeField] private string systemName = "Station system";
    [SerializeField] private Light statusLight;
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Color offlineColor = new Color(1f, 0.18f, 0.08f);
    [SerializeField] private Color activeColor = new Color(1f, 0.75f, 0.08f);
    [SerializeField] private Color onlineColor = new Color(0.08f, 1f, 0.65f);
    [SerializeField] private float repairSeconds = 4f;
    [SerializeField] private float abandonDistance = 4.8f;
    [SerializeField] private float endlessResetSeconds = 8f;
    [SerializeField] private KeyCode[] calibrationSequence = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

    private bool repaired;
    private bool active;
    private int sequenceIndex;
    private float repairProgress;
    private float repairedTimer;
    private GameObjectInteractor activeInteractor;

    private void Start()
    {
        ApplyState();
    }

    private void Update()
    {
        if (repaired)
        {
            HandleEndlessReset();
            return;
        }

        if (!active)
        {
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused)
        {
            return;
        }

        if (activeInteractor == null || Vector3.Distance(transform.position, activeInteractor.transform.position) > abandonDistance)
        {
            AbortRepair("Repair link lost");
            return;
        }

        HUDController.Instance?.SetPrompt(GetPrompt());

        if (sequenceIndex < calibrationSequence.Length)
        {
            HandleCalibrationInput();
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            repairProgress += Time.deltaTime;
        }
        else
        {
            repairProgress = Mathf.Max(0f, repairProgress - Time.deltaTime * 0.8f);
        }

        if (repairProgress >= repairSeconds)
        {
            CompleteRepair();
        }
    }

    public string GetPrompt()
    {
        if (repaired)
        {
            if (GameManager.Instance != null && GameManager.Instance.EndlessModeActive)
            {
                int seconds = Mathf.CeilToInt(Mathf.Max(0f, endlessResetSeconds - repairedTimer));
                return systemName + " rebooting " + seconds + "s";
            }

            return systemName + " online";
        }

        if (!active)
        {
            return "Press E to start " + systemName + " repair";
        }

        if (sequenceIndex < calibrationSequence.Length)
        {
            return "CALIBRATE " + systemName + ": press " + FormatKey(calibrationSequence[sequenceIndex]) + "  [" + (sequenceIndex + 1) + "/" + calibrationSequence.Length + "]";
        }

        int percent = Mathf.RoundToInt(Mathf.Clamp01(repairProgress / repairSeconds) * 100f);
        return "Hold E to stabilize " + systemName + "  " + percent + "%";
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (repaired || GameManager.Instance == null)
        {
            return;
        }

        if (!active)
        {
            active = true;
            activeInteractor = interactor;
            sequenceIndex = 0;
            repairProgress = 0f;
            ApplyState();
            GameAudio.Instance?.PlayTerminalStart();
            HUDController.Instance?.ShowMessage(systemName + " diagnostic started. Follow the number sequence.", 3f);
            return;
        }
    }

    public void ConfigureTask(float seconds, params KeyCode[] sequence)
    {
        repairSeconds = Mathf.Max(1f, seconds);
        if (sequence != null && sequence.Length > 0)
        {
            calibrationSequence = sequence;
        }
    }

    private void HandleCalibrationInput()
    {
        KeyCode pressed = GetPressedCalibrationKey();
        if (pressed == KeyCode.None)
        {
            return;
        }

        if (pressed == calibrationSequence[sequenceIndex])
        {
            sequenceIndex++;
            GameAudio.Instance?.PlayCalibrationStep();
            if (sequenceIndex >= calibrationSequence.Length)
            {
                HUDController.Instance?.ShowMessage(systemName + " calibrated. Hold E to route power.", 2.4f);
            }
            return;
        }

        sequenceIndex = 0;
        repairProgress = 0f;
        GameAudio.Instance?.PlayCalibrationError();
        GameManager.Instance?.AddInstability(0.08f, "Wrong input caused a pressure spike");
        ApplyState();
    }

    private void CompleteRepair()
    {
        repaired = true;
        active = false;
        activeInteractor = null;
        repairedTimer = 0f;
        ApplyState();
        GameManager.Instance.RegisterRepair(systemName);
    }

    private void HandleEndlessReset()
    {
        if (GameManager.Instance == null || !GameManager.Instance.EndlessModeActive || GameManager.Instance.IsGameEnded)
        {
            return;
        }

        repairedTimer += Time.deltaTime;
        if (repairedTimer < endlessResetSeconds)
        {
            return;
        }

        repaired = false;
        active = false;
        activeInteractor = null;
        sequenceIndex = 0;
        repairProgress = 0f;
        repairedTimer = 0f;
        ApplyState();
    }

    private void AbortRepair(string reason)
    {
        active = false;
        activeInteractor = null;
        sequenceIndex = 0;
        repairProgress = 0f;
        ApplyState();
        GameAudio.Instance?.PlayDenied();
        HUDController.Instance?.ShowMessage(reason, 2f);
    }

    private void ApplyState()
    {
        Color color = repaired ? onlineColor : active ? activeColor : offlineColor;

        if (statusLight != null)
        {
            statusLight.color = color;
            statusLight.intensity = repaired ? 3.5f : active ? 3f : 2f;
        }

        if (screenRenderer != null)
        {
            screenRenderer.material.color = color;
            screenRenderer.material.SetColor("_EmissionColor", color);
        }
    }

    private static KeyCode GetPressedCalibrationKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            return KeyCode.Alpha1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            return KeyCode.Alpha2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            return KeyCode.Alpha3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            return KeyCode.Alpha4;
        }

        return KeyCode.None;
    }

    private static string FormatKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Alpha1:
                return "1";
            case KeyCode.Alpha2:
                return "2";
            case KeyCode.Alpha3:
                return "3";
            case KeyCode.Alpha4:
                return "4";
            default:
                return key.ToString();
        }
    }
}
