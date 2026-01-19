using System.Collections;
using UnityEngine;

public class Bishop : EnemyBase
{
    [Header("Protected Enemy")]
    [SerializeField] GameObject protectedEnemy;
    [SerializeField] GameObject shieldEffectPrefab;

    [Header("Slowing Fields")]
    [SerializeField] GameObject slowingFieldPrefab;
    [SerializeField] ParticleSystem slowingFieldTelegraphEffect;
    [SerializeField] float slowingSpeed = 5f;

    GameObject player;
    PlayerMovement PlayerMovement;
    Health protectedEnemyHealth;
    EnemyBase protectedEnemyBase;
    GameObject shieldEffect;

    int beatCount;  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        PlayerMovement = player.GetComponent<PlayerMovement>();
        protectedEnemyHealth = protectedEnemy.GetComponent<Health>();
        protectedEnemyBase = protectedEnemy.GetComponent<EnemyBase>();
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
                shieldEffect = Instantiate(shieldEffectPrefab, protectedEnemy.transform.position, Quaternion.identity, protectedEnemy.transform);
                shieldEffect.transform.localScale = protectedEnemy.transform.localScale * 1.5f;
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
}
