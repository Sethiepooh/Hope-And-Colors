using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReginaldBoss : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] bool section;
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int damage = 5;
    [SerializeField] Transform attackPoint;
    int beatCount = 0;
    int barBeatCount = -1;
    [SerializeField] ParticleSystem telegraphEffect;
    [SerializeField] GameObject player;

    [Header("Phase Management")]
    [SerializeField] int attackPhase = 0;
    [SerializeField] int attackType = 0;
    [SerializeField] int attacksTillChange = 4;
    Health bossHealth;
    int attacksDone = 0;

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
    List<JadeMissile> jadeMissiles = new List<JadeMissile>();

    [Header("Drill Hazard Stats")]
    [SerializeField] FallingHazard drillHazardPrefab;
    [SerializeField] int drillsPerSalvo = 5;
    [SerializeField] float drillSpawnRadius;

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
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        bpmInteract = GameObject.FindGameObjectWithTag("RhythmManager").GetComponent<BPMInteract>();
        bossHealth = GetComponent<Health>();
        ChangeColor(disabledColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (bpmInteract.GetCurrentSection() == 2 && !active)
        {
            ChangeColor(defaultColor);
            attackPhase = 1;
            beatCount = 0;
            attacksDone = 0;
            active = true;
            bossHealth.SetDamagable(true);
        }

        if (bossHealth.GetHealthPercent() <= .66f && attackPhase == 1)
        {
            attackPhase = 2;
            attackType = 0;
            beatCount = 0;
            attacksDone = 0;
            bpmInteract.currentMovement++;
        }
        else if (bossHealth.GetHealthPercent() <= .33f && attackPhase == 2)
        {
            active = false;
            attackPhase = 3;
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
    public void AddToAttacksDone()
    {
        if (!active)
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
        if (bossHealth.GetHealthPercent() >= .66f)
        {
            if (attackPhase == 1 && attackType == 0 && attacksDone == 0)
            {
                StopAllCoroutines();
                StartCoroutine(PushPlayerAway());
            }

            if (attackPhase == 1 && attackType == 1 && attacksDone == 0)
            {
                StopAllCoroutines();
            }
        }
    }

    public void ChangeColor(Color newColor)
    {
        sRend.color = newColor;

        foreach (GameObject spike in spikes)
        {
            spike.GetComponent<SpriteRenderer>().color = newColor;
        }
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

    //FRAGILE CRYSTAL
    void SpawnFragileCrystal()
    {
        Instantiate(fragileCrystalPrefab, (Vector2)transform.position + Random.insideUnitCircle * crystalSpawnRadius, Quaternion.identity).Initialize(transform);
    }

    //ROLLING CRYSTAL
    void SpawnRollingCrystal()
    {
        int randDir = Random.Range(0, 4);
        rollingCrystals[randDir].gameObject.SetActive(true);
        rollingCrystals[randDir].Initialize();
    }

    void TriggerRollingCrystals()
    {
        foreach (RollingCrystals roll in rollingCrystals)
        {
            if(roll.gameObject.activeSelf)
                roll.StartSliding();
        }
        rollingCrystals.Clear();
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
            JadeMissile missile = Instantiate(jadeMissilePrefab, transform.position, Quaternion.identity).Initialize(player);
            jadeMissiles.Add(missile);
        }
    }

    void FireJadeMissile(int i)
    {
        jadeMissiles[i].Fire((player.transform.position - transform.position).normalized);
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
            if (barBeatCount % 8 == 0)
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

        switch (attackType)
        {
            
        }
    }

    void PhaseTwoAttackRotation()
    {
        beatCount++;
        switch (attackType)
        {
           
        }
    }

    void PhaseThreeAttackRotation()
    {
        beatCount++;
        switch (attackType)
        {
            
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
