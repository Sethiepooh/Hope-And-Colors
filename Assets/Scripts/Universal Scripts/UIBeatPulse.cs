using UnityEngine;

public class UIBeatPulse : MonoBehaviour
{

    [SerializeField] float pulseScale = 1.2f;
    [SerializeField] float returnSpeed = 5f;
    Vector3 originalScale;
    bool scaleInitialized = false;
    RectTransform rectTransform;


    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        if (!scaleInitialized)
        {
            originalScale = rectTransform.sizeDelta;
            scaleInitialized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.sizeDelta = Vector3.Lerp(rectTransform.sizeDelta, originalScale, Time.deltaTime * returnSpeed);
    }

    public void Pulse()
    {
        rectTransform.sizeDelta = originalScale * pulseScale;
    }

    public void SetOriginalScale(Vector2 scale)
    {
        originalScale = scale;
    }
}
