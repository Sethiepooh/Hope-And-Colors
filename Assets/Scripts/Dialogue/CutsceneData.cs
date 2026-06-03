using System;
using UnityEngine;

[System.Serializable]
public class CutsceneData
{
    public bool DialogueUIState;
    public bool unskippable;
    public bool goToNextLineAutomatically;
    public float autoAdvanceDelay;

    [Header("Action Settings")]
    [SerializeField] ActionEnum.Action action;
    public GameObject actionTarget;
    public Transform endPos;
    public float actionDuration;

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
    [SerializeField] CharacterEnum.Character speaker;
    [SerializeField] ExpressionEnum.Expression speakerExpression;
    public string dialogueLine;

    [Header("Effect Settings")]
    [SerializeField] ScreenEffectEnum.ScreenEffect screenEffect;
    [SerializeField] SoundEffectEnum.SoundEffect soundEffect;

    public string GetSpeakerName()
    {
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

    public SoundEffectEnum.SoundEffect GetSoundEffect()
    {
        return soundEffect; 
    }

    public ScreenEffectEnum.ScreenEffect GetScreenEffect()
    {
        return screenEffect; 
    }

    public ActionEnum.Action GetAction()
    {
        return action;
    }

    public CameraEnum.ChangeCameraState GetCameraState()
    {
        return cameraState;
    }
}
 