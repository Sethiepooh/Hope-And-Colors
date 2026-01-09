using UnityEngine;

public class GlitchShaman : EnemyBase
{
    [SerializeField] GameObject protectedEnemy;
    [SerializeField] GameObject shieldEffectPrefab;
    Health protectedEnemyHealth;
    GameObject shieldEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        protectedEnemyHealth = protectedEnemy.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!active) return;

        if(protectedEnemyHealth != null)
        {
            if(protectedEnemyHealth.damagable)
            {
                protectedEnemyHealth.damagable = false;
                shieldEffect = Instantiate(shieldEffectPrefab, protectedEnemy.transform.position, Quaternion.identity, protectedEnemy.transform);
                shieldEffect.transform.localScale = protectedEnemy.transform.localScale * 1.5f;
            }
        }
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
}
