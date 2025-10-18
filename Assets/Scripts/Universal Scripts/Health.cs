using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    [SerializeField] Slider healthBar;
    [SerializeField] bool isPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (isPlayer && healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (isPlayer && healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
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
