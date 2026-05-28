using UnityEngine;

public class PlayerItemUser : MonoBehaviour
{
    [SerializeField] private float medkitHealAmount = 35f;
    [SerializeField] private float batteryRechargeAmount = 45f;

    private PlayerHealth health;
    private PlayerInventory inventory;
    private PlayerScanner scanner;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        inventory = GetComponent<PlayerInventory>();
        scanner = GetComponent<PlayerScanner>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            UseMedkit();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            UseBattery();
        }
    }

    private void UseMedkit()
    {
        if (inventory == null || !inventory.ConsumeItem("medkit"))
        {
            GameAudio.Instance?.PlayDenied();
            HUDController.Instance?.ShowMessage("No medkit in backpack", 2f);
            return;
        }

        health?.Heal(medkitHealAmount);
        HUDController.Instance?.ShowMessage("Medkit used", 2f);
    }

    private void UseBattery()
    {
        if (inventory == null || !inventory.ConsumeItem("battery"))
        {
            GameAudio.Instance?.PlayDenied();
            HUDController.Instance?.ShowMessage("No battery pack in backpack", 2f);
            return;
        }

        scanner?.Recharge(batteryRechargeAmount);
        GameAudio.Instance?.PlayRecharge();
        HUDController.Instance?.ShowMessage("Scanner battery recharged", 2f);
    }
}
