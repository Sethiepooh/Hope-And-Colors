using UnityEngine;
using UnityEngine.Events;

public class MultiCutsceneHandler : MonoBehaviour, IInteractable
{
    public CutsceneActivator[] triggerableCutscenes;
    public CutsceneActivator fallbackCutscene;

    enum ListReadSetting
    {
        Ordered,
        Random
    }
    [SerializeField] ListReadSetting readSetting;

    public bool interactable { get => canInteract; set => canInteract = value; }
    [SerializeField] bool canInteract = true;
    public bool activeInteraction { get; set; }

    public Vector2 position => transform.position;

    public void OnInteract()
    {
        switch (readSetting)
        {        
            case ListReadSetting.Ordered:
                Debug.Log("Ordered");
                for (int i = 0; i < triggerableCutscenes.Length; i++)
                {
                    if (triggerableCutscenes[i].interactable != false)
                    {
                        Debug.Log("Invoking cutscene at index " + i);
                        triggerableCutscenes[i].OnEventTrigger();
                        return;
                    }
                }
                fallbackCutscene.OnEventTrigger();
                break;
            case ListReadSetting.Random:
                Debug.Log("Random");
                int randomIndex = Random.Range(0, triggerableCutscenes.Length);
                if (CheckInteractableState())
                {
                    for (int i = 0; i < triggerableCutscenes.Length; i++)
                    {
                        if (triggerableCutscenes[randomIndex].interactable != false)
                        {
                            Debug.Log("Invoking cutscene at index " + randomIndex);
                            triggerableCutscenes[randomIndex].OnEventTrigger();
                            return;
                        }
                        else
                        {
                            randomIndex = Random.Range(0, triggerableCutscenes.Length);
                            i = 0;
                        }
                    }
                }
                else
                {
                    fallbackCutscene.OnEventTrigger();
                }
                break;
        }
    }

    bool CheckInteractableState()
    {
        for (int i = 0; i < triggerableCutscenes.Length; i++)
        {
            if (triggerableCutscenes[i].interactable != false)
            {
                return true;
            }
        }
        return false;
    }
}
