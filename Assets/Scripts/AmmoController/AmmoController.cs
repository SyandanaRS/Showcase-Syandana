using UnityEngine;
using TMPro;

public class AmmoController : MonoBehaviour
{
    public static int maxAmmo = 30;
    public static int currentAmmo = 30;
    [SerializeField] TMP_Text ammoDisplay;

    // Update is called once per frame
    void Update()
    {
        if(ammoDisplay != null)
        {
            ammoDisplay.text = currentAmmo.ToString();
        }
    }

    public static bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    public static void ConsumeAmmo(int amount = 1)
    {
        currentAmmo = Mathf.Max(0, currentAmmo - amount);
    }

    public static void Reload()
    {
        currentAmmo = maxAmmo;
    }
}
