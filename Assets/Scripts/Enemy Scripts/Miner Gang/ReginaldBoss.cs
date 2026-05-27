using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReginaldBoss : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] bool section;
    [SerializeField] ParticleSystem telegraphEffect;

    [Header("Shield")]
    [SerializeField] GameObject shield;
    Color defaultShieldColor;
    bool isShielded = true;

    [Header("Phase Management")]
    [SerializeField] int attackPhase = 0;
    [SerializeField] int attackType = 0;
    Health bossHealth;

    [Header("Fragile Crystal Stats")]
    [SerializeField] FragileCrystal fragileCrystalPrefab;
    [SerializeField] float crystalSpawnRadius;

    [Header("Rolling Crystal Stats")]
    [SerializeField] List<RollingCrystals> rollingCrystals = new List<RollingCrystals>();

    [Header("Falling Fist Stats")]
    [SerializeField] FallingHazard fallingFistPrefab;

    [Header("Jade Missile Stats")]
    [SerializeField] JadeMissile jadeMissilePrefab;
    [SerializeField] int missilesPerSalvo = 5;
    Stack<JadeMissile> jadeMissiles = new Stack<JadeMissile>();

    [Header("Drill Hazard Stats")]
    [SerializeField] FallingHazard drillHazardPrefab;
    [SerializeField] int drillsPerSalvo = 5;
    [SerializeField] float drillSpawnRadius;

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
    }

    void Initialize()
    {
        ChangeColor(defaultColor);
        attackPhase = 1;
        beatCount = 0;
        attackType = 0;
        active = true;
        bossHealth.SetDamagable(true);
        EnableShield();
    }


    // Update is called once per frame
    void Update()
    {
        if (bossHealth.GetHealthPercent() <= .66f && attackPhase == 1)
        {
            EnableShield();
            attackPhase = 2;
            attackType = 0;
            beatCount = 0;
            bpmInteract.QueueTransitionToNextSection();
            missilesPerSalvo += 1;
        }
        else if (bossHealth.GetHealthPercent() <= .33f && attackPhase == 2)
        {
            EnableShield();
            active = false;
            attackPhase = 3;
            attackType = 0;
            beatCount = 0;
            bpmInteract.QueueTransitionToNextSection();
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
        //interactManager.ForceDialogueInteract(dialogueScript);
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

    //FRAGILE CRYSTAL
    void SpawnFragileCrystal()
    {
        Instantiate(fragileCrystalPrefab, (Vector2)transform.position + Random.insideUnitCircle * crystalSpawnRadius, Quaternion.identity).Initialize(transform, projectilePool);
    }

    //ROLLING CRYSTAL
    void SpawnRollingCrystal()
    {
        int randDir = Random.Range(0, 4);
        if (rollingCrystals[randDir].initialized)
        {
           SpawnRollingCrystal();
           return;
        }
        rollingCrystals[randDir].Initialize();
    }

    void TriggerRollingCrystals()
    {
        foreach (RollingCrystals roll in rollingCrystals)
        {
            if (!roll.sliding)
            {
                roll.StartSliding();
            }
        }
    }

    //FALLING FIST
    void SpawnFallingFist()
    {
        Instantiate(fallingFistPrefab, player.transform.position, Quaternion.identity).Initialize();
    }

    // JADE MISSILE
    void SpawnJadeMissile()
    {
        for (int i = 0; i < missilesPerSalvo; i++)
        {
            JadeMissile missile = Instantiate(jadeMissilePrefab, transform.position, Quaternion.identity).Initialize(player, projectilePool);
            jadeMissiles.Push(missile);
        }
    }

    void FireJadeMissile()
    {
        JadeMissile missile = jadeMissiles.Pop();
        missile.Fire((player.transform.position - transform.position).normalized);
    }

    // DRILL HAZARD
    void FireDrillSalvo()
    {
        for (int i = 0; i < drillsPerSalvo; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * drillSpawnRadius;
            Instantiate(drillHazardPrefab, spawnPos, Quaternion.identity).Initialize();
        }
    }

    //SHIELD
    void EnableShield()
    {
        isShielded = true;
        shield.SetActive(true);
        shield.GetComponent<SpriteRenderer>().color = defaultShieldColor;
        shield.GetComponent<Health>().Heal(100);
        bossHealth.SetDamagable(false);
    }

    public void DisableShield()
    {
        isShielded = false;
        shield.SetActive(false);
        bossHealth.SetDamagable(true);
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

    void PhaseOneAttackRotation()
    {
        beatCount++;

        if (jadeMissiles.Count > 0)
        {
            FireJadeMissile();
        }

        TriggerRollingCrystals();

        switch (attackType)
        {
            case 0:
                if (beatCount % 8 == 0)
                {
                    SpawnFragileCrystal();
                    SpawnJadeMissile();
                    attackType++;
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                    SpawnFragileCrystal();
                    SpawnRollingCrystal();
                    attackType--;
                }
                break;
        }
    }

    void PhaseTwoAttackRotation()
    {
        beatCount++;

        if (jadeMissiles.Count > 0)
        {
            FireJadeMissile();
        }

        TriggerRollingCrystals();

        switch (attackType)
        {
            case 0:
                if (beatCount % 8 == 0)
                {
                    SpawnFragileCrystal();
                    SpawnFragileCrystal();  
                    SpawnJadeMissile();
                    FireDrillSalvo();
                    attackType++;
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                    SpawnFragileCrystal();
                    SpawnRollingCrystal();
                    SpawnRollingCrystal();
                    attackType--;
                }
                break;
        }
    }

    void PhaseThreeAttackRotation()
    {
        beatCount++;

        if (jadeMissiles.Count > 0)
        {
            FireJadeMissile();
        }

        TriggerRollingCrystals();

        switch (attackType)
        {
            case 0:
                if (beatCount % 8 == 0)
                {
                    SpawnFragileCrystal();
                    SpawnFragileCrystal();
                    SpawnJadeMissile();
                    FireDrillSalvo();
                    attackType++;
                }
                break;
            case 1:
                if (beatCount % 8 == 0)
                {
                    SpawnFragileCrystal();
                    SpawnRollingCrystal();
                    SpawnRollingCrystal();
                    SpawnFallingFist();
                    attackType--;
                }
                break;
        }
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
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
