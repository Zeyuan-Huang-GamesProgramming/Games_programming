using UnityEngine;

public class TerminalTask : MonoBehaviour, IInteractable
{
    [SerializeField] private string systemName = "Station system";
    [SerializeField] private Light statusLight;
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Color offlineColor = new Color(1f, 0.18f, 0.08f);
    [SerializeField] private Color onlineColor = new Color(0.08f, 1f, 0.65f);

    private bool repaired;

    private void Start()
    {
        ApplyState();
    }

    public string GetPrompt()
    {
        return repaired ? systemName + " online" : "Press E to repair " + systemName;
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (repaired || GameManager.Instance == null)
        {
            return;
        }

        repaired = true;
        ApplyState();
        GameManager.Instance.RegisterRepair(systemName);
    }

    private void ApplyState()
    {
        Color color = repaired ? onlineColor : offlineColor;

        if (statusLight != null)
        {
            statusLight.color = color;
            statusLight.intensity = repaired ? 3.5f : 2f;
        }

        if (screenRenderer != null)
        {
            screenRenderer.material.color = color;
            screenRenderer.material.SetColor("_EmissionColor", color);
        }
    }
}
