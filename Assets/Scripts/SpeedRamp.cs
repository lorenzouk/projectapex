using UnityEngine;

public class SpeedRamp : MonoBehaviour
{
    public float boostPower = 15f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null)
        {
            return;
        }

        Vector3 boostDir = transform.forward;
        boostDir.y = 0f;
        boostDir.Normalize();
        rb.linearVelocity += boostDir * boostPower * Time.deltaTime;
    }
}