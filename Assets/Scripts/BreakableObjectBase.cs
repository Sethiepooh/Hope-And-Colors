using UnityEngine;
using UnityEngine.Events;

public abstract class BreakableObjectBase : MonoBehaviour
{
    public UnityEvent ManagerDeathEvent;

    [Header("VFX")]
    [SerializeField] protected ParticleSystem deathParticles;
    protected SpriteRenderer sRend;
    protected Color defaultColor;


    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        if (deathParticles != null)
            deathParticles.startColor = defaultColor;
    }

    public void Initialize()
    {

    }

    public abstract void OnDeath();
}
