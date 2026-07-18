using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManagement : MonoBehaviour
{
    [SerializeField] MenuPageHandler[] menuPages;
    bool inital = true;

    public void TriggerTitleTransition(InputAction.CallbackContext context)
    {
        if (inital)
        {
            PageTransition(0);
            inital = false; 
        }
    }

    public void PageTransition(int targetPageIndex)
    {
        menuPages[targetPageIndex].TriggerTransition();
    }
}
