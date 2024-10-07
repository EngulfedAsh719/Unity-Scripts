using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public GameObject muzzleEffect;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    public bool isShooting, readyToShoot;
    public bool isActiveWeapon;
    private bool allowReset = true;
    public float shootingDelay = 2f;
    public float spreadIntensity;

    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    [Header("Reloading")] 
    public float reloadTime;

    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public enum WeaponModel
    {
        PistolColt,
        Riffle
    }

    public WeaponModel thisWeaponModel;
    
    internal Animator animator;

    public enum ShootingMode 
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;

        bulletsLeft = magazineSize;
    }

    private void Update()
    {
        if (isActiveWeapon)
        {
            GetComponent<Outline>().enabled = false;
                        
            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSound.Play();
            }
        
            if (currentShootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }

            else if (currentShootingMode == ShootingMode.Single ||
                currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && isShooting == false)
            {
                Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (AmmoManager.Instance.ammoDisplay != null)
            {
                AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
            }
        }
    }

    private void FireWeapon()
    {
        if (isReloading == false)
        {
            bulletsLeft--;
        
              muzzleEffect.GetComponent<ParticleSystem>().Play();
            animator.SetTrigger("Recoil");
        
            SoundManager.Instance.PlayShootingSound(thisWeaponModel);

            readyToShoot = false;

            Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            bullet.transform.forward = shootingDirection;
            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

            if (allowReset)
            {
                Invoke("ResetShot", shootingDelay);
                allowReset = false;
            }

            if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
            {
                burstBulletsLeft--;
                Invoke("FireWeapon", shootingDelay);
            }
        }
    }

    private void Reload()
    {
        animator.SetTrigger("Reload");
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }

        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
