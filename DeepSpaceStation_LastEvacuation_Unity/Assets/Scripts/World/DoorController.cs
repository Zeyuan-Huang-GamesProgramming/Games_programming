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
    private bool opening;
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
        if (doorPanel == null || !opening)
        {
            return;
        }

        doorPanel.localPosition = Vector3.MoveTowards(
            doorPanel.localPosition,
            targetPosition,
            openSpeed * Time.deltaTime);

        if (Vector3.SqrMagnitude(doorPanel.localPosition - targetPosition) <= 0.0001f)
        {
            doorPanel.localPosition = targetPosition;
            opening = false;
            opened = true;
            ClearDoorwayBlockers();
        }
    }

    public string GetPrompt()
    {
        if (opened)
        {
            return "Door open";
        }

        if (opening)
        {
            return "Door opening";
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
        if (opened || opening)
        {
            return;
        }

        if (requiresRepairs && GameManager.Instance != null && GameManager.Instance.CompletedRepairs < GetRequiredRepairs())
        {
            GameAudio.Instance?.PlayDenied();
            HUDController.Instance?.ShowMessage("Door locked by emergency protocol", 2.5f);
            return;
        }

        if (!itemUnlocked && !string.IsNullOrEmpty(requiredItemName))
        {
            PlayerInventory inventory = interactor == null ? null : interactor.Inventory;
            if (inventory == null || !inventory.HasItem(requiredItemName))
            {
                GameAudio.Instance?.PlayDenied();
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

        opening = true;
        targetPosition = closedPosition + openOffset;
        GameAudio.Instance?.PlayDoorOpen();
    }

    private void ClearDoorwayBlockers()
    {
        DisableDoorBlockers();
        DisableCentralDoorwayObjects();
    }

    private void DisableDoorBlockers()
    {
        if (doorPanel == null)
        {
            return;
        }

        Collider[] colliders = doorPanel.GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                collider.enabled = false;
            }
        }
    }

    private void DisableCentralDoorwayObjects()
    {
        BoxCollider trigger = GetComponent<BoxCollider>();
        float clearHalfWidth = 1.8f;
        if (trigger != null)
        {
            clearHalfWidth = Mathf.Max(1.2f, (trigger.size.x - 1.3f) * 0.5f + 0.2f);
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer sceneRenderer in renderers)
        {
            if (ShouldClearDoorwayObject(sceneRenderer.transform, clearHalfWidth))
            {
                sceneRenderer.enabled = false;
            }
        }

        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            if (collider == trigger || collider.isTrigger)
            {
                continue;
            }

            if (IsPermanentDoorFrame(collider.transform) || ShouldClearDoorwayObject(collider.transform, clearHalfWidth))
            {
                collider.enabled = false;
            }
        }
    }

    private bool ShouldClearDoorwayObject(Transform candidate, float clearHalfWidth)
    {
        if (candidate == null || candidate == transform)
        {
            return false;
        }

        string objectName = candidate.name.ToLowerInvariant();
        if (IsPermanentDoorFrame(candidate))
        {
            return false;
        }

        if (objectName.Contains("sliding panel") || objectName.Contains("status strip") || objectName.Contains("center access line"))
        {
            return true;
        }

        Vector3 localPosition = transform.InverseTransformPoint(candidate.position);
        return Mathf.Abs(localPosition.x) <= clearHalfWidth
            && localPosition.y > -1.25f
            && localPosition.y < 1.35f
            && Mathf.Abs(localPosition.z) < 0.85f;
    }

    private bool IsPermanentDoorFrame(Transform candidate)
    {
        return candidate != null && candidate.name.ToLowerInvariant().Contains("permanent frame");
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
