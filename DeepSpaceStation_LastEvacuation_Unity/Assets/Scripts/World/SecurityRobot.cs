using UnityEngine;

public class SecurityRobot : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2.1f;
    [SerializeField] private float chaseSpeed = 4.2f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.7f;
    [SerializeField] private float attackDamage = 16f;
    [SerializeField] private float attackCooldown = 1.25f;
    [SerializeField] private float crouchDetectionMultiplier = 0.45f;
    [SerializeField] private float sprintDetectionMultiplier = 1.25f;
    [SerializeField] private float loseSightDelay = 2f;

    private Transform player;
    private PlayerHealth playerHealth;
    private FirstPersonController playerMovement;
    private int patrolIndex;
    private float attackTimer;
    private float lostSightTimer;
    private bool chasing;

    public void ConfigurePatrol(Transform[] points)
    {
        patrolPoints = points;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
            playerMovement = playerObject.GetComponent<FirstPersonController>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused)
        {
            return;
        }

        attackTimer = Mathf.Max(0f, attackTimer - Time.deltaTime);

        bool canSeePlayer = CanDetectPlayer();
        if (canSeePlayer)
        {
            if (!chasing)
            {
                GameManager.Instance.RegisterDetection();
            }

            lostSightTimer = loseSightDelay;
            chasing = true;
        }
        else if (chasing)
        {
            lostSightTimer -= Time.deltaTime;
            if (lostSightTimer <= 0f)
            {
                chasing = false;
            }
        }

        if (chasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        Transform target = patrolPoints[patrolIndex];
        MoveToward(target.position, patrolSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.35f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        if (player == null)
        {
            chasing = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        MoveToward(player.position, chaseSpeed);

        if (distance > detectionRange * 2f)
        {
            chasing = false;
            return;
        }

        if (distance <= attackRange && attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            playerHealth?.TakeDamage(attackDamage);
            HUDController.Instance?.ShowMessage("Suit damaged by security robot", 1.8f);
        }
    }

    private bool CanDetectPlayer()
    {
        if (player == null)
        {
            return false;
        }

        float range = detectionRange;
        if (playerMovement != null)
        {
            if (playerMovement.IsCrouching)
            {
                range *= crouchDetectionMultiplier;
            }
            else if (playerMovement.IsSprinting)
            {
                range *= sprintDetectionMultiplier;
            }
        }

        Vector3 eyePosition = transform.position + Vector3.up * 1.25f;
        Vector3 playerTarget = player.position + Vector3.up * (playerMovement != null && playerMovement.IsCrouching ? 0.75f : 1.35f);
        Vector3 toPlayer = playerTarget - eyePosition;
        float distance = toPlayer.magnitude;
        if (distance > range)
        {
            return false;
        }

        Vector3 direction = toPlayer.normalized;
        if (!chasing && Vector3.Angle(transform.forward, direction) > 105f)
        {
            return false;
        }

        if (Physics.Raycast(eyePosition, direction, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.transform == player || hit.transform.IsChildOf(player);
        }

        return false;
    }

    private void MoveToward(Vector3 target, float speed)
    {
        Vector3 next = Vector3.MoveTowards(transform.position, new Vector3(target.x, transform.position.y, target.z), speed * Time.deltaTime);
        Vector3 direction = next - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        transform.position = next;
    }
}
