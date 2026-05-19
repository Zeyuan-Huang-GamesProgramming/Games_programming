using UnityEngine;

public class HazardZone : MonoBehaviour
{
    [SerializeField] private float oxygenDrainMultiplier = 2.6f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerOxygen oxygen = other.GetComponent<PlayerOxygen>();
        if (oxygen != null)
        {
            oxygen.SetHazardMultiplier(oxygenDrainMultiplier);
            HUDController.Instance?.ShowMessage("Radiation leak: oxygen drain increased", 2.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerOxygen oxygen = other.GetComponent<PlayerOxygen>();
        if (oxygen != null)
        {
            oxygen.SetHazardMultiplier(1f);
        }
    }
}
