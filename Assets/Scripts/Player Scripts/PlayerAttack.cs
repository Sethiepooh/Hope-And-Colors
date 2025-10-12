using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    BPMInteract bpmInteract;
    int comboStep = 0;
    int maxComboStep = 4;

    public int baseDamage = 10;
    public int damageIncreasePerCombo = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if (context.performed && bpmInteract.attackWindow)
        {
            Debug.Log("On Beat!");
        }
        else if(context.performed && !bpmInteract.attackWindow)
        {
            Debug.Log("Missed Beat!");
        }
    }
}
