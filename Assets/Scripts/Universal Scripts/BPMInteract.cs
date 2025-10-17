using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;

public class BPMInteract : MonoBehaviour
{
    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Intervals[] intervals;

    float clipLengthInBeats;

    public bool attackWindow = true;

    private void Awake()
    {
        GetAudioLength(audioSource.clip);
    }

    private void Update()
    {
        foreach(Intervals interval in intervals)
        {
            float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
            interval.CheckForNewInterval(sampledTime);           
        }
    }

    void GetAudioLength(AudioClip clip)
    {
        clipLengthInBeats = (audioSource.timeSamples / (audioSource.clip.frequency * intervals[0].GetIntervalLength(bpm)));
    }

    public int CheckInput()
    {
        float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * intervals[0].GetIntervalLength(bpm)));
        if(sampledTime  < intervals[0].GetLastInterval() - .1f || sampledTime > intervals[0].GetLastInterval() + .9f)
        {
            return 0;
        }
        else if(sampledTime < intervals[0].GetLastInterval() - .2f || sampledTime > intervals[0].GetLastInterval() + .9f)
        {
            return 1;
        }
        else
        {
            return 2;
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

    public int GetLastInterval()
    {
        return lastInterval;
    }
}
