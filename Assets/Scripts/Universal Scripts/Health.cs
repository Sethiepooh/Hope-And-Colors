using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    [SerializeField] Slider healthBar;
    [SerializeField] bool isPlayer = false;
    SpriteRenderer sRend;
    Color defaultColor;
    bool damagable = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
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
        if (damagable)
        {
            currentHealth -= damage;
            StartCoroutine(HitFlash());
            if (isPlayer && healthBar != null)
            {
                healthBar.value = (float)currentHealth / maxHealth;
            }
            if (currentHealth <= 0)
            {
                Die();
            }
        }      
    }

    public void SetDamagable(bool b)
    {
        damagable = b;
    }

    IEnumerator HitFlash()
    {
        sRend.color = Color.white;
        yield return new WaitForSeconds(.1f);
        sRend.color = defaultColor;
    }

    void Die()
    {
        // Handle death (e.g., play animation, disable object, etc.)
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }
}
