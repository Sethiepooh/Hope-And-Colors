using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent onDeathEvent;
    public UnityEvent onDamageEvent;
    public float maxHealth = 100;
    float currentHealth;
    [SerializeField] Slider healthBar;
    [SerializeField] bool isPlayer = false;
    [SerializeField] bool isBoss = false;
    [SerializeField] ParticleSystem deathParticles;

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
        if(deathParticles != null)
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

    public void HealToMax()
    {
        currentHealth = maxHealth;
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

            if (isBoss || isPlayer)
            {
                onDamageEvent.Invoke();
            }

            if(isPlayer && !isBoss)
            {
                StartCoroutine(SetPlayerHitStun());
            }

            if (isPlayer && healthBar != null)
            {
                healthBar.value = (float)currentHealth / maxHealth;
            }

            if (currentHealth <= 0)
            {
                onDeathEvent.Invoke();
            }
        }      
    }

    IEnumerator SetPlayerHitStun()
    {
        damagable = false;
        yield return new WaitForSeconds(.2f);
        Debug.Log("Player can be damaged again");   
        damagable = true;
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

    #region DEATH METHODS

   

    public void DeactivateCollision()
    {
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void HandlePlayerDeath()
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


   

    public void HandleObstacleDeath()
    {
        StartCoroutine(DestroyObstacle());
    }

    IEnumerator DestroyObstacle()
    {
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
       // rb.linearVelocity = Vector3.zero;
       if(deathParticles != null)
            deathParticles.Play();

        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }

    public void HandleBossDeath()
    {
        StartCoroutine(BossDeath());
    }
    IEnumerator BossDeath()
    {
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector3.zero;
        deathParticles.Play();
        NextLevel nextLevel = GameObject.FindFirstObjectByType<NextLevel>();
        nextLevel.LoadNextLevel();

        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }

    public void HandleTurretBatteryDeath(Turret turret)
    {
        StartCoroutine(DestroyTurretBattery(turret));
    }

    IEnumerator DestroyTurretBattery(Turret turret)
    {
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        if(deathParticles != null)
            deathParticles.Play();
        turret.DeactivateTurret();

        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }
    #endregion

   
}
