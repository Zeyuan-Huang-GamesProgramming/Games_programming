using System.Collections.Generic;
using UnityEngine;

public class PlayerScanner : MonoBehaviour
{
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float scanRange = 14f;
    [SerializeField] private float batteryDrainPerSecond = 11f;

    private float currentBattery;
    private bool scannerActive;

    public float CurrentBattery => currentBattery;
    public float NormalizedBattery => maxBattery <= 0f ? 0f : currentBattery / maxBattery;

    private void Start()
    {
        currentBattery = maxBattery;
        HUDController.Instance?.SetBattery(NormalizedBattery, currentBattery);
        HUDController.Instance?.SetScan(false, "");
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused)
        {
            SetScannerAudio(false);
            HUDController.Instance?.SetScan(false, "");
            return;
        }

        bool scanning = Input.GetKey(KeyCode.Q) && currentBattery > 0f;
        if (!scanning)
        {
            SetScannerAudio(false);
            HUDController.Instance?.SetScan(false, "");
            return;
        }

        SetScannerAudio(true);
        currentBattery = Mathf.Max(0f, currentBattery - batteryDrainPerSecond * Time.deltaTime);
        HUDController.Instance?.SetBattery(NormalizedBattery, currentBattery);
        HUDController.Instance?.SetScan(true, BuildScanText());
    }

    public void Recharge(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
        HUDController.Instance?.SetBattery(NormalizedBattery, currentBattery);
    }

    private void SetScannerAudio(bool active)
    {
        if (scannerActive == active)
        {
            return;
        }

        scannerActive = active;
        GameAudio.Instance?.SetScannerActive(active);
    }

    private string BuildScanText()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, scanRange, ~0, QueryTriggerInteraction.Collide);
        List<string> signals = new List<string>();

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.transform == transform)
            {
                continue;
            }

            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            SecurityRobot robot = hit.GetComponentInParent<SecurityRobot>();
            if (interactable == null && robot == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            string label = robot != null ? "Security Robot" : hit.GetComponentInParent<Transform>().name;
            string line = label + "  " + distance.ToString("0.0") + "m";

            if (!signals.Contains(line))
            {
                signals.Add(line);
            }

            if (signals.Count >= 6)
            {
                break;
            }
        }

        return signals.Count == 0 ? "No nearby signals" : string.Join("\n", signals.ToArray());
    }
}
