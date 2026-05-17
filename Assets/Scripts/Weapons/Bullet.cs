using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 200f;
    public float lifeTime = 3f;

    private Vector3 direction;
    public LayerMask wallMask;
    void Start()
    {
        direction = -transform.forward;

        float checkRadius = 0.05f;
        if (Physics.CheckSphere(transform.position, checkRadius, wallMask))
        {
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        Vector3 displacement = direction * speed * Time.deltaTime;
        float distance = displacement.magnitude;

        if (Physics.Raycast(transform.position, displacement.normalized, out RaycastHit hit, distance))
        {
            HandleHit(hit.collider);
            return;
        }

        transform.position += displacement;
    }

    private void HandleHit(Collider other)
    {
        if (other.CompareTag("Bullet"))
            return;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider);
    }
}
