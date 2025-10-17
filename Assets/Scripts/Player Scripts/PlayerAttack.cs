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
    float attackRange;
    int currentDamage;

    [Header("Inspiration Stats")]
    [SerializeField] float inspirationGainOnBeat = 5f;
    [SerializeField] float inspirationgainOffBeat = 1f;
    [SerializeField] float maxInspiration = 100f;
    [SerializeField] float inspirationConsumptionRate = 5f;
    [SerializeField] Slider inspirationBar;
    float inspirationGainOnHit;
    float currentInspiration = 0f;

    [Header("Hyperdrive Riff Stats")]
    [SerializeField] GameObject riffProjectile;
    [SerializeField] float maxRiffTime;
    [SerializeField] Transform riffSpawnPoint;
    float riffTime;

    bool allegroMode;
    bool hyperdriveRiff;



    void Awake()
    {
        p_Mov = GetComponent<PlayerMovement>();
        currentDamage = baseDamage;
        attackRange = Vector2.Distance(transform.position, facedDirection.position);
    }

    void Start()
    {
        bpmInteract = GameObject.Find("Rhythm Manager").GetComponent<BPMInteract>();
    }

    // Update is called once per frame
    void Update()
    {
        if(p_Mov.GetMovement() != Vector2.zero)
            facedDirection.transform.position = new Vector2(transform.position.x + (p_Mov.GetMovement().x * 1.5f), transform.position.y + (p_Mov.GetMovement().y * 1.5f));

        if (allegroMode)
        {
            currentInspiration -= inspirationConsumptionRate * Time.deltaTime;
            inspirationBar.value = currentInspiration / maxInspiration;
            if (inspirationBar.value <= 0)
            {
                inspirationBar.value = 0;
                allegroMode = false;
            }
        }

        if (hyperdriveRiff)
        {
            p_Mov.SetFreeze(true);
            riffTime += Time.deltaTime;
            if (riffTime >= maxRiffTime)
            {
                if(currentInspiration >= (maxInspiration / 4))
                {
                    currentInspiration -= (maxInspiration / 4);
                    inspirationBar.value = currentInspiration / maxInspiration;
                    var blast = Instantiate(riffProjectile, riffSpawnPoint.position, Quaternion.identity);
                    blast.GetComponent<Projectile>().direction = facedDirection.localPosition.normalized;
                }
                else
                {
                    p_Mov.SetFreeze(false);
                    hyperdriveRiff = false;
                }
                riffTime = 0f;
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (bpmInteract.CheckInput() == 0)
            {
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
            else
            {
                inspirationGainOnHit = inspirationgainOffBeat;
                comboStep = 0;
                if(allegroMode)
                    currentDamage = baseInspirationDamage;
                else
                    currentDamage = baseDamage;
            }

            // Detect enemies in range of attack
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.gameObject.CompareTag("Enemy"))
                {
                    // Check if enemy is in front of player
                    Vector2 relativePos = enemy.transform.position - transform.position;
                    Vector2 forward = (Vector2)facedDirection.position - (Vector2)transform.position;
                    float angle = Vector3.Angle(relativePos, forward);
                    if (angle < 90f)
                    {
                        //Apply damage to enemy & add inspiration to Player
                        Debug.Log("Hit " + enemy.name + " for " + currentDamage + " damage!");
                        enemy.gameObject.GetComponent<Health>().TakeDamage(currentDamage);
                        currentInspiration += inspirationGainOnHit;
                        if (currentInspiration > maxInspiration)
                            currentInspiration = maxInspiration;
                        inspirationBar.value = currentInspiration / maxInspiration;
                    }
                    else
                    {
                        Debug.Log("Missed!");
                    }
                }
            }
        }
    }

    public void Allegro(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inspirationBar.value >= (maxInspiration / 4) / maxInspiration && !allegroMode)
            {
                allegroMode = true;
            }
            else if (allegroMode)
            {
                allegroMode = false;
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
            }
            else if (context.canceled)
            {
                p_Mov.SetFreeze(false);
                hyperdriveRiff = false;
                riffTime = 0f;
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
