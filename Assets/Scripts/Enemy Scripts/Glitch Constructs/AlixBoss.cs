using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AlixBoss : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField]bool section;
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int damage = 5;
    [SerializeField] Transform attackPoint;
    int beatCount = 0;
    int barBeatCount = -1;
    bool slash = false;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectile;
    [SerializeField] ParticleSystem telegraphEffect;

    [Header("Shockwave Spawn Stats")]
    [SerializeField] GameObject shockwavePrefab;
    [SerializeField] float spawnRange = 3.0f;
    [HideInInspector]public List<GameObject> activeShockwaves = new List<GameObject>();

    [Header("Enemy Spawn Stats")]
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] SpawnPoint[] enemySpawnPoints;
    [SerializeField] Transform arenaCenter;

    [Header("Phase Management")]
    Health bossHealth;
    [SerializeField] int attackPhase = 0;


    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 15.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] LayerMask playerLayer;
  
    EnemyManager enemyManager;
    [Header("Effects")]
    [SerializeField] Color attackColor;
    [SerializeField] ParticleSystem chargeEffect;
    TrailRenderer tRend;
    Color defaultColor;
    SpriteRenderer sRend;
    BPMInteract bpmInteract;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        tRend = GetComponent<TrailRenderer>();
        tRend.emitting = false;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        bpmInteract = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<BPMInteract>();
        bossHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, offset);

        if(bpmInteract.GetCurrentSection() == 2 && !active)
        {
            attackPhase = 1;
            active = true;
            bossHealth.SetDamagable(true);
        }
        if (bossHealth.GetHealthPercent() <= .66f && attackPhase == 1)
        {
            attackPhase = 2;
            bpmInteract.currentMovement++;
        }
        else if (bossHealth.GetHealthPercent() <= .33f && attackPhase == 2)
        {
            attackPhase = 3;
            bpmInteract.currentMovement++;
        }



        if (!section)
        {
            sRend.color = attackColor;
        }
        else
        {
            sRend.color = defaultColor;
        }

        if (slash)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            if (hitPlayers.Length > 0)
            {
                if (hitPlayers[0].CompareTag("Player"))
                {
                    player.GetComponent<Health>().TakeDamage(damage);
                    Debug.Log("Player Hit!");
                    slash = false;
                }
            }
        }
    }

    public override void Attack()
    {
      
    }

    //void GetRicochetPoints()
    //{
    //    targetOrder.Clear();
    //    orderedRicochetPoints.Clear();
    //    for (int i = 0; i < ricochetPoints.Length; i++)
    //    {
    //        int index = Random.Range(0, ricochetPoints.Length);
    //        if(targetOrder.Contains(ricochetPoints[index].GetPosition()))
    //        {
    //            i--;
    //            continue;
    //        }
    //        targetOrder.Add(ricochetPoints[index].GetPosition());
    //        orderedRicochetPoints.Add(ricochetPoints[index]);
    //       // Debug.Log("Added Ricochet Point at: " + ricochetPoints[index].gameObject.name);
    //    }    
    //}

    //IEnumerator RicochetAttack()
    //{
    //    slash = true;
    //    //Debug.Log("Ricochet Attack Initiated");
    //    if (targetIndex >= targetOrder.Count)
    //    {
    //        targetIndex = 0;
    //    }

    //    int currentIndex = targetIndex;
    //    targetIndex++;

    //    tRend.emitting = true;
    //    Vector2 direction = (targetOrder[currentIndex] - (Vector2)transform.position).normalized;
    //    orderedRicochetPoints[currentIndex].ActivatePoint();
    //    this.transform.position = Vector2.Lerp(this.transform.position, targetOrder[currentIndex], (60f / bpmInteract.GetBPM() * 2 ));


    //    slash = false;
    //    tRend.emitting = false;
    //    orderedRicochetPoints[currentIndex].DeactivatePoint();
    //    slash = false;
    //    yield return null;
    //}

    void SpawnEnemies()
    {
        int randEnemy = Random.Range(0, enemyPrefabs.Length);
        int randPoint = Random.Range(0, enemySpawnPoints.Length);
        enemySpawnPoints[randPoint].PlayEffect();
        StartCoroutine(enemySpawnPoints[randPoint].SpawnEnemy(enemyPrefabs[randEnemy]));
    }

    void DestroyEnemies()
    {
        int enemiesDestroyed = 0;

        foreach (SpawnPoint spawnPoint in enemySpawnPoints)
        {
            if(enemiesDestroyed >= 3)
            {
                break;
            }
            if (spawnPoint.HasEnemy())
            {
                spawnPoint.DestroyEnemy();

            }
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        Vector2 direction;
        Vector2 playerPos = player.transform.position;
        direction = (playerPos - rb.position).normalized;
        slash = true;

        tRend.emitting = true;
        rb.linearVelocity = direction * moveSpeed;
        yield return new WaitForSeconds(dashDuration * 2);
        rb.linearVelocity = Vector2.zero;
        slash = false;
        tRend.emitting = false;
    }

    void SpawnShockwaveNearPlayer()
    {
        Vector2 spawnPos = (Vector2)player.transform.position + UnityEngine.Random.insideUnitCircle * spawnRange;
        GameObject shock =  Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
        activeShockwaves.Add(shock);
    }

    void SpawnShockwaveNearBoss()
    {
        chargeEffect.Play();
        Vector2 spawnPos = (Vector2)this.transform.position + UnityEngine.Random.insideUnitCircle * spawnRange / 1.5f;
        GameObject shock = Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
        activeShockwaves.Add(shock);
    }

    void FireProjectile()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        GameObject proj = Instantiate(projectile, projectileSpawn.position, Quaternion.identity);
        proj.GetComponent<Projectile>().direction = direction;
        proj.GetComponent<Projectile>().speed = 15;
    }

    public override void AddToBeatCount()
    {
        foreach (GameObject shock in activeShockwaves)
        {
            if (shock != null)
            {
                shock.GetComponent<Shockwave>().AddToBeatCount();
            }
        }

        if(attackPhase == 1)
        {
            PhaseOneAttackRotation();
        }
        else if (attackPhase == 2)
        {
            PhaseTwoAttackRotation();
        }
        else if (attackPhase == 3)
        {
            PhaseThreeAttackRotation();
        }
    }

    public void AddToBarBeatCount()
    {
        if (active && attackPhase > 0)
        {
            barBeatCount++;
            if (barBeatCount %8 == 0)
            {
                section = !section;
            }           
        }
    }

    void PhaseOneAttackRotation()
    {
        if (active && !section)
        {
            beatCount++;
            chargeEffect.Stop();
            StartCoroutine(DashTowardsPlayer());
        }
        if (active && section)
        {
            beatCount++;
            transform.position = arenaCenter.position;
            if (beatCount % 2 == 0)
                SpawnShockwaveNearBoss();
        }
    }

    void PhaseTwoAttackRotation()
    {
        if (active && !section)
        {
            beatCount++;
            chargeEffect.Stop();
            DestroyEnemies();

            if (beatCount % 2 == 0)
                SpawnShockwaveNearPlayer();

            StartCoroutine(DashTowardsPlayer());
        }
        if (active && section)
        {
            beatCount++;
            transform.position = arenaCenter.position;
            if (beatCount % 2 == 0)
                SpawnShockwaveNearBoss();
            if (beatCount % 20 == 0)
                SpawnEnemies();
        }
    }

    void PhaseThreeAttackRotation()
    {
        if (active && !section)
        {
            beatCount++;
            chargeEffect.Stop();
            if (beatCount % 2 == 0)
                SpawnShockwaveNearPlayer();
            StartCoroutine(DashTowardsPlayer());
        }
        if (active && section)
        {
            beatCount++;
            transform.position = arenaCenter.position;
            if (beatCount % 2 == 0)
                SpawnShockwaveNearBoss();
            if (beatCount % 8 == 0)
                SpawnEnemies();
            if (beatCount % 4 == 0)
                FireProjectile();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
