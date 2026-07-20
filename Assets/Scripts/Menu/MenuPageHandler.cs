using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class MenuPageHandler : MonoBehaviour
{
    [SerializeField] MenuMovable[] menuMovables;
    [SerializeField] ParticleSystem menuParticles;
    [SerializeField] Color particleColor;
    [SerializeField] float particleColorChangeTime;

    Coroutine AnimationCoroutine;
    Coroutine ColorChangeCoroutine;
    Coroutine ParticleColorChange;

    public void TriggerTransition()
    {
        StartCoroutine(HandleTransition());
    }

    IEnumerator HandleTransition()
    {
        foreach (var obj in menuMovables)
        {
            if (TransitionRequired(obj))
            {
                if (obj.targetTransform != null)
                {
                    AnimationCoroutine = StartCoroutine(AnimateMovable(obj));
                }
                ColorChangeCoroutine = StartCoroutine(ChangeColor(obj));
            }        
        }
        ParticleColorChange = StartCoroutine(ChangeParticleColor());
        yield return null;
    }

    bool TransitionRequired(MenuMovable movable)
    {
        if(movable.obj.GetComponent<Image>().color == movable.endColor)
        {
            if(movable.targetTransform != null)
            {
                if (movable.obj.GetComponent<RectTransform>().anchoredPosition == movable.targetTransform.anchoredPosition)
                {
                    return false;
                }
            }         
        }
        return true;      
    }

    IEnumerator AnimateMovable(MenuMovable movable)
    {
        RectTransform rect = movable.obj.GetComponent<RectTransform>();
        Vector2 startPos = movable.obj.GetComponent<RectTransform>().anchoredPosition;
        Vector2 endPos =  movable.targetTransform.anchoredPosition;
        Vector3 startScale = movable.obj.GetComponent<RectTransform>().sizeDelta;
        Vector3 endScale = movable.targetTransform.sizeDelta;

        rect.anchoredPosition = startPos;
        rect.sizeDelta = startScale;

        float elapsed = 0f;
        while (elapsed < movable.duration)
        {
            float t = elapsed / movable.duration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            rect.sizeDelta = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = endPos;
        if (movable.obj.GetComponent<UIBeatPulse>())
        {
            movable.obj.GetComponent<UIBeatPulse>().SetOriginalScale(endScale);
        }
        rect.sizeDelta = endScale;
        AnimationCoroutine = null;
    }

    IEnumerator ChangeColor(MenuMovable movable)
    {
        Image sRend = movable.obj.GetComponent<Image>();

        Color startColor = sRend.color;
        Color endColor = movable.endColor;

        float elapsed = 0f;
        while (elapsed < movable.duration)
        {
            float t = elapsed / movable.duration;
            sRend.color = Color.Lerp(startColor, endColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        sRend.color = endColor;
        ColorChangeCoroutine = null;
    }

    IEnumerator ChangeParticleColor()
    {
        Color startColor = menuParticles.startColor;
        Color endColor = particleColor;

        float elapsed = 0f;
        while (elapsed < particleColorChangeTime)
        {
            float t = elapsed / particleColorChangeTime;
            menuParticles.startColor = Color.Lerp(startColor, endColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }      
        ParticleColorChange = null;
    }

    public bool CheckActiveCoroutines()
    {
        if(AnimationCoroutine != null && ColorChangeCoroutine != null && ParticleColorChange != null)
        {
            return true;
        }

        return false;
    }
}
[System.Serializable]
public class MenuMovable
{
    public GameObject obj;
    public RectTransform targetTransform;
    public float duration = 1f;
    public Color endColor;
}
