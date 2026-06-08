using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class AlixBoss : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField]bool section;
    [SerializeField] Transform attackPoint;

    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectile;
    [SerializeField] ParticleSystem telegraphEffect;

    [Header("Projectile Pools")]
    [SerializeField] ProjectilePool onslaughtProjectilePool;
    [SerializeField] ProjectilePool shockOrbProjectilePool;
    [SerializeField] ProjectilePool blastWaveProjectilePool;

    //Onslaught Settings
    [Header("Scatter Onslaught Stats")]
    [SerializeField] int numberOfScatterProjectiles = 16;

    [Header("Single Slash Onslaught Stats")]
    [SerializeField] int numberOfProjectiles = 4;
    [SerializeField] float spreadAngle = 30f;

    [Header("Teleportation")]
    List<Transform> teleportPoints = new List<Transform>();
    Vector3 arenaCenter;
    Transform lastPosition;

    [Header("Shockwave Spawn Stats")]
    [SerializeField] GameObject shockwavePrefab;
    [SerializeField] float spawnRange = 3.0f;
    [SerializeField] float nearSpawnRange = 1.0f;
    [HideInInspector]public List<GameObject> activeShockwaves = new List<GameObject>();

    [Header("Enemy Spawn Stats")]
    [SerializeField] List<SpawnPoint> enemySpawnPoints = new List<SpawnPoint>();

    [Header("Phase Management")]
    [SerializeField] int attackPhase = 0;
    [SerializeField] int attackType = 0;
    [SerializeField] int attacksTillChange = 4;
    int attacksTillNextPhase = 6;
    int storedAttacks = 0;
    int attacksDone = 0;

    BPMInteract bpmInteract;

    [Header("Dialogue Settings")]
    [SerializeField] UnityEvent[] cutscenes;

    [Header("Rhythm Minigame Settings")]
    [SerializeField] UnityEvent[] minigame;

    [Header("Effects")]
    [SerializeField] ParticleSystem chargeEffect;
    public Color disabledColor = Color.purple;
    public GameObject[] spikes;

    int delayBeats = 0;


    void Start()
    {
        bpmInteract = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<BPMInteract>();
        ChangeColor(disabledColor);
        foreach (SpawnPoint spawn in enemySpawnPoints)
        {
            //Debug.Log("Added Spawn Point " + spawn.transform);
            teleportPoints.Add(spawn.gameObject.transform);
        }
        GetAveragePosition(teleportPoints);
        transform.position = arenaCenter;
        health.onDamageEvent += AddToAttacksDone;
    }

    // Update is called once per frame
    void Update()
    {
        if (delayBeats > 0) return;

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

    public void TriggerNextPhase()
    {
        switch (attackPhase)
        {
            case 0:
                Debug.Log("Phase 1 Starting");
                attackPhase = 1;
                delayBeats = 32;
                bpmInteract.TriggerNextPhase();
                break;
            case 1:
                Debug.Log("Phase 2 Starting");
                active = true;

                health.SetDamagable(true);
                ChangeColor(defaultColor);
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(1);

                attackPhase = 2;
                beatCount = 0;
                attacksDone = 0;

                bpmInteract.TriggerNextPhase();
                break;
            case 2:
                Debug.Log("Phase 3 Starting");
                attackPhase = 3;
                delayBeats = 34;

                cutscenes[0].Invoke();
                StartCoroutine(PushPlayerAway());
                bpmInteract.TriggerNextPhase();
                break;
            case 3:
                Debug.Log("Phase 4 Starting");
                minigame[0].Invoke();
                StartCoroutine(PushPlayerAway());
                bpmInteract.TriggerNextPhase();
                break;
            case 4:
                Debug.Log("Phase 5 Starting");

                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(2);

                attackPhase = 5;
                beatCount = 0;
                attacksDone = 0;

                bpmInteract.TriggerNextPhase();
                break;
            case 5:
                Debug.Log("Phase 6 Starting");
                attackPhase = 6;
                delayBeats = 34;

                cutscenes[1].Invoke();
                StartCoroutine(PushPlayerAway());
                bpmInteract.TriggerNextPhase();
                break;
            case 6:
                Debug.Log("Phase 7 Starting");
                attackPhase = 7;
                minigame[1].Invoke();
                StartCoroutine(PushPlayerAway());
                bpmInteract.TriggerNextPhase();
                break;
            case 7:
                Debug.Log("Phase 8 Starting");
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(3);
                SpawnEnemies(1, 2);

                attackPhase = 8;
                beatCount = 0;
                attacksDone = 0;

                bpmInteract.TriggerNextPhase();
                break;
            case 8:
                Debug.Log("Phase 9 Starting");
                attackPhase = 9;
                cutscenes[2].Invoke();
                bpmInteract.TriggerNextPhase();
                break;
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
        bpmInteract.TriggerNextPhase();
    }

    private void FixedUpdate()
    {
        if(attackPhase == 2)
            attackPoint.Rotate(new Vector3(0,0,3));
    }

    public void AddToAttacksDone()
    {
        if(!active) return;

        attacksDone++;
        storedAttacks++;
        if(storedAttacks >= attacksTillNextPhase)
        {
            storedAttacks = 0;
            attacksDone = 0;
            TriggerNextPhase();
            return;
        }
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

        if(attackPhase == 2)
        {
            Debug.Log("Phase One Attack Type: " + attackType);
            if (attackType == 0 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                TeleportToCenter();
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(1);
            }

            if (attackType == 1 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                Teleport();
            }
        }

        if(attackPhase == 5)
        {
            if (attackType == 0 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                TeleportToCenter();
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(2);
            }

            if ( attackType == 1 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                Teleport();
                SpawnEnemies(1, 2);
            }
        }

        if(attackPhase == 8)
        {
            if (attackType == 0 && attacksDone == 0)
            {
                StopAllCoroutines();
                DestroyEnemies();
                TeleportToCenter();
                StartCoroutine(PushPlayerAway());
                SpawnShamanDefense(3);
                SpawnEnemies(1, 2);
            }

            if (attackType == 1 && attacksDone == 0)
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
        player.GetComponent<PlayerMovement>().controlable = true;
    }

    void GetAveragePosition(List<Transform> transforms)
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform t in transforms)
        {
            sum += t.position;
        }
        arenaCenter =  sum / transforms.Count;
    }

    #region Enemy Spawn Methods

    RoomEncounterManager.EnemySpawnConfig GenerateRandomEnemy(Transform spawnLocation)
    {
        EnemyType.ChosenEnemyType randomEnemyType = EnemyType.ChosenEnemyType.GlitchChild;
        int enemyIndex = Random.Range(0, 3);

        switch (enemyIndex)
        {
            case 0:
                randomEnemyType = EnemyType.ChosenEnemyType.GlitchChild;
                break;
            case 1:
                randomEnemyType = EnemyType.ChosenEnemyType.GlitchMother;
                break;
            case 2:
                randomEnemyType = EnemyType.ChosenEnemyType.GlitchFather;
                break;
        }

        return new RoomEncounterManager.EnemySpawnConfig(randomEnemyType, spawnLocation, false);
    }

    void SpawnEnemies(int spawnIndex, int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnPoint randomSpawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];

            if (!randomSpawnPoint.HasEnemy())
            {
                RoomEncounterManager.EnemySpawnConfig config = GenerateRandomEnemy(randomSpawnPoint.transform);
                randomSpawnPoint.PlayEffect();
                StartCoroutine(randomSpawnPoint.SpawnEnemy(config));
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
        for (int i = 0; i < spawnNum; i++)
        {
            SpawnPoint randomSpawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];

            if (!randomSpawnPoint.HasEnemy())
            {
                RoomEncounterManager.EnemySpawnConfig config = new RoomEncounterManager.EnemySpawnConfig(EnemyType.ChosenEnemyType.GlitchShaman, randomSpawnPoint.transform, false);
                randomSpawnPoint.PlayEffect();
                StartCoroutine(randomSpawnPoint.SpawnAlixShaman(config));
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
        int randPoint = Random.Range(0, teleportPoints.Count);
        if(teleportPoints[randPoint] == lastPosition)
        {
            randPoint++;
            if (randPoint >= teleportPoints.Count)
            {
                randPoint = 0;
            }
        }
        lastPosition = teleportPoints[randPoint];
        transform.position = teleportPoints[randPoint].position;
    }

    void TeleportToCenter()
    {
        transform.position = arenaCenter;
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

        if(delayBeats > 0)
        {
            if(attackPhase == 1)
            {
                if (bpmInteract.CheckIfMarkerPassed("Intro"))
                {
                    delayBeats--;
                    if (delayBeats == 0)
                    {
                        TriggerNextPhase();
                    }
                }
            }
            if(attackPhase == 3)
            {
                if (bpmInteract.CheckIfMarkerPassed("Dialogue 1"))
                {
                    delayBeats--;
                    if (delayBeats == 0)
                    {
                        TriggerNextPhase();
                    }
                }
            }
            
            return;
        }

        if (attackPhase == 2)
        {
            PhaseOneAttackRotation();
        }
        else if (attackPhase == 5)
        {
            PhaseTwoAttackRotation();

        }
        else if (attackPhase == 7)
        {
            PhaseThreeAttackRotation();
        }
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
                }
                break;
            case 1:
                if(beatCount % 8 == 0)
                {
                    Teleport();
                    AddToAttacksDone();
                }
                if (beatCount % 2 == 0)
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

    protected override IEnumerator EnemyDeath()
    {
        sRend.enabled = false;
        var col = gameObject.GetComponent<Collider2D>();
        col.enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector3.zero;
        health.PlayDeathParticles();
        NextLevel nextLevel = GameObject.FindFirstObjectByType<NextLevel>();
        nextLevel.LoadNextLevel();

        yield return new WaitForSeconds(.5f);
        this.gameObject.SetActive(false);
    }
}
