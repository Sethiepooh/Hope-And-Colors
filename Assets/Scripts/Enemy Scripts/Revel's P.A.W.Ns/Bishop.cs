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
        slowingFieldTelegraphEffect.transform.position = player.transform.position;
        slowingFieldTelegraphEffect.Play();
        yield return new WaitForSeconds(slowingFieldTelegraphEffect.duration);
        GameObject field = Instantiate(slowingFieldPrefab, player.transform.position, Quaternion.identity);
        field.GetComponent<SlowingField>().Initialize(PlayerMovement, slowingSpeed);
        StartCoroutine(RemoveSlowingField(field));
        // Implementation for spawning slowing field goes here
    }

    IEnumerator RemoveSlowingField(GameObject field)
    {
        yield return new WaitForSeconds(4f);
        Destroy(field);
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
