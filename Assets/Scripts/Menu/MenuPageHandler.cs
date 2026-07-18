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
                    StartCoroutine(AnimateMovable(obj));
                }
                StartCoroutine(ChangeColor(obj));
            }        
        }
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
        rect.sizeDelta = endScale;
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
