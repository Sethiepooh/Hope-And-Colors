using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] GameObject dialogueUI;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Image characterSprite;

    [Header("Screen Effects")]
    [SerializeField] Image screenEffectOverlay;
    [SerializeField] CinemachineImpulseSource screenShakeSource;


    [SerializeField] float textScrollSpeed;
    Coroutine rollingTextCoroutine;
    bool isScrolling;

    [Header("Audio")]
    [SerializeField] AudioClip[] soundEffects;

    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] InteractionManager interactionManager;
    [SerializeField] ManualCameraControl cameraControl;

    public void SetInteractable(IInteractable interactable)
    {
        interactionManager.nearbyInteractable = interactable;
    }

    public void UpdateDialogueUI(string name, Color nameColor, string dialogueLine, Sprite characterSprite, bool UIactiveState = true)
    {
        nameText.text = name;
        nameText.color = nameColor;
        dialogueText.text = dialogueLine;
        this.characterSprite.sprite = characterSprite;
        ToggleDialogueUI(UIactiveState);
    }

    public void StartRollingText()
    {
        rollingTextCoroutine = StartCoroutine(ShowRollingText());
    }

    public IEnumerator ShowRollingText()
    {
        isScrolling = true;

        // Force TMP to generate the full mesh so wrapping is pre-calculated
        dialogueText.ForceMeshUpdate();

        int totalCharacters = dialogueText.textInfo.characterCount;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(1f / textScrollSpeed);
        }

        isScrolling = false;
    }

    public void SkipRollingText()
    {
        if (!isScrolling) return;

        if(rollingTextCoroutine != null)
        {
            StopCoroutine(rollingTextCoroutine);
        }

        // Reveal all characters instantly and end the scroll
        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        isScrolling = false;
    }

    public bool IsScrollingText()
    {
        return isScrolling;
    }

    public void ToggleDialogueUI(bool state)
    {
        dialogueUI.SetActive(state);
    }

    public void PlayScreenEffect(ScreenEffectEnum.ScreenEffect effect)
    {
        if (effect == ScreenEffectEnum.ScreenEffect.Flash)
        {
            StartCoroutine(ScreenFadeCoroutine(0f, 1f, .2f));
        }
        else if (effect == ScreenEffectEnum.ScreenEffect.Shake)
        {
            ScreenShake();
        }
        else if (effect == ScreenEffectEnum.ScreenEffect.FadeIn)
        {
            StartCoroutine(ScreenFadeCoroutine(1f, 0f, 5f));
        }
        else if (effect == ScreenEffectEnum.ScreenEffect.FadeOut)
        {
            StartCoroutine(ScreenFadeCoroutine(0f, 1f, 5f));
        }
    }

    public void PlaySoundEffect(SoundEffectEnum.SoundEffect effect)
    {
        foreach (AudioClip clip in soundEffects)
        {
            if (clip.name == effect.ToString())
            {
                SFXManager.Instance.PlaySFX(clip);
                break;
            }
        }
    }

    public void HandleAction(GameObject actor, Vector2 actionEnd,  float duration)
    {
        // Simple movement action - can be expanded with more complex actions as needed
        StartCoroutine(MoveActor(actor, actor.transform.position, actionEnd, duration));
    }

    IEnumerator MoveActor(GameObject actor, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            actor.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }
        actor.transform.position = endPos; // Ensure final position is set
    }

    public void ReturnCamToPlayer(float time)
    {
        cameraControl.StartCoroutine(cameraControl.RepositionCamera(playerMovement.transform, time, true));
    }

    public void MoveCamToPoint(Transform point, float time, float focus = 10f)
    {
        cameraControl.StartCoroutine(cameraControl.RepositionCamera(point, time, false, focus));
    }

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ToggleFreezePlayer(bool freeze)
    {
        playerMovement.SetFreeze(freeze);
    }

    public bool IsPlayerControllable()
    {
        return playerMovement.controlable;
    }


    #region Screen Effects
    IEnumerator ScreenFadeCoroutine(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color overlayColor = screenEffectOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            screenEffectOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, alpha);
            yield return null;
        }
    }

    void ScreenShake()
    {
        screenShakeSource.GenerateImpulse();
    }

    #endregion
}
