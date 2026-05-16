using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Gun Statistics")]
    public float fireRate = 0.1f;
    public int magSize = 20;
    public GameObject bullet;
    public Transform bulletSpawnPoint;
    public GameObject muzzleFlash;

    [Header("Recoil Settings")]
    public float recoilDistance = 0.05f;
    public float recoilSpeed = 15f;

    [Header("Bloom Settings")]
    public float bloomAmount = 1f;

    [Header("Shotgun Settings")]
    public bool useShotgunSpread = false;
    public float spreadAmount = 1f;
    public int pelletCount = 1;

    [Header("Reload Settings")]
    public float reloadTime = 1f;
    public Vector3 reloadRotationOffset = new Vector3(66, 50, 50);
    [Range(0f, 1f)] public float upTimePercent = 0.4f;
    [Range(0f, 1f)] public float holdTimePercent = 0.2f;
    [Range(0f, 1f)] public float downTimePercent = 0.4f;

    [Header("Audio Settings")]
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

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 shootDirection = bulletSpawnPoint.forward;
            if (useShotgunSpread)
            {
                shootDirection = Quaternion.Euler(Random.Range(-spreadAmount, spreadAmount), Random.Range(-spreadAmount, spreadAmount), 0) * shootDirection;
                Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);
                Instantiate(bullet, bulletSpawnPoint.position, bulletRotation);
            }
            else
            {
                shootDirection += new Vector3(Random.Range(-bloomAmount, bloomAmount), Random.Range(-bloomAmount, bloomAmount), Random.Range(-bloomAmount, bloomAmount)) * 0.01f;
                Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);
                Instantiate(bullet, bulletSpawnPoint.position, bulletRotation);
            }
        }
        Instantiate(muzzleFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        PlaySound(shootSound, shootPitchMin, shootPitchMax);
        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));
    }

    IEnumerator Reload()
    {
        isReloading = true;

        PlaySound(reloadSound, 0.95f, 1.05f);

        Quaternion startRot = initalRotation;
        Quaternion upRot = Quaternion.Euler(initalRotation.eulerAngles + reloadRotationOffset);

        float upTime = reloadTime * upTimePercent;
        float holdTime = reloadTime * holdTimePercent;
        float downTime = reloadTime * downTimePercent;

        float t = 0f;
        while (t < upTime)
        {
            t += Time.deltaTime;
            float p = t / upTime;

            p = Mathf.SmoothStep(0, 1, p);
            transform.localRotation = Quaternion.Slerp(startRot, upRot, p);
            yield return null;
        }

        transform.localRotation = upRot;
        yield return new WaitForSeconds(holdTime);

        t = 0f;
        while (t < downTime)
        {
            t += Time.deltaTime;
            float p = t / downTime;
            p = 1f - Mathf.Pow(1f - p, 3f);
            transform.localRotation = Quaternion.Slerp(upRot, startRot, p);
            yield return null;
        }

        transform.localRotation = startRot;

        currentAmmo = magSize;
        isReloading = false;
    }

    public void TryReload()
    { 
        if (isReloading)
        {
            return;
        }

        if (currentAmmo >= magSize)
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
