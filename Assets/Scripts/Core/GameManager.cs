using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int requiredRepairs = 3;
    [SerializeField] private float evacuationTimeLimit = 420f;

    private int completedRepairs;
    private float remainingTime;

    public bool IsGameEnded { get; private set; }
    public bool AllRepairsComplete => completedRepairs >= requiredRepairs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        remainingTime = evacuationTimeLimit;
    }

    private void Start()
    {
        HUDController.Instance?.SetObjective("Repair 3 station systems, then reach the escape pod.");
        HUDController.Instance?.SetRepairs(completedRepairs, requiredRepairs);
        HUDController.Instance?.ShowMessage("Emergency power fading. Find the terminals.", 4f);
    }

    private void Update()
    {
        if (IsGameEnded)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }

            return;
        }

        remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
        HUDController.Instance?.SetTimer(remainingTime);

        if (remainingTime <= 0f)
        {
            Lose("Evacuation window closed");
        }
    }

    public void RegisterRepair(string systemName)
    {
        if (IsGameEnded)
        {
            return;
        }

        completedRepairs = Mathf.Clamp(completedRepairs + 1, 0, requiredRepairs);
        HUDController.Instance?.SetRepairs(completedRepairs, requiredRepairs);
        HUDController.Instance?.ShowMessage(systemName + " restored", 3f);

        if (AllRepairsComplete)
        {
            HUDController.Instance?.SetObjective("All systems restored. Reach the escape pod.");
            HUDController.Instance?.ShowMessage("Escape pod authorization unlocked", 4f);
        }
    }

    public void Win()
    {
        if (IsGameEnded)
        {
            return;
        }

        IsGameEnded = true;
        UnlockCursor();
        HUDController.Instance?.ShowEndScreen("EVACUATION SUCCESSFUL", "You restored the derelict station long enough to launch.");
    }

    public void Lose(string reason)
    {
        if (IsGameEnded)
        {
            return;
        }

        IsGameEnded = true;
        UnlockCursor();
        HUDController.Instance?.ShowEndScreen("MISSION FAILED", reason);
    }

    private static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
