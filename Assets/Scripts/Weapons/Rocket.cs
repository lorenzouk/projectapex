using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 200f;
    public float lifeTime = 5f;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public float explosionYOffset = 0.5f;
    private Vector3 direction;
    private AudioSource audioSource;
    private bool hasExploded = false;
    public LayerMask wallMask;

    void Start()
    {
        direction = -transform.forward;
        audioSource = GetComponent<AudioSource>();

        float checkRadius = 0.1f;

        if (Physics.CheckSphere(transform.position, checkRadius, wallMask))
        {
            Explode(transform.position);
            return;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (hasExploded)
        {
            return;
        }

        Vector3 displacement = direction * speed * Time.deltaTime;
        float distance = displacement.magnitude;

        if (Physics.Raycast(transform.position, displacement.normalized, out RaycastHit hit, distance))
        {
            Explode(hit.point);
            return;
        }

        transform.position += displacement;
    }

    private void HandleHit(Collider other)
    {
        if (hasExploded)
        {
            return;
        }

        if (other.CompareTag("Bullet"))
        {
            return;
        }

        Explode(transform.position);
    }

    void Explode(Vector3 position)
    {
        hasExploded = true;
        Vector3 spawnPos = position + Vector3.up * explosionYOffset;
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, spawnPos);
        }

        if (CameraShake.Instance != null)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, spawnPos);
            float shakeAmount = Mathf.Clamp(1f / (distance * 0.5f), 0.05f, 0.6f);
            CameraShake.Instance.Shake(0.25f, shakeAmount);
        }

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