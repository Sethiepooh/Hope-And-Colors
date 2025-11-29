using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AlixBoss : EnemyBase
{
    [Header("Attack Stats")]
    [SerializeField] float attackRange = 1.0f;
    [SerializeField] int damage = 5;
    [SerializeField] Transform attackPoint;
    int beatCount = 0;
    int barBeatCount = 0;
    int attackPhase = 0;
    bool section;
    bool slash = false;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] GameObject shockwavePrefab;
    [SerializeField] float spawnRange = 3.0f;
    [HideInInspector]public List<GameObject> activeShockwaves = new List<GameObject>();

    [Header("Movement Stats")]
    [SerializeField] float moveSpeed = 15.0f;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] RicochetPoint[] ricochetPoints;
    List<RicochetPoint> orderedRicochetPoints = new List<RicochetPoint>();
    List<Vector2> targetOrder = new List<Vector2>();
    int targetIndex = 0;

    EnemyManager enemyManager;
    [Header("Effects")]
    [SerializeField] Color attackColor;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(!section)
        {
            sRend.color = attackColor;
        }
        else
        {
            sRend.color = defaultColor;
        }
    }

    public override void Attack()
    {
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

    void GetRicochetPoints()
    {
        targetOrder.Clear();
        orderedRicochetPoints.Clear();
        for (int i = 0; i < ricochetPoints.Length; i++)
        {
            int index = Random.Range(0, ricochetPoints.Length);
            if(targetOrder.Contains(ricochetPoints[index].GetPosition()))
            {
                i--;
                continue;
            }
            targetOrder.Add(ricochetPoints[index].GetPosition());
            orderedRicochetPoints.Add(ricochetPoints[index]);
           // Debug.Log("Added Ricochet Point at: " + ricochetPoints[index].gameObject.name);
        }    
    }

    IEnumerator RicochetAttack()
    {
        slash = true;
        //Debug.Log("Ricochet Attack Initiated");
        if (targetIndex >= targetOrder.Count)
        {
            targetIndex = 0;
        }

        int currentIndex = targetIndex;
        targetIndex++;

        tRend.emitting = true;
        Vector2 direction = (targetOrder[currentIndex] - (Vector2)transform.position).normalized;
        orderedRicochetPoints[currentIndex].ActivatePoint();
        this.transform.position = Vector2.Lerp(this.transform.position, targetOrder[currentIndex], (60f / bpmInteract.GetBPM() * 2 ));


        slash = false;
        tRend.emitting = false;
        orderedRicochetPoints[currentIndex].DeactivatePoint();
        slash = false;
        yield return null;
    }

    IEnumerator DashTowardsPlayer()
    {
        Vector2 direction;
        Vector2 playerPos = player.transform.position;
        direction = (playerPos - rb.position).normalized;

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
        Vector2 spawnPos = (Vector2)this.transform.position + UnityEngine.Random.insideUnitCircle * spawnRange;
        GameObject shock = Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
        activeShockwaves.Add(shock);
    }

    public override void AddToBeatCount()
    {
        if (active && !section)
        {
            beatCount++;
            foreach (GameObject shock in activeShockwaves)
            {
                if (shock != null)
                {
                    shock.GetComponent<Shockwave>().AddToBeatCount();
                }
            }

            if(beatCount %2 == 0)
                SpawnShockwaveNearPlayer();
            StartCoroutine(DashTowardsPlayer());
        }
        if (active && section)
        {
            beatCount++;
            if (beatCount % 2 == 0)
                SpawnShockwaveNearBoss();
        }
    }

    public void AddToBarBeatCount()
    {
        if (active)
        {
            barBeatCount++;
            if (barBeatCount == 8)
            {
                section = !section;
            }
            
        }
    }
}
