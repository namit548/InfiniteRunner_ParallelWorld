using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyScript : MonoBehaviour
{
    private static readonly float[] LanePattern = { 0f, 3f, 0f, -3f };
    private Rigidbody body;
    private bool grounded;
    private int laneIndex;
    private float laneWait = 1f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.constraints &= ~RigidbodyConstraints.FreezePositionX;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        laneIndex = body.position.x > 1.5f ? 1 : body.position.x < -1.5f ? 3 : 0;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = body.linearVelocity;
        float lateralSpeed = 0f;
        if (grounded)
        {
            float distance = LanePattern[laneIndex] - body.position.x;
            lateralSpeed = Mathf.Clamp(distance / Time.fixedDeltaTime, -5f, 5f);
            if (Mathf.Abs(distance) < 0.01f)
            {
                laneWait -= Time.fixedDeltaTime;
                if (laneWait <= 0f)
                {
                    laneIndex = (laneIndex + 1) % LanePattern.Length;
                    laneWait = 1f;
                }
            }
        }
        body.linearVelocity = new Vector3(lateralSpeed, velocity.y, velocity.z);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground") || body.linearVelocity.y > 0.2f)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.6f)
            {
                grounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            grounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!grounded || !other.CompareTag("Obstacle"))
            return;

        grounded = false;
        body.linearVelocity = new Vector3(0f, 8f, body.linearVelocity.z);
    }
}
