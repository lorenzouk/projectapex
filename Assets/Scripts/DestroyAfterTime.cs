using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time;
    void Update()
    {
        Destroy(gameObject, time);
    }
}
