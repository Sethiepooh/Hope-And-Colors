using UnityEngine;

public class GlitchShaman : EnemyBase, IProtector
{
    [SerializeField] GameObject shieldEffectPrefab;
    Health protectedEnemyHealth;
    GameObject shieldEffect;

    public EnemyBase protectedEnemyBase { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       health.onDeathEvent += DeactivateShaman;
    }

    // Update is called once per frame
    void Update()
    {
        if(!active) return;

        if(protectedEnemyHealth != null)
        {
            if(protectedEnemyHealth.damagable)
                protectedEnemyHealth.damagable = false;

            if(shieldEffect == null)
            {
                shieldEffect = Instantiate(shieldEffectPrefab, protectedEnemyBase.transform.position, Quaternion.identity, protectedEnemyBase.transform);
                shieldEffect.transform.localScale = protectedEnemyBase.transform.localScale * 1.5f;
            }
          
        }
    }

    public void SetProtectionTarget(GameObject enemy)
    {
        protectedEnemyBase = enemy.GetComponent<EnemyBase>();
        protectedEnemyHealth = protectedEnemyBase.GetComponent<Health>();
    }

    public void DeactivateShaman()
    {
        if (protectedEnemyHealth != null)
        {
            Destroy(shieldEffect);
            protectedEnemyHealth.damagable = true;
        }
    }

    public override void AddToBeatCount()
    {
        // No implementation needed for this enemy
    }

    public override void Attack()
    {
        // No implementation needed for this enemy
    }

    public void InitializeProteciton(EnemyBase e)
    {
        protectedEnemyBase = e;
        protectedEnemyHealth = protectedEnemyBase.gameObject.GetComponent<Health>();
    }
}
