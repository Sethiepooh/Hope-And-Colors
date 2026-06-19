using UnityEngine;
using UnityEngine.Events;

public abstract class BreakableObjectBase : MonoBehaviour
{
    public UnityEvent ManagerDeathEvent;
    Health health;

    [Header("VFX")]
    [SerializeField] protected ParticleSystem deathParticles;
    protected SpriteRenderer sRend;
    protected Color defaultColor;


    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        defaultColor = sRend.color;
        if (deathParticles != null)
            deathParticles.startColor = defaultColor;
        if (health != null)
            health.onDeathEvent += OnDeath;
    }

    public void Initialize()
    {

    }

    public abstract void OnDeath();
}
