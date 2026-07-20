using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    PlayerMovement p_Mov;
    BPMInteract bpmInteract;
    EnemyManager enemyManager;

    //Combo system variables
    int comboStep = 0;
    int maxComboStep = 4;

    [Header("Damage Stats")]
    //Damage Stats
    [SerializeField] int baseDamage = 10;
    [SerializeField] int damageIncreasePerCombo = 2;
    //Perfect Damage Stats
    [SerializeField] int perfectDamageIncreasePerCombo = 4;
    //Inspiration DMG Stats
    [SerializeField] int baseInspirationDamage = 10;
    [SerializeField] int inspirationDamagePerCombo = 5;
    [SerializeField] int inspirationPerfectDamagePerCombo = 8;
    [SerializeField] Transform facedDirection;
    [SerializeField] bool canAttack = true;
    bool stumble;
    public LayerMask enemyLayer;
    [SerializeField] float strikeForce;
    float attackRange;
    int currentDamage;

    [Header("Inspiration Stats")]
    [SerializeField] float inspirationGainOnBeat = 5f;
    [SerializeField] float inspirationGainOffBeat = 1f;
    [SerializeField] float maxInspiration = 100f;
    [SerializeField] float inspirationConsumptionRate = 5f;
    [SerializeField] Image[] inspirationBars;
    float inspirationGainOnHit;
     public float currentInspiration = 0f;
    PlayerMovement playerMovement;
    public float healthRegenRate;

    [Header("Heartthrob's Solo Stats")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawnPoint;
    bool soloActive;
    bool comboButton;
    public SonicEruption sErupt;

    [Header("Angel Break Stats")]
    public float angelBreakTime;

    [Header("Progression")]
    public bool hasHeartthrobsSolo;
    public bool hasAngelBreak;
    public bool hasStageDive;


    [Header("Effects")]
    [SerializeField]Color allegroColor;
    [SerializeField] Color stumbleColor;
    SpriteRenderer sRend;
    Color defaultColor;
    TrailRenderer trail;
    [SerializeField] GameObject attackIndicator;
    [SerializeField] GameObject criticalIndicator;
    AttackIndicator cIndicator;
    AttackIndicator aIndicator;

    bool allegroMode;



    void Awake()
    {
        cIndicator = criticalIndicator.GetComponent<AttackIndicator>();
        aIndicator = attackIndicator.GetComponent<AttackIndicator>();
        sRend = GetComponent<SpriteRenderer>();
        defaultColor = sRend.color;
        trail = GetComponent<TrailRenderer>();
        p_Mov = GetComponent<PlayerMovement>();
        currentDamage = baseDamage;
        attackRange = Vector2.Distance(transform.position, facedDirection.position);
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        bpmInteract = GameObject.Find("Rhythm Manager").GetComponent<BPMInteract>();
        enemyManager = GameObject.FindWithTag("EnemyManager")?.GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(p_Mov.GetMovement() != null)
        {
            facedDirection.transform.position = new Vector2(transform.position.x + (p_Mov.GetMovementLateral().x * 1.5f), transform.position.y + (p_Mov.GetMovementLateral().y * 1.5f));
            attackIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, p_Mov.GetMovementLateral());
            criticalIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, p_Mov.GetMovementLateral());
        }
            
        if (allegroMode)
        {
            GetComponent<Health>().Heal(healthRegenRate * Time.deltaTime);
            if(!stumble)
                sRend.color = allegroColor;
            currentInspiration -= inspirationConsumptionRate * Time.deltaTime;
            UpdateInspirationUI();
            if (currentInspiration / maxInspiration <= 0)
            {
                sRend.color = defaultColor;
                UpdateInspirationUI();
                allegroMode = false;
            }
        }
        else
        {
            if(!stumble)
                sRend.color = defaultColor;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!canAttack || soloActive)
            return;

        if (context.performed)
        {         
            if (bpmInteract.CheckInput(false) == 0)
            {
                cIndicator.AttackFlash();
                inspirationGainOnHit = inspirationGainOnBeat;
                if(comboStep < maxComboStep)
                {
                    comboStep++;
                    if (allegroMode)
                        currentDamage += inspirationPerfectDamagePerCombo;
                    else
                        currentDamage += perfectDamageIncreasePerCombo;
                }
                else
                {
                    comboStep = 1;
                    if (allegroMode)
                        currentDamage = baseInspirationDamage;
                    else
                        currentDamage = baseDamage;
                }
            }
            else if (bpmInteract.CheckInput(false) == 1)
            {
                aIndicator.AttackFlash();
                inspirationGainOnHit = inspirationGainOnBeat;
                if (comboStep < maxComboStep)
                {
                    comboStep++;
                    if (allegroMode)
                        currentDamage += inspirationDamagePerCombo;
                    else
                        currentDamage += damageIncreasePerCombo;
                }
                else
                {
                    comboStep = 1;
                    if (allegroMode)
                        currentDamage = baseInspirationDamage;
                    else
                        currentDamage = baseDamage;
                }
            }
            else if(bpmInteract.CheckInput(false) == 2)
            {
                Debug.Log("Missed Attack");
                StopCoroutine(AttackCooldown());    
                StartCoroutine(AttackCooldown());
                inspirationGainOnHit = inspirationGainOffBeat;
                currentDamage = baseDamage;
                comboStep = 0;
               return;
            }

            // Detect enemies in range of attack
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.gameObject.CompareTag("Enemy") || enemy.gameObject.CompareTag("Boss"))
                {
                    Debug.Log("Enemy Hit Detected");
                    // Check if enemy is in front of player
                    Vector2 relativePos = enemy.transform.position - transform.position;
                    Vector2 forward = (Vector2)facedDirection.position - (Vector2)transform.position;
                    float angle = Vector3.Angle(relativePos, forward);
                    if (angle < 90f)
                    {
                        //Apply damage to enemy & add inspiration to Player
                        Debug.Log("Hit " + enemy.name + " for " + currentDamage + " damage!");
                        Health hp = enemy.gameObject.GetComponent<Health>();
                        hp.TakeDamage(currentDamage);
                        if(hp.damagable)
                            currentInspiration += inspirationGainOnHit;
                        if (currentInspiration > maxInspiration)
                            currentInspiration = maxInspiration;
                        UpdateInspirationUI();
                    }
                }
                else if (enemy.gameObject.CompareTag("Obstacle") || enemy.gameObject.CompareTag("Bomb"))
                {
                    // Check if enemy is in front of player
                    Vector2 relativePos = enemy.transform.position - transform.position;
                    Vector2 forward = (Vector2)facedDirection.position - (Vector2)transform.position;
                    float angle = Vector3.Angle(relativePos, forward);
                    if (angle < 90f)
                    {
                        enemy.gameObject.GetComponent<Health>().TakeDamage(currentDamage);
                    }
                }
            }
            canAttack = false;
        }
    }

    public void UpdateInspirationUI()
    {
        float inspirationRatio = currentInspiration / maxInspiration;

        if(inspirationRatio <= .25)
        {
            inspirationBars[0].fillAmount = inspirationRatio * 4;
            inspirationBars[1].fillAmount = 0;
            inspirationBars[2].fillAmount = 0;
            inspirationBars[3].fillAmount = 0;
            if(inspirationRatio < 0)
            {
                inspirationBars[0].fillAmount = 0;
            }
        }
        else if(inspirationRatio > .25f && inspirationRatio <= .50f)
        {
            inspirationBars[0].fillAmount = 1;
            inspirationBars[1].fillAmount = (inspirationRatio - .25f) * 4;
            inspirationBars[2].fillAmount = 0;
            inspirationBars[3].fillAmount = 0;
        }
        else if(inspirationRatio > .50f && inspirationRatio <= .75f)
        {
            inspirationBars[0].fillAmount = 1;
            inspirationBars[1].fillAmount = 1;
            inspirationBars[2].fillAmount = (inspirationRatio - .50f) * 4;
            inspirationBars[3].fillAmount = 0;
        }
        else if(inspirationRatio >= .75f)
        {
            inspirationBars[0].fillAmount = 1;
            inspirationBars[1].fillAmount = 1;
            inspirationBars[2].fillAmount = 1;
            inspirationBars[3].fillAmount = (inspirationRatio - .75f) * 4;
        }
    }

    public void AddToCurrentInspiration(float inspiration)
    {
        currentInspiration += inspiration;
        if (currentInspiration > maxInspiration)
            currentInspiration = maxInspiration;
        UpdateInspirationUI();
        Debug.Log("Current Inspiration: " + currentInspiration);
    }

    public void SetCanAttack(bool b)
    {
        if(stumble)
            return;
        canAttack = b;
    }

    IEnumerator AttackCooldown()
    {
        sRend.color = stumbleColor;
        stumble = true;
        canAttack = false;
        yield return new WaitForSeconds(1f);
        stumble = false;
        canAttack = true;
        sRend.color = defaultColor;
    }

    public void Allegro(InputAction.CallbackContext context)
    {
        if(stumble || comboButton)
            return;

        if (context.performed)
        {
            if (currentInspiration / maxInspiration >= .25f && !allegroMode)
            {
                allegroMode = true;
                playerMovement.allegro = true;
            }
            else if (allegroMode)
            {
                allegroMode = false;
                playerMovement.allegro = false;
            }           
        }
    }

    //IEnumerator ResetAngelBreak()
    //{

    //}

    public void AngelBreak(InputAction.CallbackContext context)
    {
        if (!comboButton || !hasAngelBreak)
            return;

        if (currentInspiration >= maxInspiration)
        {
            if (context.performed)
            {
                StartCoroutine(AngelBreakTimer());
                currentInspiration -= maxInspiration;
                UpdateInspirationUI();
            }
        }
       
    }

    IEnumerator AngelBreakTimer()
    {
        Projectile[] activeProjectiles = FindObjectsOfType<Projectile>();
        ThorneBoss thorneBoss = FindObjectOfType<ThorneBoss>();
        Rotator[] activeRotators = FindObjectsOfType<Rotator>();
        ShatterSword[] activeShatterSwords = FindObjectsOfType<ShatterSword>();

        if (thorneBoss != null)
            thorneBoss.active = false;

        enemyManager.angelBreak = true;

        foreach (ShatterSword shatterSword in activeShatterSwords)
        {
            shatterSword.active = false;
        }

        foreach (Rotator rotator in activeRotators)
        {
            rotator.isRotating = false;
        }

        foreach (Projectile projectile in activeProjectiles)
        {
            if (!projectile.fireFromPlayer)
            {
                projectile.ToggleFreeze(true);
            }
        }
        Debug.Log("Angel Break State: " + enemyManager.angelBreak);
        
        yield return new WaitForSeconds(angelBreakTime);

        foreach (ShatterSword shatterSword in activeShatterSwords)
        {
            shatterSword.active = true;
        }

        foreach (Projectile projectile in activeProjectiles)
        {
            if (!projectile.fireFromPlayer)
            {
                projectile.ToggleFreeze(false);
            }
        }

        foreach (Rotator rotator in activeRotators)
        {
            rotator.isRotating = true;
        }

        if (thorneBoss != null)
            thorneBoss.active = true;
        enemyManager.angelBreak = false;
    }

    public void HeartthrobsSolo(InputAction.CallbackContext context)
    {
        if(soloActive || !comboButton || !hasHeartthrobsSolo)
            return;

        if (currentInspiration >= (maxInspiration / 4))
        {
            if (context.performed)
            {
                var proj = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
                proj.GetComponent<HeartthrobSoloProjectile>().Initialize(this, (facedDirection.position - transform.position).normalized, this.gameObject);
                soloActive = true;
                currentInspiration -= (maxInspiration / 4);
                UpdateInspirationUI();
            }
        }       
    }

    public void SetComboButton(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            comboButton = true;
        }
        else if (context.canceled)
        {
            comboButton = false;
        }
    }

    public void ResetHearttrhobSolo()
    {
        soloActive = false;
    }

    public void StageDive(InputAction.CallbackContext context)
    {
        if(comboButton || !hasStageDive)
            return;

        if (currentInspiration >= (maxInspiration / 4))
        {
            if (context.performed)
            {
                playerMovement.Dive();
                currentInspiration -= (maxInspiration / 4);
                UpdateInspirationUI();
            }
        }      
    }

    private void OnDrawGizmos()
    {
        if (facedDirection == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
