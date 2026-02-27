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

    [Header("Enemy Spawn Stats")]
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] SpawnPoint[] enemySpawnPoints;

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
    [SerializeField] RollingCrystals rollingCrystalPrefab;
    [SerializeField] float rollingCrystalOffset;
    List<RollingCrystals> rollingCrystals = new List<RollingCrystals>();

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

    void SpawnFragileCrystal()
    {
        Instantiate(fragileCrystalPrefab, (Vector2)transform.position + Random.insideUnitCircle * crystalSpawnRadius, Quaternion.identity).Initialize(transform);
    }

    void SpawnRollingCrystal()
    {
        int randDir = Random.Range(0, 4);
        Vector2 spawnPos = Vector2.zero;
        Quaternion spawnRot = Quaternion.identity;
        switch (randDir)
        {
            case 0:
                spawnPos = Vector2.right;
                spawnRot = Quaternion.Euler(0, 0, -90);
                break;
            case 1:
                spawnPos = Vector2.up;
                spawnRot = Quaternion.Euler(0, 0, 180);
                break;
            case 2:
                spawnPos = Vector2.left;
                spawnRot = Quaternion.Euler(0, 0, 90);
                break;
            case 3:
                spawnPos = Vector2.down;
                break;
        }

        RollingCrystals roll = Instantiate(rollingCrystalPrefab, spawnPos, spawnRot).Initialize();
        rollingCrystals.Add(roll);
    }

    void TriggerRollingCrystals()
    {
        foreach (RollingCrystals roll in rollingCrystals)
        {
            roll.StartSliding();
        }
        rollingCrystals.Clear();
    }

    void SpawnFallingFist()
    {
        Instantiate(fallingFistPrefab, player.transform.position, Quaternion.identity).Initialize();
    }

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
