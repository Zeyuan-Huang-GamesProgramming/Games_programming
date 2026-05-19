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

        if (!GameManager.Instance.AllRepairsComplete)
        {
            HUDController.Instance?.ShowMessage("The pod needs all station systems online.", 2.5f);
            return;
        }

        GameManager.Instance.Win();
    }
}
