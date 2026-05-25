using UnityEngine;

public class PlayerFallGuard : MonoBehaviour
{
    [SerializeField] private float failHeight = -8f;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameEnded)
        {
            return;
        }

        if (transform.position.y < failHeight)
        {
            GameManager.Instance.Lose("Astronaut drifted below the station deck");
        }
    }
}
