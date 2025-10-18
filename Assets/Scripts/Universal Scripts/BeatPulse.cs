using UnityEngine;

public class BeatPulse : MonoBehaviour
{
    [SerializeField] float pulseScale = 1.2f;
    [SerializeField] float returnSpeed = 5f;
    Vector3 originalScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * returnSpeed);
    }

    public void Pulse()
    {
        transform.localScale = originalScale * pulseScale;
    }
}
