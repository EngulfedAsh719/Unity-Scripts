using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;

    public AudioClip RiffleShooting;
    public AudioClip PistolColtShooting;

    public AudioSource riffleReloadingSound;
    public AudioSource pistolReloadingSound;


    public AudioSource emptyMagazineSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.PistolColt:
                ShootingChannel.PlayOneShot(PistolColtShooting);
                break;
            case WeaponModel.Riffle:
                ShootingChannel.PlayOneShot(RiffleShooting);
                break;

        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.PistolColt:
                pistolReloadingSound.Play();
                break;
            case WeaponModel.Riffle:
                riffleReloadingSound.Play();
                break;
        }
    }
}
