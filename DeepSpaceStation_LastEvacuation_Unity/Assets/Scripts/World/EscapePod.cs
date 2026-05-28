using UnityEngine;

public class EscapePod : MonoBehaviour, IInteractable
{
    [SerializeField] private Light beacon;

    private void Update()
    {
        if (beacon != null && GameManager.Instance != null)
        {
            beacon.color = GameManager.Instance.AllRepairsComplete ? Color.green : Color.red;
        }
    }

    public string GetPrompt()
    {
        if (GameManager.Instance != null && GameManager.Instance.EndlessModeActive)
        {
            return "Endless survival active: keep scoring until death";
        }

        if (GameManager.Instance != null && GameManager.Instance.AllRepairsComplete)
        {
            return "Press E to launch escape pod";
        }

        return "Escape pod locked: repairs incomplete";
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.EndlessModeActive)
        {
            GameAudio.Instance?.PlayDenied();
            HUDController.Instance?.ShowMessage("No extraction in Endless Mode. Keep repairing for a higher score.", 2.8f);
            return;
        }

        if (!GameManager.Instance.AllRepairsComplete)
        {
            GameAudio.Instance?.PlayDenied();
            HUDController.Instance?.ShowMessage("The pod needs all station systems online.", 2.5f);
            return;
        }

        GameManager.Instance.Win();
    }
}
