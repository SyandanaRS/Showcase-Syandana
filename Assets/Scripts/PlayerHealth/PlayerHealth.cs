using UnityEngine;
using TMPro;
using System;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public int maxHealth = 100;
    public int currentHealth;
    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;
    public bool playerAlive = true;

    [SerializeField] TMP_Text healthDisplay;

    // Update is called once per frame
    void Update()
    {
        if(healthDisplay != null)
        {
            healthDisplay.text = currentHealth.ToString();
        }
    }

    void Awake()
    {
        Instance = this;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        OnPlayerDied?.Invoke();
        Debug.Log("Player Died");
        playerAlive = false;
    }
}
