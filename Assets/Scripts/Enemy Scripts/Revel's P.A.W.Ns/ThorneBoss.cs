using System.Collections;
using UnityEngine;

public class ThorneBoss : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] bool section;
    [SerializeField] ParticleSystem telegraphEffect;
    [SerializeField] ProjectilePool shatterSwordPool;

    [Header("Phase Management")]
    [SerializeField] int attackPhase = 0;
    [SerializeField] int attackType = 0;
    Health bossHealth;
    bool attacking = false;

    [Header("Orbiting Daggers")]
    [SerializeField] GameObject[] orbitingDaggers;

    [Header("Clockwork Swords")]
    [SerializeField] GameObject[] clockworkSwords;

    [Header("Hunting Daggers")]
    [SerializeField] GameObject[] huntingDaggers;
    int currentDaggerIndex = 0;

    [Header("Armament Rainfall")]
    [SerializeField] GameObject armamentRainfallPrefab;
    [SerializeField] float spawnRadius;
    [SerializeField] float spawnBuffer;
    [SerializeField] int armamentCountPerVolley;

    [Header("Projectile Lightswords")]
    [SerializeField] float scatterAngle;
    [SerializeField] int erraticBarrageProjectileCount;
    [SerializeField] float swordOffset;

    [Header("Greatsword")]
    [SerializeField] JadeMissile greatswordPrefab;
    [SerializeField] JadeMissile greatswordInstance;

    [Header("Melodium Crystal Spawn")]
    [SerializeField] Spawner melodiumSpawner;

    [Header("Dialogue")]
    [SerializeField] Dialogue dialogueScript;
    [SerializeField] InteractionManager interactManager;

    [Header("Effects")]
    [SerializeField] ParticleSystem chargeEffect;
    public Color disabledColor = Color.purple;
    BPMInteract bpmInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bpmInteract = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<BPMInteract>();
        ChangeColor(disabledColor);
        Initialize();
        DeactivateClockworkSwords();
        DeactivateOrbitingDaggers();
        foreach (GameObject dagger in huntingDaggers)
        {
            dagger.GetComponent<HuntingDaggers>().DeactivateDagger();
        }
    }

    void Initialize()
    {
        ChangeColor(defaultColor);
        attackPhase = 1;
        beatCount = 0;
        attackType = 0;
        active = true;
        attacking = true ;
        bossHealth.SetDamagable(true);
    }


    // Update is called once per frame
    void Update()
    {
        if (bossHealth.GetHealthPercent() <= .66f && attackPhase == 1)
        {
            DeactivateOrbitingDaggers();
            DeactivateDaggers();
            StartCoroutine(PhaseTransition());
            attackPhase = 2;
            attackType = 0;
            beatCount = 0;
            bpmInteract.currentMovement++;
        }
        else if (bossHealth.GetHealthPercent() <= .33f && attackPhase == 2)
        {
            DeactivateOrbitingDaggers();
            DeactivateDaggers();
            DeactivateClockworkSwords();
            StartCoroutine(PhaseTransition());
            attackPhase = 3;
            attackType = 0;
            beatCount = 0;
            bpmInteract.currentMovement++;
        }

        if (attackPhase > 0)
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


    #region Management
    public void ChangeColor(Color newColor)
    {
        sRend.color = newColor;
    }

    void TriggerDialogueBreak()
    {
        interactManager.ForceDialogueInteract(dialogueScript);
    }

    IEnumerator PushPlayerAway()
    {
        player.GetComponent<PlayerMovement>().controlable = false;
        player.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * 40, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
        player.GetComponent<PlayerMovement>().controlable = true;
    }
    #endregion

    //ORBITING DAGGERS
    void ActivateSingleOrbitingDagger(int i)
    {
        orbitingDaggers[i].SetActive(true);
        Rotator rot = orbitingDaggers[i].GetComponent<Rotator>();
        rot.ResetRotation();
        rot.RestartRotation();
    }

    void ActivateAllOrbitingDaggers()
    {
        for (int i = 0; i < orbitingDaggers.Length; i++)
        {
            ActivateSingleOrbitingDagger(i);
        }
    }

    void DeactivateOrbitingDaggers()
    {
        for (int i = 0; i < orbitingDaggers.Length; i++)
        {
            orbitingDaggers[i].SetActive(false);
        }
    }

    bool CheckIfOrbitingDaggersReady(int i)
    {
        if (orbitingDaggers[i].activeInHierarchy)
        {
            return false;
        }
        return true;
    }

    //CLOCKWORK SWORDS
    void ActivateSingleClockworkSword(int i)
    {
        clockworkSwords[i].SetActive(true);
        Rotator rot = clockworkSwords[i].GetComponent<Rotator>();
        rot.ResetRotation();
        rot.RestartRotation();
    }

    void ActivateAllClockworkSwords()
    {
        for (int i = 0; i < clockworkSwords.Length; i++)
        {
            ActivateSingleClockworkSword(i);
        }
    }

    void DeactivateClockworkSwords()
    {
        for (int i = 0; i < clockworkSwords.Length; i++)
        {
            clockworkSwords[i].SetActive(false);
        }
    }

    //HUNTING DAGGERS

    bool CheckIfDaggersReady()
    {
        foreach (GameObject dagger in huntingDaggers)
        {
            if (!dagger.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    void SummonHuntingDaggers()
    {
        foreach (GameObject dagger in huntingDaggers)
        {
            dagger.SetActive(true);
            dagger.GetComponent<HuntingDaggers>().Initialize();
        }
    }

    void TriggerDagger(int i)
    {
        huntingDaggers[i].GetComponent<HuntingDaggers>().DashTowardsPlayer();
    }

    void DeactivateDaggers()
    {
        foreach (GameObject dagger in huntingDaggers)
        {
            dagger.GetComponent<HuntingDaggers>().DeactivateDagger();
        }
    }

    //ARMAMENT RAINFALL
    void SummonArmament(Vector2 spawnPos)
    {
        ShatterSword sword = Instantiate(armamentRainfallPrefab, spawnPos, Quaternion.identity).GetComponent<ShatterSword>();
    }

    void SummonArmamentVolley(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            // Ensure spawnPos is outside the spawnBuffer range
            if (Vector2.Distance(spawnPos, transform.position) < spawnBuffer)
            {
                i--;
                continue;
            }
            if (CheckForObstruction(spawnPos))
            {
                i--;
                continue;
            }
            SummonArmament(spawnPos);
        }
    }

    bool CheckForObstruction(Vector2 spawnPoint)
    {
        RaycastHit2D hit = Physics2D.Raycast(spawnPoint, Vector2.down, 10f, LayerMask.GetMask("Obstacle"));
        return hit.collider != null;
    }

    //PROJECTILE LIGHTSWORDS
    void ErraticBarrage()
    {
        for (int i = 0; i < erraticBarrageProjectileCount; i++)
        {
            // Generate a random angle in degrees
            float angle = Random.Range(0f, 360f);
            // Convert angle to radians
            float rad = angle * Mathf.Deg2Rad;
            // Calculate direction vector
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

            Vector3 spawnPosition = transform.position + direction * swordOffset;

            Projectile projectileInstance = projectilePool.GetProjectile(
                spawnPosition,
                Quaternion.LookRotation(Vector3.forward, direction)
            );
            projectileInstance.Initialize(projectilePool, false, direction);
        }
    }

    protected virtual void FireScatterShot(int amount)
    {
        float angleStep = scatterAngle / amount;
        float angle = 0f;
        for (int i = 0; i < amount; i++)
        {
            float projectileDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);
            Vector3 projectileVector = new Vector3(projectileDirX, projectileDirY, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;

            Projectile projectileInstance = projectilePool.GetProjectile(
                transform.position,
                Quaternion.LookRotation(Vector3.forward, projectileVector)
            );
            projectileInstance.Initialize(projectilePool, false, projectileMoveDirection);

            angle += angleStep;
        }
    }

    void SummonLightsword()
    {
        Vector2 moveDir = player.transform.position - transform.position;
        Projectile projectileInstance = projectilePool.GetProjectile(
            transform.position,
            Quaternion.identity
        );
        projectileInstance.Initialize(projectilePool, false, moveDir);
    }

    //GREATSWORD
    void SpawnGreatsword()
    {
        JadeMissile missile = Instantiate(greatswordPrefab, transform.position, Quaternion.identity).Initialize(player, shatterSwordPool);
        greatswordInstance = missile;
    }

    void FireGreatsword()
    {
        if (greatswordInstance == null) return;
        greatswordInstance.Fire((player.transform.position - transform.position).normalized);
        greatswordInstance = null;
    }

    //CRYSTAl
    void SpawnMelodiumCrystal(int i)
    {
        for (int j = 0; j < i; j++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            // Ensure spawnPos is outside the spawnBuffer range
            if (Vector2.Distance(spawnPos, transform.position) < spawnBuffer)
            {
                j--;
                continue;
            }
            if (CheckForObstruction(spawnPos))
            {
                j--;
                continue;
            }
            Instantiate(melodiumSpawner, spawnPos, Quaternion.identity);
        }
    }

    public override void AddToBeatCount()
    {
        if (!active)
        { return; }

        if (!attacking) return;

        if (attackPhase == 1)
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


    void PhaseOneAttackRotation()
    {
        beatCount++;

        switch (attackType)
        {
            case 0:
                if(!CheckIfDaggersReady())
                {
                    SummonHuntingDaggers();
                }              
                if (beatCount % 2 == 0)
                {
                    FireScatterShot(10);
                    Debug.Log("Triggering Dagger " + currentDaggerIndex);
                    TriggerDagger(currentDaggerIndex);

                    if(currentDaggerIndex >= huntingDaggers.Length - 1)
                    {
                        currentDaggerIndex = 0;
                    }
                    else
                        currentDaggerIndex++;
                }

                if (beatCount % 32 == 0)
                {
                    DeactivateDaggers();
                    attackType++;
                }
                break;
            case 1:
                if(CheckIfOrbitingDaggersReady(0))
                {
                    ActivateSingleOrbitingDagger(0);
                    SummonArmamentVolley(armamentCountPerVolley);
                }
                if (beatCount % 32 == 0)
                {
                    DeactivateOrbitingDaggers();
                    attackType--;
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

                if (CheckIfOrbitingDaggersReady(0))
                {
                    ActivateAllOrbitingDaggers();
                }
                if (!CheckIfDaggersReady())
                {
                    SummonHuntingDaggers();
                }
                if (beatCount % 2 == 0)
                {
                    //Debug.Log("Triggering Dagger " + currentDaggerIndex);
                    TriggerDagger(currentDaggerIndex);

                    if (currentDaggerIndex >= huntingDaggers.Length - 1)
                    {
                        currentDaggerIndex = 0;
                    }
                    else
                        currentDaggerIndex++;
                }

                if (beatCount % 2 == 0)
                {
                    SummonArmament(player.transform.position);
                }
                if (beatCount % 32 == 0)
                {
                    DeactivateDaggers();
                    DeactivateOrbitingDaggers();
                    attackType++;
                }
                break;
            case 1:
                if (beatCount % 16 == 0)
                {
                    ActivateAllClockworkSwords();
                }
                if (beatCount % 64 == 0)
                {
                    DeactivateClockworkSwords();
                    attackType--;
                }
                break;
        }
    }

    void PhaseThreeAttackRotation()
    {
        beatCount++;

       
        if (CheckIfOrbitingDaggersReady(0))
        {
            ActivateAllOrbitingDaggers();
        }

        if (beatCount % 32 == 0)
        {
            SpawnMelodiumCrystal(1);
        }

        switch (attackType)
        {
            case 0:
                if (beatCount % 8 == 0)
                {
                    FireScatterShot(8);
                }

                if (beatCount % 4 == 0)
                {
                    if (greatswordInstance != null)
                        FireGreatsword();
                    else
                        SpawnGreatsword();
                }
                if (beatCount % 32 == 0)
                {
                    if (greatswordInstance != null)
                        FireGreatsword();
                    attackType++;
                }
                break;
            case 1:
                if (!CheckIfDaggersReady())
                {
                    SummonHuntingDaggers();
                }
                if (beatCount % 2 == 0)
                {
                    //Debug.Log("Triggering Dagger " + currentDaggerIndex);
                    TriggerDagger(currentDaggerIndex);

                    if (currentDaggerIndex >= huntingDaggers.Length - 1)
                    {
                        currentDaggerIndex = 0;
                    }
                    else
                        currentDaggerIndex++;
                }
                if (beatCount % 16 == 0)
                {
                    ActivateAllClockworkSwords();
                    SummonArmamentVolley(armamentCountPerVolley / 3);
                }
                if (beatCount % 64 == 0)
                {
                    DeactivateClockworkSwords();
                    DeactivateDaggers();
                    attackType--;
                }
                break;
        }
    }

    IEnumerator PhaseTransition()
    {
        attacking = false;
        StartCoroutine(PushPlayerAway());
        yield return new WaitForSeconds(1f);
        attacking = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnBuffer);
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
