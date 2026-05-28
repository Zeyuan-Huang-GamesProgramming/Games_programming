using UnityEngine;

public class ConsumablePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "medkit";
    [SerializeField] private string displayName = "Medkit";
    [SerializeField] private int quantity = 1;

    private bool collected;

    public string GetPrompt()
    {
        return collected ? displayName + " collected" : "Press E to pick up " + displayName;
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (collected || interactor == null || interactor.Inventory == null)
        {
            return;
        }

        collected = true;
        interactor.Inventory.AddItem(itemName, displayName, quantity);
        GameAudio.Instance?.PlayPickup();
        HUDController.Instance?.ShowMessage(displayName + " added to backpack", 2.4f);

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }
}
