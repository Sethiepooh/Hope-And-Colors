using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CutsceneData
{
    public string dialogueLine;
    public bool DialogueUIState;
    public bool unskippable;
    public bool goToNextLineAutomatically;
    public float autoAdvanceDelay;

    [Header("Action Settings")]
    public CutsceneAction[] actions;

    [Header("Camera Settings")]
    [SerializeField] CameraEnum.ChangeCameraState cameraState;
    public float cameraTransitionTime;
    public Transform cameraTarget;
    public float cameraFocus;

    [Header("Scene Management Settings")]
    [SerializeField] bool changeScene;
    [SerializeField] int sceneIndex;

    [Header("Speaker Settings")]
    [SerializeField] CharacterData[] speakerData;
    public bool obscureSpeaker;
    [SerializeField] CharacterEnum.Character speaker;
    [SerializeField] ExpressionEnum.Expression speakerExpression;

    [Header("Effect Settings")]
    [SerializeField] ScreenEffectEnum.ScreenEffect screenEffect;
    [SerializeField] SoundEffectEnum.SoundEffect soundEffect;

    [Header("Cutscene Events")]
    [SerializeField] UnityEvent lineEvents;

    public string GetSpeakerName()
    {
        if(obscureSpeaker)
        {
            return "???";
        }
        return speaker.ToString();
    }

    private void Reset()
    {
        dialogueLine = "Default Text";
    }

    public Color GetSpeakerColor()
    {
        foreach (CharacterData character in speakerData)
        {
            if (character.characterName == speaker.ToString())
            {
                return character.textColor;
            }
        }
        return Color.white; // Default color if speaker not found
    }

    public Sprite GetSpeakerExpression()
    {
        if(speakerExpression == ExpressionEnum.Expression.NULL)
        {
            return null; // No expression to display
        }
        foreach (CharacterData character in speakerData)
        {
            if (character.characterName == speaker.ToString())
            {
                if(character.characterExpressions[(int)speakerExpression] != null)
                {
                    return character.characterExpressions[(int)speakerExpression];
                }
            }
        }
        return null; // Default sprite if speaker not found
    }

    public void TriggerEvents()
    {
        lineEvents.Invoke();
    }

    public SoundEffectEnum.SoundEffect GetSoundEffect()
    {
        return soundEffect; 
    }

    public ScreenEffectEnum.ScreenEffect GetScreenEffect()
    {
        return screenEffect; 
    }

    public CameraEnum.ChangeCameraState GetCameraState()
    {
        return cameraState;
    }
}

[System.Serializable]
public class CutsceneAction
{
    public ActionEnum.Action action;
    public bool activateBeforeAction;
    public bool deactivateAfterAction;
    public GameObject actionTarget;
    public Transform endPos;
    public float actionDuration;
    public bool flipSprite;
} 
 