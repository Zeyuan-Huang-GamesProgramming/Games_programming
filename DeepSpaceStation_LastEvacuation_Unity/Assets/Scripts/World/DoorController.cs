using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform doorPanel;
    [SerializeField] private Vector3 openOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private float openSpeed = 4f;
    [SerializeField] private bool requiresRepairs;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool opened;

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

        if (requiresRepairs && GameManager.Instance != null && !GameManager.Instance.AllRepairsComplete)
        {
            return "Restore all systems to unlock this door";
        }

        return "Press E to open door";
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (opened)
        {
            return;
        }

        if (requiresRepairs && GameManager.Instance != null && !GameManager.Instance.AllRepairsComplete)
        {
            HUDController.Instance?.ShowMessage("Door locked by emergency protocol", 2.5f);
            return;
        }

        opened = true;
        targetPosition = closedPosition + openOffset;
    }
}
