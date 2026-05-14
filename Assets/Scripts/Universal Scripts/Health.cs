using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class Health : MonoBehaviour
{
    public Action onDeathEvent;
    public Action onDamageEvent;
    public Action onHealEvent;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth;
    [SerializeField] ParticleSystem deathParticles;

    SpriteRenderer sRend;
    Color defaultColor;

    public bool damagable = true;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        currentHealth = maxHealth;
        if(deathParticles != null)
            deathParticles.startColor = defaultColor;

    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        onHealEvent?.Invoke();
    }

    public void HealToMax()
    {
        currentHealth = maxHealth;
        onHealEvent?.Invoke();
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

            onDamageEvent?.Invoke();

            if (currentHealth <= 0)
            {
                onDeathEvent.Invoke();
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

    public void PlayDeathParticles()
    {
        if (deathParticles != null)
            deathParticles.Play();
    }

    #region DEATH METHODS

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
