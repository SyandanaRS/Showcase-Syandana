using UnityEngine;

public class HealthBox : MonoBehaviour
{
    public int healAmount = 50;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Give health
            PlayerHealth.Instance.Heal(healAmount);

            // Destroy pickup
            Destroy(gameObject);
        }
    }
}