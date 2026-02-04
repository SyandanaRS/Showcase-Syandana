using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 10f;

    [Header("References")]
    [SerializeField] CinemachineCamera fpsCam;
    [SerializeField] AudioSource gunFire;
    [SerializeField] FPController fpController;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform crosshair;
    [SerializeField] Animator bulletAnimator;
    [SerializeField] Animator magazineAnimator;

    bool isFiring;
    bool isReloading;

    public float nextTimeToFire = 0f;
    public float crosshairShootSize = 1.25f;
    public float crosshairReturnSpeed = 10f;
    Vector3 crosshairCurrentScale = Vector3.one;
    Vector3 crosshairTargetScale = Vector3.one;

    public void SetFiring(bool firing)
    {
        isFiring = firing;
        animator.SetBool("IsFiring", firing);
    }

    public void TryShoot()
    {
        if (isReloading)
        {
            return;
        }

        if(Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (!AmmoController.HasAmmo())
        {
            return;
        }

        // ammo
        AmmoController.ConsumeAmmo();

        // gun sound
        gunFire.PlayOneShot(gunFire.clip);

        // bullet animation
        if(bulletAnimator != null)
        {
            bulletAnimator.SetTrigger("Fire");
        }

        // crosshair movement
        crosshairTargetScale = Vector3.one * crosshairShootSize;

        // muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // recoil
        fpController.AddRecoil();

        // raycast
        Vector3 origin = fpsCam.transform.position + fpsCam.transform.forward * 0.1f;
        if(Physics.Raycast(origin, fpsCam.transform.forward, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
    }

    public void Reload()
    {
        if (isReloading) return;
        isReloading = true;

        if(magazineAnimator != null)
        {
            magazineAnimator.SetTrigger("Reload");
        }
        StartCoroutine(FinishReload());
    }

    private IEnumerator FinishReload()
    {
        yield return new WaitForSeconds(2f);
        isReloading = false;
        AmmoController.Reload();
    }

    void Update()
    {
    crosshairTargetScale = Vector3.Lerp(
        crosshairTargetScale,
        Vector3.one,
        crosshairReturnSpeed * Time.deltaTime
    );

    crosshairCurrentScale = Vector3.Lerp(
        crosshairCurrentScale,
        crosshairTargetScale,
        crosshairReturnSpeed * Time.deltaTime
    );

    crosshair.localScale = crosshairCurrentScale;

    if (isFiring && AmmoController.currentAmmo <= 0)
        {
            SetFiring(false);
        }
    }

}
