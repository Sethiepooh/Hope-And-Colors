using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    PlayerMovement p_Mov;
    BPMInteract bpmInteract;

    //Combo system variables
    int comboStep = 0;
    int maxComboStep = 4;

    [Header("Damage Stats")]
    [SerializeField] int baseDamage = 10;
    [SerializeField] int damageIncreasePerCombo = 2;
    float attackRange;
    [SerializeField] Transform facedDirection;
    int currentDamage;

    public GameObject target;

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
            facedDirection.transform.position = new Vector2(transform.position.x + p_Mov.GetMovement().x, transform.position.y + p_Mov.GetMovement().y);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (bpmInteract.CheckInput())
            {
                if(comboStep < maxComboStep)
                {
                    comboStep++;
                    currentDamage = baseDamage + damageIncreasePerCombo;
                }
                else
                {
                    comboStep = 1;
                    currentDamage = baseDamage;
                }
            }
            else
            {
                comboStep = 0;
                currentDamage = baseDamage;
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach(Collider2D enemy in hitEnemies)
            {
                Vector2 relativePos = enemy.transform.position - transform.position;
                Vector2 forward = (Vector2)facedDirection.position - (Vector2)transform.position;
                float angle = Vector3.Angle(relativePos, forward);
                if (angle < 90f)
                {
                    Debug.Log("Hit for " + currentDamage + " damage!");
                }
                else
                {
                    Debug.Log("Missed!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (facedDirection == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
