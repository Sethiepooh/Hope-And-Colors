using System.Collections;
using UnityEngine;

public class Bishop : EnemyBase, IProtector
{
    [Header("Protected Enemy")]
    [SerializeField] GameObject protectedEnemy;
    [SerializeField] GameObject shieldEffectPrefab;

    [Header("Slowing Fields")]
    [SerializeField] GameObject slowingFieldPrefab;
    [SerializeField] ParticleSystem slowingFieldTelegraphEffect;
    [SerializeField] float slowingSpeed = 5f;

    PlayerMovement PlayerMovement;
    Health protectedEnemyHealth;
    public EnemyBase protectedEnemyBase { get; set; }
    GameObject shieldEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerMovement = player.GetComponent<PlayerMovement>();
        health.onDeathEvent += DeactivateBishop;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        if (protectedEnemyHealth != null)
        {
            if (protectedEnemyHealth.damagable)
            {
                protectedEnemyHealth.damagable = false;
                protectedEnemyBase.empowered = true;
                shieldEffect = Instantiate(shieldEffectPrefab, protectedEnemyBase.transform.position, Quaternion.identity, protectedEnemyBase.transform);
                shieldEffect.transform.localScale = protectedEnemyBase.transform.localScale * 1.5f;
            }
        }
    }

    public void DeactivateBishop()
    {
        if (protectedEnemyHealth != null)
        {
            Destroy(shieldEffect);
            protectedEnemyHealth.damagable = true;
            protectedEnemyBase.empowered = false;
        }
    }

    IEnumerator SpawnSlowingField()
    {
        Vector2 playerPos = player.transform.position;
        ParticleSystem effect = Instantiate(slowingFieldTelegraphEffect, playerPos, Quaternion.Euler(90,0,0));
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration);
        yield return new WaitForSeconds(effect.main.duration * .6f);
        Debug.Log("Spawning Slowing Field");
        GameObject field = Instantiate(slowingFieldPrefab, playerPos, Quaternion.identity);
        field.GetComponent<SlowingField>().Initialize(PlayerMovement, slowingSpeed);
        Destroy(field, 4f);
        // Implementation for spawning slowing field goes here
    }

    public override void AddToBeatCount()
    {
        beatCount++;
        if (beatCount % 4 == 0)
        {
            StartCoroutine(SpawnSlowingField());
            beatCount = 0;
        }
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
