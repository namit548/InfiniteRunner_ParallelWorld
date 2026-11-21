using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask sightMask;

    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float sightDistance = 6f;
    [SerializeField] private float giveUpDistance = 10f;

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
        agent.SetDestination(nextPoint.position);

        while (agent.isOnNavMesh)
        {
            yield return checkInterval;

            float distance = Vector3.Distance(transform.position, player.position);

            if (chasing && distance > giveUpDistance)
            {
                chasing = false;
                agent.speed = patrolSpeed;
                agent.SetDestination(nextPoint.position);
                continue;
            }

            if (!chasing && distance <= sightDistance && CanSeePlayer())
            {
                chasing = true;
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