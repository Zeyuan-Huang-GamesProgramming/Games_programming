using UnityEngine;

public class GameObjectInteractor : MonoBehaviour
{
    [SerializeField] private Camera viewCamera;
    [SerializeField] private float interactDistance = 3.2f;
    [SerializeField] private LayerMask interactMask = ~0;

    private IInteractable currentTarget;

    public PlayerOxygen Oxygen { get; private set; }
    public PlayerInventory Inventory { get; private set; }

    private void Awake()
    {
        Oxygen = GetComponent<PlayerOxygen>();
        Inventory = GetComponent<PlayerInventory>();
        if (viewCamera == null)
        {
            viewCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused || GameManager.Instance.IsChoosingMode)
        {
            HUDController.Instance?.SetPrompt("");
            return;
        }

        ScanForTarget();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.Interact(this);
        }
    }

    private void ScanForTarget()
    {
        currentTarget = null;

        if (viewCamera == null)
        {
            HUDController.Instance?.SetPrompt("");
            return;
        }

        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
        {
            currentTarget = hit.collider.GetComponentInParent<IInteractable>();
        }

        HUDController.Instance?.SetPrompt(currentTarget == null ? "" : currentTarget.GetPrompt());
    }
}
