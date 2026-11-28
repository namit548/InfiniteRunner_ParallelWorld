using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private GameObject alertIndicator;
    [SerializeField] private GameObject stunStars;

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask sightMask;

    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float sightDistance = 6f;
    [SerializeField] private float giveUpDistance = 10f;
    private Transform viewCamera;

    public bool IsStunned { get; private set; }

    private NavMeshAgent patrolAgent;
    private Animator[] guardAnimators;
    private Rigidbody guardBody;
    private Coroutine stunRoutine;
    private bool wasAgentStopped;
    private bool[] wereAnimatorsEnabled;
    private bool wasBodyKinematic;
    private bool wasAlertVisible;
    private bool restoreAgentStop;

    private void Awake()
    {
        patrolAgent = GetComponent<NavMeshAgent>();
        // The visible character can have its own Animator below the guard root.
        guardAnimators = GetComponentsInChildren<Animator>(true);
        wereAnimatorsEnabled = new bool[guardAnimators.Length];
        guardBody = GetComponent<Rigidbody>();

        if (stunStars != null)
            stunStars.SetActive(false);
    }

    private void OnEnable()
    {
        RestoreAgentStop();
    }

    private void OnDisable()
    {
        if (!IsStunned)
            return;

        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        RecoverFromStun();
    }

    public void Stun(float duration)
    {
        if (!isActiveAndEnabled)
            return;

        if (!IsStunned)
        {
            IsStunned = true;
            wasAgentStopped = patrolAgent.isActiveAndEnabled &&
                              patrolAgent.isOnNavMesh && patrolAgent.isStopped;
            if (patrolAgent.isActiveAndEnabled && patrolAgent.isOnNavMesh)
            {
                patrolAgent.isStopped = true;
                patrolAgent.velocity = Vector3.zero;
            }

            if (guardBody != null)
            {
                wasBodyKinematic = guardBody.isKinematic;
                if (!guardBody.isKinematic)
                {
                    guardBody.linearVelocity = Vector3.zero;
                    guardBody.angularVelocity = Vector3.zero;
                    guardBody.isKinematic = true;
                }
            }

            for (int i = 0; i < guardAnimators.Length; i++)
            {
                if (guardAnimators[i] == null)
                    continue;

                wereAnimatorsEnabled[i] = guardAnimators[i].enabled;
                guardAnimators[i].enabled = false;
            }

            wasAlertVisible = alertIndicator != null && alertIndicator.activeSelf;
            ShowAlert(false);
        }

        if (stunStars != null)
        {
            stunStars.SetActive(true);
            foreach (ParticleSystem stars in stunStars.GetComponentsInChildren<ParticleSystem>())
                stars.Play(true);
        }

        // Another bomb refreshes the stun instead of restoring a guard too early.
        if (stunRoutine != null)
            StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(RecoverAfter(Mathf.Max(0.1f, duration)));
    }

    private IEnumerator RecoverAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        RecoverFromStun();
    }

    private void RecoverFromStun()
    {
        IsStunned = false;
        stunRoutine = null;
        restoreAgentStop = true;
        RestoreAgentStop();

        if (guardBody != null)
            guardBody.isKinematic = wasBodyKinematic;
        for (int i = 0; i < guardAnimators.Length; i++)
        {
            if (guardAnimators[i] != null)
                guardAnimators[i].enabled = wereAnimatorsEnabled[i];
        }
        if (stunStars != null)
        {
            foreach (ParticleSystem stars in stunStars.GetComponentsInChildren<ParticleSystem>())
                stars.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            stunStars.SetActive(false);
        }

        ShowAlert(wasAlertVisible);
    }

    private void RestoreAgentStop()
    {
        if (restoreAgentStop && patrolAgent != null &&
            patrolAgent.isActiveAndEnabled && patrolAgent.isOnNavMesh)
        {
            patrolAgent.isStopped = wasAgentStopped;
            restoreAgentStop = false;
        }
    }
    
//     private void Awake()
// {
//     if (Camera.main != null)
//         viewCamera = Camera.main.transform;

//     ShowAlert(false);
// }
private void LateUpdate()
{
    // Keep the icon facing the camera as the enemy turns.
    if (alertIndicator != null && viewCamera != null)
        alertIndicator.transform.rotation = viewCamera.rotation;
}
private void ShowAlert(bool visible)
{
    if (alertIndicator != null)
        alertIndicator.SetActive(visible);
}

    private IEnumerator Start()
    {
        var agent = GetComponent<NavMeshAgent>();

        if (pointA == null || pointB == null || player == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning("Assign the player and both points, and place the enemy on the NavMesh.", this);
            yield break;
        }

        var checkInterval = new WaitForSeconds(0.15f);
        Transform nextPoint = pointA;
        bool chasing = false;

        agent.speed = patrolSpeed;
        RestoreAgentStop();
        agent.SetDestination(nextPoint.position);

        while (agent.isOnNavMesh)
        {
            yield return checkInterval;

            if (IsStunned)
                continue;

            RestoreAgentStop();

            float distance = Vector3.Distance(transform.position, player.position);

            if (chasing && distance > giveUpDistance)
            {
                chasing = false;
                ShowAlert(false);
                agent.speed = patrolSpeed;
                agent.SetDestination(nextPoint.position);
                continue;
            }

            if (!chasing && distance <= sightDistance && CanSeePlayer())
            {
                chasing = true;
                  ShowAlert(true);
                agent.speed = chaseSpeed;
            }

            if (chasing)
            {
                agent.SetDestination(player.position);
            }
            else if (!agent.pathPending &&
                     agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                nextPoint = nextPoint == pointA ? pointB : pointA;
                agent.SetDestination(nextPoint.position);
            }
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        // 45 degrees on either side gives the guard a 90-degree view.
        if (Vector3.Angle(transform.forward, direction) > 45f)
            return false;

        Vector3 eyes = transform.position + Vector3.up * 1.25f;
        Vector3 chest = player.position + Vector3.up * 0.8f;

        if (!Physics.Linecast(eyes, chest, out RaycastHit hit,
                              sightMask, QueryTriggerInteraction.Ignore))
            return true;

        return hit.transform == player || hit.transform.IsChildOf(player);
    }
}
