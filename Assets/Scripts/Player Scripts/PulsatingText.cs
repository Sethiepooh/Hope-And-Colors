using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Pulsatingtext : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    private bool usingTMP;

    [Header("Pulsate Settings")]
    [Range(0.1f, 5f)]
    public float pulsateSpeed = 1.5f;
    [Range(0.1f, 1f)]
    public float minAlpha = 0.2f;
    [Range(0.1f, 1f)]
    public float maxAlpha = 1.0f;

    [Header("Scale Settings")]
    public bool enableScalePulsate = true;
    [Range(0.8f, 1f)]
    public float minScale = 0.9f;
    [Range(1f, 1.2f)]
    public float maxScale = 1.05f;

    private float currentTime = 0f;
    private Vector3 baseScale;

    private void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();

        usingTMP = (tmpText != null);

        baseScale = transform.localScale;

        currentTime = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        currentTime += Time.deltaTime * pulsateSpeed;

        float pulsateValue = Mathf.Sin(currentTime) * 0.5f + 0.5f;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulsateValue);

        if (usingTMP && tmpText != null)
        {
            Color color = tmpText.color;
            color.a = alpha;
            tmpText.color = color;
        }

        if (enableScalePulsate)
        {
            float scale = Mathf.Lerp(minScale, maxScale, pulsateValue);
            transform.localScale = baseScale * scale;
        }
    }
}