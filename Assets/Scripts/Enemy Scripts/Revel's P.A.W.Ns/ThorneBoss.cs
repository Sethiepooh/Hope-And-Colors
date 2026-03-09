using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThorneBoss : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] bool section;
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int damage = 5;
    int beatCount = 0;
    int barBeatCount = -1;
    [SerializeField] ParticleSystem telegraphEffect;
    [SerializeField] GameObject player;
    [SerializeField] ProjectilePool projectilePool;

    [Header("Shield")]
    [SerializeField] GameObject shield;
    Color defaultShieldColor;
    bool isShielded = true;

    [Header("Phase Management")]
    [SerializeField] int attackPhase = 0;
    [SerializeField] int attackType = 0;
    [SerializeField] int attacksTillChange = 4;
    Health bossHealth;
    int attacksDone = 0;

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
    [SerializeField] int armamentCountPerVolley;

    [Header("Projectile Lightswords")]
    [SerializeField] int erraticBarrageProjectileCount;
    [SerializeField] protected float scatterAngle = 360f;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        bpmInteract = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<BPMInteract>();
        bossHealth = GetComponent<Health>();
        ChangeColor(disabledColor);
        Initialize();
    }

    void Initialize()
    {
        ChangeColor(defaultColor);
        attackPhase = 1;
        beatCount = 0;
        attackType = 0;
        active = true;
        bossHealth.SetDamagable(true);
    }


    // Update is called once per frame
    void Update()
    {
        if (bossHealth.GetHealthPercent() <= .66f && attackPhase == 1)
        {
            attackPhase = 2;
            attackType = 0;
            beatCount = 0;
            bpmInteract.currentMovement++;
        }
        else if (bossHealth.GetHealthPercent() <= .33f && attackPhase == 2)
        {
            active = false;
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
        if (attackPhase == 3 && !active)
        {
            TriggerDialogueBreak();
        }
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
    //ARMAMENT RAINFALL
    void SummonArmament(Vector2 spawnPos)
    {
        Instantiate(armamentRainfallPrefab, spawnPos, Quaternion.identity).GetComponent<ShatterSword>();
    }

    void SummonArmamentVolley(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRadius;
            SummonArmament(spawnPos);
        }
    }    

    //PROJECTILE LIGHTSWORDS
    void ErraticBarrage()
    {
        float angleStep = scatterAngle / erraticBarrageProjectileCount;
        float angle = 0f;
        for (int i = 0; i < erraticBarrageProjectileCount; i++)
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

    public override void AddToBeatCount()
    {
        if (!active)
        { return; }

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

    public void AddToBarBeatCount()
    {
        if (active && attackPhase > 0)
        {
            barBeatCount++;


        }
    }

    void PhaseOneAttackRotation()
    {
        beatCount++;       

        switch (attackType)
        {
            case 0:
                if (beatCount % 8 == 0)
                {
                   
                    attackType++;
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                   
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
                if (beatCount % 8 == 0)
                {
                   
                    attackType++;
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                   
                    attackType--;
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
                if (beatCount % 8 == 0)
                {
                   
                    attackType++;
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                   
                    attackType--;
                }
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {

    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
