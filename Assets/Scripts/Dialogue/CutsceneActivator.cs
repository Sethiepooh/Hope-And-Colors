using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneActivator : MonoBehaviour, IInteractable
{
    [SerializeField]bool _interactable;
    public bool interactable { get => _interactable; set => _interactable = value; }

    public bool activeInteraction { get; set; }

    public Vector2 position => transform.position;
    
    enum TriggerType {OnEnter, OnExit, OnInteract, OnEvent}
    [SerializeField] TriggerType triggerType;

    [SerializeField] bool DisableAfterTrigger;
    int currentLine;

    [SerializeField] CutsceneData[] cutsceneData;
    [SerializeField] DialogueSystem dialogueSystem;

    Coroutine delayCutsceneCoroutine;



    public void OnInteract()
    {
        if (interactable)
        {
            if (activeInteraction)
            {
                if (!cutsceneData[currentLine - 1].goToNextLineAutomatically)
                    TriggerCutscene();
            }
            else
            {
                if (triggerType == TriggerType.OnInteract)
                {
                    TriggerCutscene();
                }
            }           
        }
    }  

    public void OnEventTrigger()
    {
        if (interactable)
        {
            dialogueSystem.SetInteractable(this);
            TriggerCutscene();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!interactable) { return; }

        if (collision.CompareTag("Player"))
        {
            if (triggerType == TriggerType.OnEnter)
            {
                TriggerCutscene();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!interactable) { return; }

        if (collision.CompareTag("Player"))
        {
            if (triggerType == TriggerType.OnExit)
            {
                TriggerCutscene();
            }
        }
    }

    private void Update()
    {
        if(currentLine == 0) { return; }

        if (cutsceneData[currentLine - 1].goToNextLineAutomatically && activeInteraction && !dialogueSystem.IsScrollingText())
        {
            if (cutsceneData[currentLine - 1].autoAdvanceDelay <= 0f)
            {
                TriggerCutscene();
            }
            else
            {
                if (delayCutsceneCoroutine == null)
                {
                    delayCutsceneCoroutine = StartCoroutine(DelayCutscene(cutsceneData[currentLine - 1].autoAdvanceDelay));
                }
            }        
        }
    }

    IEnumerator DelayCutscene(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerCutscene();
        delayCutsceneCoroutine = null;
    }

    public void TriggerCutscene()
    {
        if (!activeInteraction)
        {
            activeInteraction = true;  
        }

        if (dialogueSystem.IsPlayerControllable())
        {
            dialogueSystem.ToggleFreezePlayer(true);
        }

        if (currentLine < cutsceneData.Length)
        {
            if (!dialogueSystem.IsScrollingText())
            {
                dialogueSystem.UpdateDialogueUI(cutsceneData[currentLine].GetSpeakerName(), cutsceneData[currentLine].GetSpeakerColor(), 
                    cutsceneData[currentLine].dialogueLine, cutsceneData[currentLine].GetSpeakerExpression(), cutsceneData[currentLine].DialogueUIState);

                dialogueSystem.StartRollingText();

                if (cutsceneData[currentLine].GetScreenEffect() != ScreenEffectEnum.ScreenEffect.None)
                    dialogueSystem.PlayScreenEffect(cutsceneData[currentLine].GetScreenEffect());

                if(cutsceneData[currentLine].GetSoundEffect() != SoundEffectEnum.SoundEffect.None)
                    dialogueSystem.PlaySoundEffect(cutsceneData[currentLine].GetSoundEffect());

                if (cutsceneData[currentLine].GetAction() != ActionEnum.Action.None)
                {
                    if (cutsceneData[currentLine].activateBeforeAction)
                        cutsceneData[currentLine].actionTarget.SetActive(true);

                    dialogueSystem.HandleAction(cutsceneData[currentLine].actionTarget, cutsceneData[currentLine].endPos.position, cutsceneData[currentLine].actionDuration);
                }

                if (cutsceneData[currentLine].GetCameraState() != CameraEnum.ChangeCameraState.None)
                {
                    if (cutsceneData[currentLine].GetCameraState() == CameraEnum.ChangeCameraState.FollowPlayer)
                    {
                        dialogueSystem.ReturnCamToPlayer(cutsceneData[currentLine].cameraTransitionTime);
                    }
                    else
                    {
                        dialogueSystem.MoveCamToPoint(cutsceneData[currentLine].cameraTarget, cutsceneData[currentLine].cameraTransitionTime, cutsceneData[currentLine].cameraFocus);
                    }
                }

                cutsceneData[currentLine].TriggerEvents();
                currentLine++;
            }
            else
            {
                if (cutsceneData[currentLine - 1].unskippable) { return; }
                dialogueSystem.SkipRollingText();
            }            
        }
        else
        {
            dialogueSystem.UpdateDialogueUI("", Color.white, "", null, false);
            activeInteraction = false;
            currentLine = 0;
            dialogueSystem.ToggleFreezePlayer(false);
            if (DisableAfterTrigger)
            {
                interactable = false;
                this.enabled = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(cutsceneData == null) { return; }
        foreach (CutsceneData data in cutsceneData)
        {
            if (data.GetAction() != ActionEnum.Action.None)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(data.actionTarget.transform.position, data.endPos.position);
                Gizmos.DrawWireSphere(data.endPos.position, .2f);
            }
        }
    }
}
