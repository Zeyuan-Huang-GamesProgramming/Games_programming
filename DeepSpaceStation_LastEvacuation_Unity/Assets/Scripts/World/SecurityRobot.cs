using UnityEngine;

public class SecurityRobot : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 1.9f;
    [SerializeField] private float chaseSpeed = 3.35f;
    [SerializeField] private float detectionRange = 7.25f;
    [SerializeField] private float attackRange = 1.25f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 2.1f;
    [SerializeField] private float crouchDetectionMultiplier = 0.45f;
    [SerializeField] private float sprintDetectionMultiplier = 1.15f;
    [SerializeField] private float loseSightDelay = 1.15f;
    [SerializeField] private float searchDuration = 3.4f;
    [SerializeField] private float giveUpDistance = 12f;

    private Transform player;
    private PlayerHealth playerHealth;
    private FirstPersonController playerMovement;
    private int patrolIndex;
    private float attackTimer;
    private float lostSightTimer;
    private float searchTimer;
    private bool chasing;
    private bool securityOverrideApplied;
    private Vector3 lastKnownPlayerPosition;
    private float baseChaseSpeed;
    private float baseDetectionRange;
    private float baseAttackDamage;
    private float baseAttackCooldown;
    private float baseGiveUpDistance;
    private float endlessThreatMultiplier = 1f;

    public void ConfigurePatrol(Transform[] points)
    {
        patrolPoints = points;
    }

    public void ApplySecurityOverride()
    {
        if (securityOverrideApplied)
        {
            return;
        }

        securityOverrideApplied = true;
        RecalculateThreatStats();
        StopChase(false);
    }

    public void ApplyEndlessThreat(float multiplier)
    {
        endlessThreatMultiplier = Mathf.Max(1f, multiplier);
        RecalculateThreatStats();
    }

    private void Awake()
    {
        baseChaseSpeed = chaseSpeed;
        baseDetectionRange = detectionRange;
        baseAttackDamage = attackDamage;
        baseAttackCooldown = attackCooldown;
        baseGiveUpDistance = giveUpDistance;
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

    private void RecalculateThreatStats()
    {
        float threat = Mathf.Max(1f, endlessThreatMultiplier);
        float threatDelta = threat - 1f;

        chaseSpeed = Mathf.Min(baseChaseSpeed * (1f + threatDelta * 0.75f), 6.2f);
        detectionRange = Mathf.Min(baseDetectionRange * (1f + threatDelta * 1.25f), 15f);
        attackDamage = Mathf.Min(baseAttackDamage * (1f + threatDelta * 0.85f), 24f);
        attackCooldown = Mathf.Max(baseAttackCooldown / (1f + threatDelta * 0.35f), 0.85f);
        giveUpDistance = Mathf.Min(baseGiveUpDistance * (1f + threatDelta * 0.55f), 22f);

        if (securityOverrideApplied)
        {
            chaseSpeed *= 0.78f;
            detectionRange *= 0.72f;
            attackDamage *= 0.65f;
            attackCooldown *= 1.25f;
            giveUpDistance = Mathf.Min(giveUpDistance, 13.5f);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused || GameManager.Instance.IsChoosingMode)
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
                HUDController.Instance?.ShowMessage("Robot detected you - break line of sight", 2f);
            }

            lostSightTimer = loseSightDelay;
            searchTimer = searchDuration;
            lastKnownPlayerPosition = player.position;
            chasing = true;
        }
        else if (chasing)
        {
            lostSightTimer -= Time.deltaTime;
            searchTimer -= Time.deltaTime;
            if (ShouldGiveUpChase())
            {
                StopChase(true);
            }
        }

        if (chasing)
        {
            ChasePlayer(canSeePlayer);
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

    private void ChasePlayer(bool canSeePlayer)
    {
        if (player == null)
        {
            chasing = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 targetPosition = canSeePlayer ? player.position : lastKnownPlayerPosition;
        MoveToward(targetPosition, chaseSpeed);

        if (distance > giveUpDistance)
        {
            StopChase(true);
            return;
        }

        if (canSeePlayer && distance <= attackRange && attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            playerHealth?.TakeDamage(attackDamage);
            HUDController.Instance?.ShowMessage("Suit hit - sprint away or turn a corner", 1.8f);
        }
    }

    private bool ShouldGiveUpChase()
    {
        if (player == null)
        {
            return true;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceToLastKnown = Vector3.Distance(transform.position, lastKnownPlayerPosition);
        if (distanceToPlayer > giveUpDistance)
        {
            return true;
        }

        return lostSightTimer <= 0f && (searchTimer <= 0f || distanceToLastKnown < 0.55f);
    }

    private void StopChase(bool showMessage)
    {
        if (chasing && showMessage)
        {
            HUDController.Instance?.ShowMessage("Robot lost your signal", 1.8f);
        }

        chasing = false;
        lostSightTimer = 0f;
        searchTimer = 0f;
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
