using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public sealed class NavMeshLocomotionAnimator : MonoBehaviour
{
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

    private NavMeshAgent agent;
    private Animator animator;
    private Coroutine movementWatch;
    private readonly WaitForSeconds speedCheckDelay = new WaitForSeconds(0.05f);

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void OnPathAccepted()
    {
        if (isActiveAndEnabled && movementWatch == null)
            movementWatch = StartCoroutine(WatchMovement());
    }

    private IEnumerator WatchMovement()
    {
        // Let the agent apply the new path before reading its velocity.
        yield return null;

        while (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat(MoveSpeed, speed);

            bool followingPath = !agent.isStopped && (agent.pathPending ||
                (agent.hasPath && agent.remainingDistance > agent.stoppingDistance));
            if (!followingPath && speed <= 0.05f)
                break;

            yield return speedCheckDelay;
        }

        animator.SetFloat(MoveSpeed, 0f);
        movementWatch = null;
    }

    private void OnDisable()
    {
        if (movementWatch != null)
        {
            StopCoroutine(movementWatch);
            movementWatch = null;
        }

        if (animator != null)
            animator.SetFloat(MoveSpeed, 0f);
    }
}
