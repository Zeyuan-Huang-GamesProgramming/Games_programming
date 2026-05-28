using UnityEngine;

public class OxygenCanister : MonoBehaviour, IInteractable
{
    [SerializeField] private float oxygenAmount = 35f;
    [SerializeField] private AudioSource pickupAudio;

    private bool used;

    public string GetPrompt()
    {
        return used ? "Empty oxygen canister" : "Press E to refill oxygen";
    }

    public void Interact(GameObjectInteractor interactor)
    {
        if (used || interactor == null || interactor.Oxygen == null)
        {
            return;
        }

        used = true;
        interactor.Oxygen.AddOxygen(oxygenAmount);
        GameAudio.Instance?.PlayRefill();
        HUDController.Instance?.ShowMessage("Oxygen refilled", 2f);

        if (pickupAudio != null)
        {
            pickupAudio.Play();
        }

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
