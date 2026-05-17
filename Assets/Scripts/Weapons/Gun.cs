using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    private enum WeaponState
    {
        Idle,
        Firing,
        Reloading,
        Swapping
    }

    private WeaponState state = WeaponState.Idle;

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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Range(0.95f, 1.05f)] public float shootPitchMin = 0.97f;
    [Range(0.95f, 1.05f)] public float shootPitchMax = 1.03f;

    [Range(0.9f, 1.1f)] public float volumeMin = 0.95f;
    [Range(0.9f, 1.1f)] public float volumeMax = 1f;

    private int currentAmmo;
    private float nextTimeToFire;

    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private Coroutine recoilRoutine;
    private Coroutine reloadRoutine;

    void Start()
    {
        currentAmmo = magSize;
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
    }

    public void Shoot()
    {
        if (state == WeaponState.Reloading || state == WeaponState.Swapping)
            return;

        if (Time.time < nextTimeToFire)
            return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        state = WeaponState.Firing;
        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;

        FireBullets();
        PlaySoundDetached(shootSound, bulletSpawnPoint.position, shootPitchMin, shootPitchMax);

        StartRecoil();

        state = WeaponState.Idle;
    }

    void FireBullets()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 dir = bulletSpawnPoint.forward;

            if (useShotgunSpread)
            {
                dir = Quaternion.Euler(
                    Random.Range(-spreadAmount, spreadAmount),
                    Random.Range(-spreadAmount, spreadAmount),
                    0
                ) * dir;
            }
            else
            {
                dir += new Vector3(
                    Random.Range(-bloomAmount, bloomAmount),
                    Random.Range(-bloomAmount, bloomAmount),
                    Random.Range(-bloomAmount, bloomAmount)
                ) * 0.01f;
            }

            Instantiate(bullet, bulletSpawnPoint.position, Quaternion.LookRotation(dir));
        }

        Instantiate(muzzleFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }


    public void StartReload()
    {
        if (state == WeaponState.Reloading || state == WeaponState.Swapping)
            return;

        if (currentAmmo >= magSize)
            return;

        if (reloadRoutine != null)
            StopCoroutine(reloadRoutine);

        reloadRoutine = StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        state = WeaponState.Reloading;

        PlaySound(reloadSound, 0.95f, 1.05f);

        Quaternion startRot = initialRotation;
        Quaternion upRot = Quaternion.Euler(initialRotation.eulerAngles + reloadRotationOffset);

        float upTime = reloadTime * upTimePercent;
        float holdTime = reloadTime * holdTimePercent;
        float downTime = reloadTime * downTimePercent;

        float t = 0f;

        while (t < upTime)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / upTime);
            transform.localRotation = Quaternion.Slerp(startRot, upRot, p);
            yield return null;
        }

        transform.localRotation = upRot;
        yield return new WaitForSeconds(holdTime);

        t = 0f;
        while (t < downTime)
        {
            t += Time.deltaTime;
            float p = 1f - Mathf.Pow(1f - (t / downTime), 3f);
            transform.localRotation = Quaternion.Slerp(upRot, startRot, p);
            yield return null;
        }

        transform.localRotation = startRot;

        currentAmmo = magSize;

        state = WeaponState.Idle;
        reloadRoutine = null;
    }

    public void TryReload()
    {
        StartReload();
    }

    void StartRecoil()
    {
        if (recoilRoutine != null)
            StopCoroutine(recoilRoutine);

        recoilRoutine = StartCoroutine(Recoil());
    }

    IEnumerator Recoil()
    {
        Vector3 target = initialPosition + new Vector3(0, 0, -recoilDistance);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(initialPosition, target, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(target, initialPosition, t);
            yield return null;
        }

        transform.localPosition = initialPosition;
        recoilRoutine = null;
    }

    public void CancelAllActions()
    {
        state = WeaponState.Swapping;

        if (reloadRoutine != null)
            StopCoroutine(reloadRoutine);

        if (recoilRoutine != null)
            StopCoroutine(recoilRoutine);

        reloadRoutine = null;
        recoilRoutine = null;

        transform.localRotation = initialRotation;
        transform.localPosition = initialPosition;

        state = WeaponState.Idle;
    }

    void PlaySound(AudioClip clip, float pitchMin, float pitchMax)
    {
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = Random.Range(volumeMin, volumeMax);

        audioSource.PlayOneShot(clip);

        audioSource.pitch = 1f;
        audioSource.volume = 1f;
    }

    void PlaySoundDetached(AudioClip clip, Vector3 position, float pitchMin, float pitchMax)
    {
        GameObject tempAudio = new GameObject("OneShotAudio");
        tempAudio.transform.position = position;

        AudioSource src = tempAudio.AddComponent<AudioSource>();
        src.clip = clip;
        src.pitch = Random.Range(pitchMin, pitchMax);
        src.volume = Random.Range(volumeMin, volumeMax);
        src.spatialBlend = 0f;

        src.Play();

        Destroy(tempAudio, clip.length / src.pitch);
    }
}