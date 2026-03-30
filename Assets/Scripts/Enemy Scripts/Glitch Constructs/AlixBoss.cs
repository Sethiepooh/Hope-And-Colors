using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

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

    //Onslaught Settings
    [Header("Onslaught")]
    [SerializeField] ProjectilePool projectilePool;

    [Header("Scatter Onslaught Stats")]
    [SerializeField] int numberOfScatterProjectiles = 16;

    [Header("Single Slash Onslaught Stats")]
    [SerializeField] int numberOfProjectiles = 4;
    [SerializeField] float spreadAngle = 30f;

    [Header("Teleportation")]
    [SerializeField] Transform[] teleportPoints;
    [SerializeField] Transform arenaCenter;
    Transform lastPosition;

    [Header("Shockwave Spawn Stats")]
    [SerializeField] GameObject shockwavePrefab;
    [SerializeField] float spawnRange = 3.0f;
    [SerializeField] float nearSpawnRange = 1.0f;
    [HideInInspector]public List<GameObject> activeShockwaves = new List<GameObject>();

    [Header("Enemy Spawn Stats")]
    [SerializeField] GameObject[] enemyPrefabs; //Shaman - 0, Child - 1, Mother - 2, Father - 3
    [SerializeField] SpawnPoint[] enemySpawnPoints;

    [Header("Phase Management")]
    [SerializeField] int attackPhase = 0;
    [SerializeField] int attackType = 0;
    [SerializeField] int attacksTillChange = 4;
    Health bossHealth;
    int attacksDone = 0;


    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 15.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] LayerMask playerLayer;

    [Header("Dialogue")]
    [SerializeField] Dialogue dialogueScript;
    [SerializeField] InteractionManager interactManager;
  
    EnemyManager enemyManager;
    [Header("Effects")]
    [SerializeField] Color attackColor;
    [SerializeField] ParticleSystem chargeEffect;
    TrailRenderer tRend;
    Color defaultColor;
    public Color disabledColor = Color.purple;
    SpriteRenderer sRend;
    BPMInteract bpmInteract;
    public GameObject[] spikes;


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
        ChangeColor(disabledColor);
        transform.position = arenaCenter.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(bpmInteract.GetCurrentSection() == 2 && !active)
        {
            ChangeColor(defaultColor);
            attackPhase = 1;
            beatCount = 0;
            attacksDone = 0;
            StartCoroutine(PushPlayerAway());
            SpawnShamanDefense(1);
            active = true;
            bossHealth.SetDamagable(true);
        }

        if (bossHealth.GetHealthPercent() <= .66f && attackPhase == 1)
        {
            attackPhase = 2;
            attackType = 0;
            beatCount = 0;
            attacksDone = 0;
            StartCoroutine(PushPlayerAway());
            SpawnShamanDefense(2);
            bpmInteract.currentMovement++;
        }
        else if (bossHealth.GetHealthPercent() <= .33f && attackPhase == 2)
        {
            active = false;
            attackPhase = 3;
            StartCoroutine(PushPlayerAway());
            BeginPhaseThree();

        }


        if(attackPhase > 0)
        {
            if (!section)
            {
                ChangeColor(attackColor);
            }
            else
            {
               ChangeColor(defaultColor);
            }
        }
    }

    public void BeginPhaseThree()
    {
        active = true;
        attackType = 0;
        beatCount = 0;
        attacksDone = 0;
        SpawnShamanDefense(2);
        SpawnEnemies(1, 2);
        numberOfProjectiles++;
        bpmInteract.currentMovement++;
    }

    void TriggerDialogueBreak()
    {
        interactManager.ForceDialogueInteract(dialogueScript);
    }

    private void FixedUpdate()
    {
        if(attackPhase == 2)
            attackPoint.Rotate(new Vector3(0,0,3));
    }

    public void AddToAttacksDone()
    {
        if(!active)
            { return; }

       // Debug.Log("Attack Completed");
        attacksDone++;
        if (attacksDone >= attacksTillChange)
        {
            if (attackType >= 2)
            {
                attackType = 0;
            }
            else
            {
                attackType++;
            }
            attacksDone = 0;
            beatCount = 0;  
        }

        //Phase one actions
        if(bossHealth.GetHealthPercent() >= .66f)
        {
            if (attackPhase == 1 && attackType == 0 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                TeleportToCenter();
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(1);
            }

            if (attackPhase == 1 && attackType == 1 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                Teleport();
            }
        }


        //Phase Two actions
        if (bossHealth.GetHealthPercent() >= .33f)
        {
            if (attackPhase == 2 && attackType == 0 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                TeleportToCenter();
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(2);
            }

            if (attackPhase == 2 && attackType == 1 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                Teleport();
                SpawnEnemies(1, 2);
            }
        }


        //Phase 3 Actions
        if (bossHealth.GetHealthPercent() >= 0f)
        {
            if (attackPhase == 3 && attackType == 0 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                TeleportToCenter();
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(3);
                SpawnEnemies(1, 2);
            }

            if (attackPhase == 3 && attackType == 1 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                Teleport();
                SpawnEnemies(1, 2);
                SpawnEnemies(2, 1);
            }
        }          
    }

    public void ChangeColor(Color newColor)
    {
        sRend.color = newColor;

        foreach(GameObject spike in spikes)
        {
            spike.GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    public override void Attack()
    {
      
    }
    IEnumerator PushPlayerAway()
    {
        player.GetComponent<PlayerMovement>().controlable = false;
        player.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * 40, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
        //if(attackPhase == 3 && !active)
        //{
        //    TriggerDialogueBreak();
        //}
        player.GetComponent<PlayerMovement>().controlable = true;
    }

    #region Enemy Spawn Methods

    void SpawnEnemies(int spawnIndex, int enemyCount)
    {
        for(int i = 0; i < enemyCount; i++)
        {
            int randPoint = Random.Range(0, enemySpawnPoints.Length);

            if (!enemySpawnPoints[randPoint].HasEnemy())
            {
                enemySpawnPoints[randPoint].PlayEffect();
                StartCoroutine(enemySpawnPoints[randPoint].SpawnEnemy(enemyPrefabs[spawnIndex]));

            }
            else
            {
                i--;
                continue;
            }
           
        }        
    }

    void SpawnShamanDefense(int spawnNum)
    {

        List<GameObject> shamans = new List<GameObject>();
        for (int i = 0; i < spawnNum; i++)
        {
            int randIndex = Random.Range(0, enemySpawnPoints.Count());
            if (!enemySpawnPoints[randIndex].HasEnemy())
            {
                enemySpawnPoints[randIndex].PlayEffect();
                StartCoroutine(enemySpawnPoints[randIndex].SpawnAlixShaman(enemyPrefabs[0]));
                //shamans.Add(enemySpawnPoints[randIndex].currentEnemy);
            }
            else
            {
                i--;
                continue;
            }
            Debug.Log("Spawn Shaman");
        }
    }

    void DestroyEnemies()
    {
        foreach (SpawnPoint spawnPoint in enemySpawnPoints)
        {
            spawnPoint.DestroyEnemy();
        }
    }
    #endregion

    #region Shockwave Methods

    void SpawnShockwaveNearPlayer()
    {
        Vector2 spawnPos = (Vector2)player.transform.position + UnityEngine.Random.insideUnitCircle * nearSpawnRange;
        GameObject shock =  Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
        activeShockwaves.Add(shock);
    }

    void ShockwaveRampage()
    {
        for (int i = 0; i < 7; i++)
        {
            chargeEffect.Play();
            Vector2 spawnPos = (Vector2)this.transform.position + UnityEngine.Random.insideUnitCircle * spawnRange;
            GameObject shock = Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
            activeShockwaves.Add(shock);
        }
    }
    #endregion

    #region Onslaught Methods
    void FireScatterOnslaught()
    {

        float angleStep = 360f / numberOfScatterProjectiles;
        float angle = 0f;
        for (int i = 0; i < numberOfScatterProjectiles; i++)
        {
            float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;

            // Use the pool to get a projectile
            Projectile proj = projectilePool.GetProjectile(
                attackPoint.position + (projectileMoveDirection * 4),
                Quaternion.LookRotation(Vector3.forward, projectileMoveDirection)
            );
            proj.Initialize(projectilePool, false, projectileMoveDirection);

            angle += angleStep;
        }
    }

    void FireSingleSlashOnslaught()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 direction = (playerPos - (Vector2)transform.position).normalized;
        float angleStep = spreadAngle / (numberOfProjectiles - 1);

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = -spreadAngle / 2 + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 pelletDir = rotation * direction;

            // Use the pool to get a projectile
            Projectile pellet = projectilePool.GetProjectile(
                (Vector2)transform.position + (direction * 4),
                Quaternion.LookRotation(Vector3.forward, pelletDir)
            );
            pellet.Initialize(projectilePool, false, pelletDir.normalized);
        }
    }
    #endregion

    #region Teleportation Methods
    void Teleport()
    {
        int randPoint = Random.Range(0, teleportPoints.Length);
        if(teleportPoints[randPoint] == lastPosition)
        {
            randPoint++;
            if (randPoint >= teleportPoints.Length)
            {
                randPoint = 0;
            }
        }
        lastPosition = teleportPoints[randPoint];
        transform.position = teleportPoints[randPoint].position;
    }

    void TeleportToCenter()
    {
        transform.position = arenaCenter.position;
    }
    #endregion

    public override void AddToBeatCount()
    {
        if(!active)
            { return; }
        
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
               // section = !section;
            }           
        }
    }

    public void ReduceBarBeatCount()
    {
        barBeatCount--;
    }

    void PhaseOneAttackRotation()
    {
        beatCount++;

        switch(attackType)
        {
            case 0:
                if(beatCount % 4 == 0)
                {
                    chargeEffect.Stop();
                    TeleportToCenter();
                    FireScatterOnslaught();
                    Debug.Log("Phase One Attacks");
                }
                break;
            case 1:
                if(beatCount % 8 == 0)
                {
                    Teleport();
                    AddToAttacksDone();
                }
                if (beatCount % 4 == 0)
                {
                    FireSingleSlashOnslaught();
                }
                break;
            case 2:
                if (beatCount % 4 == 0)
                {
                    TeleportToCenter();
                    SpawnShockwaveNearPlayer();
                }
                break;
        }
    }

    void PhaseTwoAttackRotation()
    {
        beatCount++;
        switch (attackType)
        {
            case 0:
                if (beatCount % 4 == 0)
                {
                    chargeEffect.Stop();
                    TeleportToCenter();
                }
                if(beatCount % 2 == 0)
                {
                    FireScatterOnslaught();
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                    Teleport();
                    AddToAttacksDone();
                }
                if (beatCount % 4 == 0)
                {
                    FireSingleSlashOnslaught();
                }
                break;
            case 2:
                if (beatCount % 4 == 0)
                {
                    TeleportToCenter();
                    ShockwaveRampage();
                }
                break;
        }
    }

    void PhaseThreeAttackRotation()
    {
        beatCount++;
        switch (attackType)
        {
            case 0:
                if (beatCount % 4 == 0)
                {
                    chargeEffect.Stop();
                    TeleportToCenter();
                }
                if (beatCount % 2 == 0)
                {
                    FireScatterOnslaught();
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                    Teleport();
                    AddToAttacksDone();
                }
                if (beatCount % 4 == 0)
                {
                    FireSingleSlashOnslaught();
                }
                break;
            case 2:
                if (beatCount % 4 == 0)
                {
                    TeleportToCenter();
                    ShockwaveRampage();
                }
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
