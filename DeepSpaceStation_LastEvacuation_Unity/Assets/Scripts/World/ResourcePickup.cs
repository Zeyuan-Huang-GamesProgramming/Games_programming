using UnityEngine;

public class ResourcePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "resource";
    [SerializeField] private string displayName = "Resource";

    private bool collected;

    public string GetPrompt()
    {
        return collected ? displayName + " collected" : "Press E to take " + displayName;
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (collected || interactor == null || interactor.Inventory == null)
        {
            return;
        }

        collected = true;
        interactor.Inventory.AddItem(itemName, displayName);
        HUDController.Instance?.ShowMessage(displayName + " acquired", 2.4f);

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
