using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle death (e.g., play animation, disable object, etc.)
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }
}
