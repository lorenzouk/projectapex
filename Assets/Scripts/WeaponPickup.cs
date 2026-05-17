using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    [SerializeField] float spinSpeed = 90f;
    private WeaponSpawner spawner;

    public void SetSpawner(WeaponSpawner s)
    {
        spawner = s;
    }

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerShooting p = other.GetComponentInParent<PlayerShooting>();
        if (p == null) return;

        bool success = TryPickUp(p);

        if (success)
        {
            if (spawner != null)
            {
                spawner.NotifyCollected();
            }

            Destroy(gameObject, 0.1f);
        }
    }

    bool TryPickUp(PlayerShooting p)
    {
        for (int i = 0; i < p.weapons.Length; i++)
        {
            if (p.weapons[i] != null && p.weapons[i].gameObject.name.Contains(weaponPrefab.name))
            {
                Debug.Log("Duplicate weapon ignored");
                return false;
            }

            if (p.weapons[i] == null)
            {
                GameObject newWeapon = Instantiate(weaponPrefab);

                newWeapon.transform.SetParent(p.weaponSocket);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localRotation = Quaternion.identity;
                newWeapon.transform.localScale = Vector3.one * 2f;

                Gun gun = newWeapon.GetComponent<Gun>();
                p.weapons[i] = gun;

                p.EquipWeapon(i);

                return true;
            }
        }

        Debug.Log("Inventory full (5 weapons max)");
        return false;
    }
}