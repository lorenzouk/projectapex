using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    public float reloadTime = 1f;
    public float fireRate = 0.1f;
    public int magSize = 20;
    public GameObject bullet;
    public Transform bulletSpawnPoint;
    public GameObject muzzleFlash;

    [Header("Recoil")]
    public float recoilDistance = 0.05f;
    public float recoilSpeed = 15f;

    [Header("Bloom")]
    public float bloomAmount = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    [Range(0.95f, 1.05f)] public float shootPitchMin = 0.97f;
    [Range(0.95f, 1.05f)] public float shootPitchMax = 1.03f;
    [Range(0.9f, 1.1f)] public float volumeMin = 0.95f;
    [Range(0.9f, 1.1f)] public float volumeMax = 1f;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    private Quaternion initalRotation;
    private Vector3 initalPosition;
    private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);

    void Start()
    {
        currentAmmo = magSize;
        initalRotation = transform.localRotation;
        initalPosition = transform.localPosition;
    }

    void PlaySound(AudioClip clip, float pitchMin, float pitchMax)
    {
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = Random.Range(volumeMin, volumeMax);

        audioSource.PlayOneShot(clip);

        audioSource.pitch = 1f;
        audioSource.volume = 1f;
    }

    public void Shoot()
    {
        if (isReloading || Time.time < nextTimeToFire)
        {
            return;
        }
        
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;

        Vector3 shootDirection = bulletSpawnPoint.forward;
        shootDirection += new Vector3(Random.Range(-bloomAmount, bloomAmount), Random.Range(-bloomAmount, bloomAmount), Random.Range(-bloomAmount, bloomAmount)) * 0.01f;

        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);
        Instantiate(bullet, bulletSpawnPoint.position, bulletRotation);
        Instantiate(muzzleFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        PlaySound(shootSound, shootPitchMin, shootPitchMax);

        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));
    }

    IEnumerator Reload()
    {
        isReloading = true;

        PlaySound(reloadSound, 0.95f, 1.05f);

        Quaternion targetRotation = Quaternion.Euler(initalRotation.eulerAngles + reloadRotationOffset);
        float halfReload = reloadTime / 2f;
        float t = 0f;
        
        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initalRotation, targetRotation, t / halfReload);
            yield return null;
        }

        t = 0f;

        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initalRotation, t / halfReload);
            yield return null;
        }

        currentAmmo = magSize;
        isReloading = false;
    }

    public void TryReload()
    { 
        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            return;
        }

        StartCoroutine(Reload());
    }

    private IEnumerator Recoil()
    {
        Vector3 recoilTarget = initalPosition + new Vector3(0, 0, -recoilDistance);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(initalPosition, recoilTarget, t);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(recoilTarget, initalPosition, t);
            yield return null;
        }

        transform.localPosition = initalPosition;
    }
}
