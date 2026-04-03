using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 90;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Give ammo
            AmmoController.AddAmmo(ammoAmount);

            // Destroy pickup
            Destroy(gameObject);
        }
    }
}