using System.Collections;
using UnityEngine;

public class SequentialLaser : MonoBehaviour
{
    [Header("Beam")]
    [SerializeField] Transform beam;              // pivot at the START of the beam, child of the emitter
    [SerializeField] float maxLength = 30f;
    [SerializeField] LayerMask obstructionMask;   // walls/obstacles only, NOT the player

    [Header("Timing")]
    [SerializeField] float extendSpeed = 40f;     // units/sec while growing
    [SerializeField] float retractSpeed = 40f;    // units/sec while shrinking
    [SerializeField] int beatInterval = 4;
    [SerializeField] bool startActive = false;

    [Header("Debug")]
    public bool debug = false;

    int currentBeat = 0;
    bool isLaserActive;
    float currentLength;

    public bool IsHittingSurface { get; private set; }
    public Vector3 HitPoint { get; private set; }

    private void Start()
    {
        isLaserActive = startActive;
        currentLength = 0f;

        if (startActive)
            currentLength = GetAllowedLength();   // snap to full length on start

        ApplyLength();
    }

    // LateUpdate so it runs after whatever moves the laser this frame
    private void LateUpdate()
    {
        float allowed = GetAllowedLength();

        if (isLaserActive)
        {
            // Grow gradually, but an obstruction cuts the beam instantly
            currentLength = Mathf.MoveTowards(currentLength, allowed, extendSpeed * Time.deltaTime);
            currentLength = Mathf.Min(currentLength, allowed);
        }
        else
        {
            currentLength = Mathf.MoveTowards(currentLength, 0f, retractSpeed * Time.deltaTime);
        }

        ApplyLength();
    }

    public void ToggleLaserActivation()
    {
        if (currentBeat % beatInterval == 0)
            isLaserActive = !isLaserActive;

        currentBeat++;
    }

    float GetAllowedLength()
    {
        Vector2 origin = beam.position;
        Vector2 dir = beam.right;   // local +X is the beam's length axis

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxLength, obstructionMask);

        if (hit.collider != null)
        {
            IsHittingSurface = true;
            HitPoint = hit.point;
            return hit.distance;
        }

        IsHittingSurface = false;
        HitPoint = origin + dir * maxLength;
        return maxLength;
    }

    void ApplyLength()
    {
        bool visible = currentLength > 0.001f;
        if (beam.gameObject.activeSelf != visible)
            beam.gameObject.SetActive(visible);

        if (!visible) return;

        // Convert world length to local scale so a scaled parent doesn't distort it
        float parentScale = beam.parent != null ? beam.parent.lossyScale.x : 1f;
        Vector3 scale = beam.localScale;
        scale.x = currentLength / parentScale;
        beam.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (beam == null) return;
        Gizmos.color = Color.red;
        Vector3 origin = beam.position;
        Vector3 dir = beam.right;
        Gizmos.DrawLine(origin, origin + dir * maxLength);
    }
}