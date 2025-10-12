using UnityEngine;
using UnityEngine.Events;

public class BPMInteract : MonoBehaviour
{
    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Intervals[] intervals;

    // Windows that attacks will register as part of a combo
    float maxAttackGrace;
    float attackGrace;
    bool attackInGracePeriod;

    float maxAttackPreFire;
    float attackPreFire;
    bool attackInPreFirePeriod;

    // Windows that attacks will register as weak
    float maxAttackDowntime;
    float attackDowntime;

    public bool attackWindow = true;

    private void Awake()
    {
        maxAttackGrace = intervals[0].GetIntervalLength(bpm) * 0.1f;
        attackGrace = maxAttackGrace;
        maxAttackPreFire = intervals[0].GetIntervalLength(bpm) * 0.1f;
        attackPreFire = maxAttackPreFire;
        maxAttackDowntime = intervals[0].GetIntervalLength(bpm) * 0.8f;
        attackDowntime = maxAttackDowntime;
    }

    private void Update()
    {
        foreach(Intervals interval in intervals)
        {
            float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
            interval.CheckForNewInterval(sampledTime);           
        }

        if(attackInGracePeriod || attackInPreFirePeriod)
        {
            attackWindow = true;
        }
        else
        {
            attackWindow = false;
        }      
    }

    private void FixedUpdate()
    {
        // Manage Attack Windows
        if (attackGrace > 0)
        {
            attackGrace -= Time.fixedDeltaTime;
        }
        else
        {
            if (attackInGracePeriod)
            {
                attackDowntime = maxAttackDowntime;
                attackInGracePeriod = false;
            }
        }

        if (attackDowntime > 0)
        {
            attackDowntime -= Time.fixedDeltaTime;
        }
        else
        {
            if (!attackWindow)
            {
                attackPreFire = maxAttackPreFire;
                attackInPreFirePeriod = true;
            }
        }

        if (attackPreFire > 0)
        {
            attackPreFire -= Time.fixedDeltaTime;
        }
        else
        {
            if (attackInPreFirePeriod)
            {
                attackGrace = maxAttackGrace;
                attackInGracePeriod = true;
                attackInPreFirePeriod = false;
            }

        }
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float steps;
    [SerializeField] private UnityEvent trigger;
    private int lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * steps);
    }

    public void CheckForNewInterval (float interval)
    {
        if(Mathf.FloorToInt(interval) != lastInterval)
        {
            lastInterval = Mathf.FloorToInt(interval);
            trigger.Invoke();
        }
    }
}
