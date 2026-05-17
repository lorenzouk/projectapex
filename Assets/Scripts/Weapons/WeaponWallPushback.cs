using UnityEngine;

public class WeaponWallPushback : MonoBehaviour
{
    public Transform gun;
    public float maxDistance = 1.0f;
    public LayerMask wallMask;
    public float smooth = 10f;

    private Vector3 originalLocalPos;

    void Start()
    {
        originalLocalPos = gun.localPosition;
    }

    void LateUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, wallMask))
        {
            float dist = hit.distance;
            Vector3 targetPos = originalLocalPos * (dist / maxDistance);

            gun.localPosition = Vector3.Lerp(
                gun.localPosition,
                targetPos,
                Time.deltaTime * smooth
            );
        }
        else
        {
            gun.localPosition = Vector3.Lerp(
                gun.localPosition,
                originalLocalPos,
                Time.deltaTime * smooth
            );
        }
    }
}