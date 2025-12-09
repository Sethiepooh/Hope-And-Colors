using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    float currentHealth;
    [SerializeField] Slider healthBar;
    [SerializeField] bool isPlayer = false;
    [SerializeField] ParticleSystem deathParticles;

    [Header("Bomb settings")]
    [SerializeField] ParticleSystem blastParticles;
    public float blastRadius;
    SpriteRenderer sRend;
    Color defaultColor;
     public bool damagable = true;

    RespawnManager r_Man;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        currentHealth = maxHealth;
        deathParticles.startColor = defaultColor;
        r_Man = GameObject.FindWithTag("RespawnManager").GetComponent<RespawnManager>();
    }

    public void Heal(float heal)
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
                else if (gameObject.CompareTag("Player"))
                {
                    HandlePlayerDeath();
                }
                else
                {
                    StartCoroutine(HandleDeath());
                }
                    
            }
        }      
    }

    public void SetDamagable(bool b)
    {
        damagable = b;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
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

            if(health != null)
                health.TakeDamage(20);
        }
        var aIndicate = transform.GetChild(0).GetComponent<AttackIndicator>();
        sRend.enabled = false;
        blastParticles.Stop();
        aIndicate.AttackFlash();
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }

    void HandlePlayerDeath()
    {
        //handle player death
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector3.zero;
        deathParticles.Play();
        r_Man.ResetPlayer();
    }

    IEnumerator HandleDeath()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            var enemyScript = gameObject.GetComponent<EnemyBase>();
            enemyScript.active = false;
            sRend.enabled = false;
            var col = gameObject.GetComponent<Collider2D>();
            col.enabled = false;
            deathParticles.Play();
            if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                WaveSpawner waveSpawner = GameObject.FindFirstObjectByType<WaveSpawner>();
                waveSpawner.enemiesSpawnedInCurrentWave--;
            }
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
        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
