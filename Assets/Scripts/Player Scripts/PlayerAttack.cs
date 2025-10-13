using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    BPMInteract bpmInteract;
    int comboStep = 0;
    int maxComboStep = 4;

    int currentDamage;
    [SerializeField] int baseDamage = 10;
    [SerializeField] int damageIncreasePerCombo = 2;
    
    void Awake()
    {
        currentDamage = baseDamage;
    }

    void Start()
    {
        bpmInteract = GameObject.Find("Rhythm Manager").GetComponent<BPMInteract>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            
        }
    }
}
