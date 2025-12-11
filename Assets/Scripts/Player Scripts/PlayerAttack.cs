using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    PlayerMovement p_Mov;
    BPMInteract bpmInteract;

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
    [SerializeField] float inspirationgainOffBeat = 1f;
    [SerializeField] float maxInspiration = 100f;
    [SerializeField] float inspirationConsumptionRate = 5f;
    [SerializeField] Slider inspirationBar;
    float inspirationGainOnHit;
    [HideInInspector]public float currentInspiration = 0f;
    PlayerMovement playerMovement;
    public float healthRegenRate;

    [Header("Hyperdrive Riff Stats")]
    [SerializeField] GameObject riffProjectile;
    [SerializeField] Transform riffSpawnPoint;

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
    bool hyperdriveRiff;



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
    }

    // Update is called once per frame
    void Update()
    {
        if(p_Mov.GetMovement() != Vector2.zero)
        {
            facedDirection.transform.position = new Vector2(transform.position.x + (p_Mov.GetMovement().x * 1.5f), transform.position.y + (p_Mov.GetMovement().y * 1.5f));
            attackIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, p_Mov.GetMovement());
            criticalIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, p_Mov.GetMovement());
        }
            
        if (allegroMode)
        {
            GetComponent<Health>().Heal(healthRegenRate * Time.deltaTime);
            if(!stumble)
                sRend.color = allegroColor;
            currentInspiration -= inspirationConsumptionRate * Time.deltaTime;
            inspirationBar.value = currentInspiration / maxInspiration;
            if (inspirationBar.value <= 0)
            {
                sRend.color = defaultColor;
                inspirationBar.value = 0;
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
        if (!canAttack)
            return;

        if (context.performed)
        {         
            if (bpmInteract.CheckInput() == 0)
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
            else if (bpmInteract.CheckInput() == 1)
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
            else if(bpmInteract.CheckInput() == 2)
            {
                Debug.Log("Missed Attack");
                StopCoroutine(AttackCooldown());    
                StartCoroutine(AttackCooldown());
                inspirationGainOnHit = inspirationgainOffBeat;
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
                        inspirationBar.value = currentInspiration / maxInspiration;
                    }
                }
                else if (enemy.gameObject.CompareTag("Obstacle") || enemy.gameObject.CompareTag("Bomb"))
                {
                    // Check if enemy is in front of player
                    Vector2 relativePos = enemy.transform.position - transform.position;
                    Vector2 forward = (Vector2)facedDirection.position - (Vector2)transform.position;
                    Rigidbody2D enemyRb = enemy.gameObject.GetComponent<Rigidbody2D>();
                    float angle = Vector3.Angle(relativePos, forward);
                    if (angle < 90f)
                    {
                        enemyRb.AddForce(forward * strikeForce, ForceMode2D.Impulse);
                        enemy.gameObject.GetComponent<Health>().TakeDamage(currentDamage);
                    }
                }
            }
            canAttack = false;
        }
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
        if(stumble)
            return;

        if (context.performed)
        {
            if (inspirationBar.value >= .25f && !allegroMode)
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

    public void HyperdriveRiff(InputAction.CallbackContext context)
    {
        if(currentInspiration >= (maxInspiration / 4))
        {
            if (context.performed)
            {
                hyperdriveRiff = true;
                p_Mov.SetFreeze(true);
            }
            else if (context.canceled)
            {
                p_Mov.SetFreeze(false);
                hyperdriveRiff = false;
            }
        }       
    }
    
    public void FireHyperdriveRiff()
    {
        if (currentInspiration >= (maxInspiration / 8) && hyperdriveRiff)
        {
            currentInspiration -= (maxInspiration / 8);
            inspirationBar.value = currentInspiration / maxInspiration;
            var blast = Instantiate(riffProjectile, riffSpawnPoint.position, Quaternion.identity);
            blast.GetComponent<Projectile>().direction = facedDirection.localPosition.normalized;
        }
        else
        {
            p_Mov.SetFreeze(false);
            hyperdriveRiff = false;
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
