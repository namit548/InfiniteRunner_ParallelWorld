using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public sealed class TapToMove : MonoBehaviour
{
    [SerializeField] private Collider ground;

    private Camera viewCamera;
    private NavMeshAgent agent;
    private NavMeshPath path;

    private void Awake()
    {
        viewCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        if (ground == null || viewCamera == null)
        {
            Debug.LogError("TapToMove needs a Ground collider and a camera tagged MainCamera.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                MoveToScreenPoint(touch.position, touch.fingerId);

            return;
        }

        if (Input.GetMouseButtonDown(0))
            MoveToScreenPoint(Input.mousePosition, -1);
    }

    private void MoveToScreenPoint(Vector2 screenPoint, int pointerId)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId))
            return;

        Ray ray = viewCamera.ScreenPointToRay(screenPoint);
        if (!ground.Raycast(ray, out RaycastHit groundHit, viewCamera.farClipPlane))
            return;

        var filter = new NavMeshQueryFilter
        {
            agentTypeID = agent.agentTypeID,
            areaMask = agent.areaMask
        };

        if (NavMesh.SamplePosition(groundHit.point, out NavMeshHit target, 0.5f, filter) &&
            agent.CalculatePath(target.position, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
        }
    }
}
