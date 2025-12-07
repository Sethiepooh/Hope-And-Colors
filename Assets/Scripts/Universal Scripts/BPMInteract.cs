using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BPMInteract : MonoBehaviour
{
    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Intervals[] intervals;

    float clipLengthInBeats;

    public bool attackWindow;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
             GetAudioLength(audioSource.clip);

       
    }

    private void Update()
    {
        foreach(Intervals interval in intervals)
        {
            float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
            interval.CheckForNewInterval(sampledTime);           
        }

        Check();
    }

    void GetAudioLength(AudioClip clip)
    {
        clipLengthInBeats = Mathf.FloorToInt(audioSource.clip.samples / (audioSource.clip.frequency * intervals[3].GetIntervalLength(bpm)));
        Debug.Log("Clip Length in Beats: " + clipLengthInBeats);
    }

    public void CheckSegmentEnd()
    {
        Debug.Log("Segment End Reached");
    }

    public int GetCurrentSection()
    {
        float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * intervals[3].GetIntervalLength(bpm)));
        return Mathf.FloorToInt(sampledTime);
    }

    public void PlayAudioFromSection(float section)
    {
        float secondsPerBeat = 60f / bpm;
        float targetTime = (section * 32) * secondsPerBeat;
        audioSource.time = Mathf.Clamp(targetTime, 0f, audioSource.clip.length);
    }

    public int CheckInput()
    {
        float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * intervals[0].GetIntervalLength(bpm)));

        float lastInterval = intervals[0].GetLastInterval();
        float nextInterval = lastInterval + 1;

        if (sampledTime < lastInterval + .2f || sampledTime > nextInterval - .2f)
        {
            return 0;
        }
        else if(sampledTime < lastInterval + .4f || sampledTime > nextInterval - .4f)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    public void Check()
    {
        float sampledTime = (audioSource.timeSamples / (audioSource.clip.frequency * intervals[0].GetIntervalLength(bpm)));
        if(sampledTime < intervals[0].GetLastInterval() - .2f || sampledTime > intervals[0].GetLastInterval() + .8f)
        {
            attackWindow = true;
        }
        else
        {
            attackWindow = false;
        }
    }

    public float GetBPM()
    {
        return bpm;
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
