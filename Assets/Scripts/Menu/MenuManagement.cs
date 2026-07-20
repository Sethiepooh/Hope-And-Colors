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
        if (CheckActiveTransitions() == true) return;
        menuPages[targetPageIndex].TriggerTransition();
    }

    bool CheckActiveTransitions()
    {
        foreach(var page in menuPages)
        {
            if(page.CheckActiveCoroutines() == true)
            {
                return true;
            }
        }
        return false;
    }
}
