using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class EnemyBase : MonoBehaviour
{
    public Action ManagerDeathEvent;

    public bool active = false;
    public bool empowered = false;

    public abstract void Attack();
    public abstract void AddToBeatCount();

    protected GameObject player;
    protected PulseManager pulseManager;
    protected ProjectilePool projectilePool;
    protected Health health;
    protected RoomEncounterManager roomEncounterManager;

    [Header("VFX")]
    [SerializeField] protected Color attackColor;
    [SerializeField] ParticleSystem deathParticles;
    protected TrailRenderer tRend;
    protected SpriteRenderer sRend;
    protected Color defaultColor;

    protected Rigidbody2D rb;

    protected int beatCount = 0;

    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;

        if (deathParticles != null)
            deathParticles.startColor = defaultColor;

        tRend = GetComponent<TrailRenderer>();

        if(tRend != null)
            tRend.emitting = false;

        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    public void Initialize(GameObject player, PulseManager pMan, ProjectilePool pool, RoomEncounterManager eMan, bool activeState)
    {
        roomEncounterManager = eMan;
        this.player = player;
        this.pulseManager = pMan;
        pulseManager.AddEntity(this.gameObject, pulseManager.entitiesToPulse);
        this.projectilePool = pool;
        active = activeState;
        if(health != null)
            health.onDeathEvent += OnDeath;
    }

    public void ResetEnemy()
    {
        if (health != null)
            health.HealToMax();
        beatCount = 0;
        sRend.color = defaultColor;

        if(tRend != null)
            tRend.emitting = false;
    }

    public void ObliterateEnemy()
    {
        if (health != null)
            health.TakeDamage(int.MaxValue);
    }

    public void OnDeath()
    {
        StartCoroutine(EnemyDeath());
        ManagerDeathEvent.Invoke();
    }
   
    protected virtual IEnumerator EnemyDeath()
    {
        active = false;
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        if(deathParticles != null)
            deathParticles.Play();

        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }

    public void SetIsActive(bool state)
    {
        active = state;
    }

    IEnumerator ResetActive(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        active = true;
    }

    public void ResetActiveForEnemy(float sec)
    {
        StartCoroutine(ResetActive(sec));
    }
}
