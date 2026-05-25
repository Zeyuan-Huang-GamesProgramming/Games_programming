using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform doorPanel;
    [SerializeField] private Vector3 openOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private float openSpeed = 4f;
    [SerializeField] private bool requiresRepairs;
    [SerializeField] private int requiredRepairCount = -1;
    [SerializeField] private string requiredItemName;
    [SerializeField] private string requiredItemDisplayName;
    [SerializeField] private bool consumeRequiredItem;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool opened;
    private bool itemUnlocked;

    private void Awake()
    {
        if (doorPanel == null)
        {
            doorPanel = transform;
        }

        closedPosition = doorPanel.localPosition;
        targetPosition = closedPosition;
    }

    private void Update()
    {
        doorPanel.localPosition = Vector3.Lerp(doorPanel.localPosition, targetPosition, openSpeed * Time.deltaTime);
    }

    public string GetPrompt()
    {
        if (opened)
        {
            return "Door open";
        }

        if (requiresRepairs && GameManager.Instance != null && GameManager.Instance.CompletedRepairs < GetRequiredRepairs())
        {
            return "Restore " + GetRequiredRepairs() + " systems to unlock";
        }

        if (!itemUnlocked && !string.IsNullOrEmpty(requiredItemName))
        {
            return "Requires " + GetRequiredItemDisplayName();
        }

        return "Press E to open door";
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (opened)
        {
            return;
        }

        if (requiresRepairs && GameManager.Instance != null && GameManager.Instance.CompletedRepairs < GetRequiredRepairs())
        {
            HUDController.Instance?.ShowMessage("Door locked by emergency protocol", 2.5f);
            return;
        }

        if (!itemUnlocked && !string.IsNullOrEmpty(requiredItemName))
        {
            PlayerInventory inventory = interactor == null ? null : interactor.Inventory;
            if (inventory == null || !inventory.HasItem(requiredItemName))
            {
                HUDController.Instance?.ShowMessage("Need " + GetRequiredItemDisplayName(), 2.5f);
                return;
            }

            if (consumeRequiredItem)
            {
                inventory.ConsumeItem(requiredItemName);
            }

            itemUnlocked = true;
            HUDController.Instance?.ShowMessage(GetRequiredItemDisplayName() + " accepted", 2f);
        }

        opened = true;
        targetPosition = closedPosition + openOffset;
    }

    private int GetRequiredRepairs()
    {
        if (requiredRepairCount > 0)
        {
            return requiredRepairCount;
        }

        return GameManager.Instance == null ? 1 : GameManager.Instance.RequiredRepairs;
    }

    private string GetRequiredItemDisplayName()
    {
        return string.IsNullOrEmpty(requiredItemDisplayName) ? requiredItemName : requiredItemDisplayName;
    }
}
