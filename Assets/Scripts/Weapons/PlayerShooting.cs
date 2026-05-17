using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Setup")]
    public Transform weaponSocket;

    [Header("Starting Weapon")]
    public GameObject startingARPrefab;

    [Header("Inventory")]
    public Gun[] weapons = new Gun[5];
    public int currentWeaponIndex = 0;

    private bool isShooting;

    void Start()
    {
        SpawnStartingWeapon();
    }

    void SpawnStartingWeapon()
    {
        if (startingARPrefab == null)
        {
            return;
        }

        GameObject ar = Instantiate(startingARPrefab, weaponSocket);

        ar.transform.localPosition = new Vector3(0f, 2f, 0f);
        ar.transform.localRotation = Quaternion.identity;
        ar.transform.localScale = Vector3.one * 2.5f;

        Gun gun = ar.GetComponent<Gun>();
        weapons[0] = gun;
        currentWeaponIndex = 0;

        EquipWeapon(0);
    }

    void Update()
    {
        if (isShooting && weapons[currentWeaponIndex] != null)
        {
            weapons[currentWeaponIndex].Shoot();
        }
    }

    public void OnShoot()
    {
        isShooting = true;
    }

    public void OnShootRelease()
    {
        isShooting = false;
    }

    public void OnReload()
    {
        if (weapons[currentWeaponIndex] != null)
            weapons[currentWeaponIndex].TryReload();
    }

    public void OnSlot1() => EquipWeapon(0);
    public void OnSlot2() => EquipWeapon(1);
    public void OnSlot3() => EquipWeapon(2);
    public void OnSlot4() => EquipWeapon(3);
    public void OnSlot5() => EquipWeapon(4);


    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        if (weapons[index] == null) return;

        if (weapons[currentWeaponIndex] != null)
            weapons[currentWeaponIndex].CancelAllActions();

        if (weapons[currentWeaponIndex] != null)
            weapons[currentWeaponIndex].gameObject.SetActive(false);

        currentWeaponIndex = index;
        weapons[currentWeaponIndex].gameObject.SetActive(true);
    }
}