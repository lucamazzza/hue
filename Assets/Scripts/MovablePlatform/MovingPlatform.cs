using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 direction = Vector3.right;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 5f;

    private Vector3 startPos;
    private float currentDistance = 0f;
    private int directionFactor = 1;

    private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void FixedUpdate()
    {
        currentDistance += speed * Time.fixedDeltaTime * directionFactor;

        if (Mathf.Abs(currentDistance) >= distance)
        {
            directionFactor *= -1;
            currentDistance = Mathf.Clamp(currentDistance, -distance, distance);
        }

        Vector3 nextPosition = startPos + direction.normalized * currentDistance;

        if (rb != null)
            rb.MovePosition(nextPosition);
        else
            transform.position = nextPosition;
    }
}