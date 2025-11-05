using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    [SerializeField] Slider healthBar;
    [SerializeField] bool isPlayer = false;
    [SerializeField] ParticleSystem deathParticles;

    [Header("Bomb settings")]
    [SerializeField] ParticleSystem blastParticles;
    public float blastRadius;
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
            if(sRend.color == Color.white)
            {
                StartCoroutine(HitFlash(Color.red));
            }
            else
            {
                StartCoroutine(HitFlash(Color.white));
            }

            if (isPlayer && healthBar != null)
            {
                healthBar.value = (float)currentHealth / maxHealth;
            }
            if (currentHealth <= 0)
            {
                if (gameObject.CompareTag("Bomb"))
                {
                    StartCoroutine(DeathBlast());
                }
                else
                {
                    deathParticles.startColor = defaultColor;
                    StartCoroutine(HandleDeath());
                }
                    
            }
        }      
    }

    public void SetDamagable(bool b)
    {
        damagable = b;
    }

    IEnumerator HitFlash(Color flashColor)
    {
        sRend.color = flashColor;
        yield return new WaitForSeconds(.1f);
        sRend.color = defaultColor;
    }

    IEnumerator DeathBlast()
    {
        blastParticles.Play();
        yield return new WaitForSeconds(2f);
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, blastRadius);
        foreach (Collider2D target in targets)
        {
            var health = target.GetComponent<Health>();
            health.TakeDamage(20);
        }
        var aIndicate = transform.GetChild(0).GetComponent<AttackIndicator>();
        sRend.enabled = false;
        blastParticles.Stop();
        aIndicate.AttackFlash();
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }

    IEnumerator HandleDeath()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            var enemyScript = gameObject.GetComponent<EnemyBase>();
            enemyScript.death = true;
            sRend.enabled = false;
            var col = gameObject.GetComponent<Collider2D>();
            col.enabled = false;
            deathParticles.Play();
        }
        else if (gameObject.CompareTag("Obstacle"))
        {
            sRend.enabled = false;
            var col = gameObject.GetComponent<Collider2D>();
            col.enabled = false;
            var rb = gameObject.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector3.zero;
            deathParticles.Play();
        }
        else
        {
            //handle player death
        }
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
