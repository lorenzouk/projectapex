using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Gun gun;
    private bool isShooting = false;

    void OnShoot()
    {
        isShooting = true;
    }

    void OnShootRelease()
    {
        isShooting = false;
    }

    void OnReload()
    {
        if (gun != null)
        {
            gun.TryReload();
        }
    }

    void Update()
    {
        if (isShooting && gun != null)
        {
            gun.Shoot();
        }
    }
}
