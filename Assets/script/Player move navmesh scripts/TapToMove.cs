using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public sealed class TapToMove : MonoBehaviour
{
    [SerializeField] private Collider ground;
    [SerializeField] private Color tapMarkerColor = new Color(0.15f, 0.9f, 1f, 1f);
    [SerializeField, Min(0.01f)] private float tapMarkerRadius = 0.3f;
    [SerializeField, Min(0.01f)] private float tapMarkerDuration = 0.6f;

    private Camera viewCamera;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private NavMeshLocomotionAnimator locomotionAnimator;
    private TapMarker tapMarker;

    private void Awake()
    {
        viewCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        locomotionAnimator = GetComponent<NavMeshLocomotionAnimator>();

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
            agent.CalculatePath(target.position, path) &&
            path.status == NavMeshPathStatus.PathComplete &&
            agent.SetPath(path))
        {
            ShowTapMarker(target.position, groundHit.normal);

            if (locomotionAnimator != null)
                locomotionAnimator.OnPathAccepted();
        }
    }

    private void ShowTapMarker(Vector3 position, Vector3 normal)
    {
        if (tapMarker == null)
        {
            var markerObject = new GameObject("Tap Marker");
            markerObject.layer = ground.gameObject.layer;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(markerObject, gameObject.scene);
            tapMarker = markerObject.AddComponent<TapMarker>();
        }

        tapMarker.Show(position, normal, tapMarkerColor, tapMarkerRadius, tapMarkerDuration);
    }

    private void OnDisable()
    {
        if (tapMarker != null)
            tapMarker.Hide();
    }

    private void OnDestroy()
    {
        if (tapMarker != null)
            Destroy(tapMarker.gameObject);
    }
}
