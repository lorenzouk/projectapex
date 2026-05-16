using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 200f;
    public float lifeTime = 3f;

    private Vector3 direction;

    void Start()
    {
        direction = -transform.forward;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("World"))
        {
            Destroy(gameObject);
        }
    }
}
