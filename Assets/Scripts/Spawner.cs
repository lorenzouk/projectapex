using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    public GameObject[] weaponPrefabs;
    public Transform spawnPoint;

    [Header("Settings")]
    public float respawnDelay = 5f;

    private GameObject currentWeapon;
    private bool waitingToRespawn = false;

    void Start()
    {
        SpawnWeapon();
    }

    void SpawnWeapon()
    {
        if (weaponPrefabs.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, weaponPrefabs.Length);
        currentWeapon = Instantiate(weaponPrefabs[index], spawnPoint.position, spawnPoint.rotation);
        WeaponPickup pickup = currentWeapon.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            pickup.SetSpawner(this);
        }
    }

    public void NotifyCollected()
    {
        if (waitingToRespawn)
        {
            return;
        }

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        waitingToRespawn = true;
        currentWeapon = null;
        yield return new WaitForSeconds(respawnDelay);
        SpawnWeapon();
        waitingToRespawn = false;
    }
}