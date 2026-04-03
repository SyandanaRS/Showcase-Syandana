using UnityEngine;
using TMPro;

public class AmmoController : MonoBehaviour
{
    public static int maxMagAmmo = 30;
    public static int currentMagAmmo = 30;
    public static int maxResrveAmmo = 90;
    public static int currentReserveAmmo = 90;

    [SerializeField] TMP_Text ammoDisplay;

    // Update is called once per frame
    void Update()
    {
        if(ammoDisplay != null)
        {
            ammoDisplay.text = currentMagAmmo + " / " + currentReserveAmmo;
        }
    }

    public static bool HasAmmoInMag()
    {
        return currentMagAmmo > 0;
    }

    public static void ConsumeAmmo(int amount = 1)
    {
        currentMagAmmo = Mathf.Max(0, currentMagAmmo - amount);
    }

    public static bool CanReload()
    {
        return currentReserveAmmo > 0 && currentMagAmmo < maxMagAmmo;
    }

    public static void Reload()
    {
        int bulletsNeeded = maxMagAmmo - currentMagAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, currentReserveAmmo);

        currentMagAmmo += bulletsToReload;
        currentReserveAmmo -= bulletsToReload;
    }

    public static void AddAmmo(int amount)
    {
        currentReserveAmmo += amount;
    }
}
