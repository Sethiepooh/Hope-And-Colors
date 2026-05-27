using System;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BPMInteract : MonoBehaviour
{
    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Intervals[] intervals;
    [SerializeField] private SongSection[] sections;        // Define sections in Inspector

    float clipLengthInBeats;
    [HideInInspector]public bool attackWindow;
   // public bool adaptive;

    private int currentSectionIndex = 0;
    public bool transitionQueued = false;

    // 32 beats = 1 movement
    private const int BEATS_PER_MOVEMENT = 32;
    private int lastMovement = -1;

    public event Action<int> OnMovementChanged;          // fires with new movement index
    public event Action<SongSection> OnSectionChanged;   // fires when section transitions

    private void Update()
    {
        if (audioSource == null) return;

        foreach (Intervals interval in intervals)
        {
            float sampledTime = (audioSource.timeSamples /
                (audioSource.clip.frequency * interval.GetIntervalLength(bpm)));
            interval.CheckForNewInterval(sampledTime);
        }

        Check();
        TrackMovement();
        HandleSectionLooping();
    }

    #region Movement Tracking

    /// <summary>Returns the current movement number (0-based). Every 32 beats = 1 movement.</summary>
    public int GetCurrentMovement()
    {
        if (audioSource == null) return 0;
        float currentBeat = GetCurrentBeat();
        return Mathf.FloorToInt(currentBeat / BEATS_PER_MOVEMENT);
    }

    /// <summary>Returns the current beat position in the audio clip.</summary>
    public float GetCurrentBeat()
    {
        if (audioSource == null) return 0f;
        float secondsPerBeat = 60f / bpm;
        return audioSource.timeSamples / (float)audioSource.clip.frequency / secondsPerBeat;
    }

    /// <summary>Returns which beat we're on within the current movement (0–31).</summary>
    public int GetBeatWithinMovement()
    {
        return Mathf.FloorToInt(GetCurrentBeat()) % BEATS_PER_MOVEMENT;
    }

    private void TrackMovement()
    {
        int currentMovement = GetCurrentMovement();
        if (currentMovement != lastMovement)
        {
            lastMovement = currentMovement;
            OnMovementChanged?.Invoke(currentMovement);

            // Check if current section wants to transition at the start of a new movement
            if (transitionQueued && GetBeatWithinMovement() == 0)
                ExecuteTransition();
        }
    }
    #endregion

    #region Section Management

    /// <summary>Returns the currently active SongSection.</summary>
    public SongSection GetCurrentSection() => sections[currentSectionIndex];

    /// <summary>Returns the index of the currently active section.</summary>
    public int GetCurrentSectionIndex() => currentSectionIndex;

    /// <summary>
    /// Queues a transition to the next section. The transition happens at the
    /// start of the next movement boundary so the music stays on the grid.
    /// </summary>
    public void QueueTransitionToNextSection()
    {
        Debug.Log("Queueing transition to next section...");
        if (currentSectionIndex < sections.Length - 1)
            transitionQueued = true;
    }

    /// <summary>Queues a transition to a specific section index.</summary>
    public void QueueTransitionToSection(int sectionIndex)
    {
        if (sectionIndex >= 0 && sectionIndex < sections.Length && sectionIndex != currentSectionIndex)
        {
            sections[currentSectionIndex].pendingTransitionTarget = sectionIndex;
            transitionQueued = true;
        }
    }

    private void HandleSectionLooping()
    {
        if (audioSource == null || sections == null || sections.Length == 0) return;

        SongSection current = sections[currentSectionIndex];
        float currentBeat = GetCurrentBeat();

        // If we've reached the end beat of this section's loop range, jump back to loop point
        if (currentBeat >= current.loopEndBeat && !transitionQueued)
        {
            Debug.Log($"Looping section '{current.name}' back to beat {current.loopStartBeat}...");
            SeekToBeat(current.loopStartBeat);
        }
        // If a transition is queued and we've hit the loop end, execute it now
        else if (currentBeat >= current.loopEndBeat && transitionQueued)
        {
            ExecuteTransition();
        }
    }

    private void ExecuteTransition()
    {
        SongSection current = sections[currentSectionIndex];

        // Use a specific target if one was set, otherwise just go to next
        int targetIndex = current.pendingTransitionTarget >= 0
            ? current.pendingTransitionTarget
            : currentSectionIndex + 1;

        current.pendingTransitionTarget = -1;
        transitionQueued = false;

        if (targetIndex >= sections.Length) return;

        currentSectionIndex = targetIndex;
        SongSection next = sections[currentSectionIndex];
        Debug.Log($"Executing section transition to '{next.name}'");
        SeekToBeat(next.loopStartBeat);

        OnSectionChanged?.Invoke(next);
    }

    /// <summary>Seeks the audio to an exact beat position.</summary>
    public void SeekToBeat(float beat)
    {
        float secondsPerBeat = 60f / bpm;
        int targetSample = Mathf.RoundToInt(beat * secondsPerBeat * audioSource.clip.frequency);
        targetSample = Mathf.Clamp(targetSample, 0, audioSource.clip.samples - 1);
        audioSource.timeSamples = targetSample;
    }

    #endregion

    // ─── Existing Methods (unchanged) ────────────────────────────────────────

    public int CheckInput()
    {
        if (audioSource == null) return -1;
        float sampledTime = (audioSource.timeSamples /
            (audioSource.clip.frequency * intervals[0].GetIntervalLength(bpm)));
        float lastInterval = intervals[0].GetLastInterval();
        float nextInterval = lastInterval + 1;

        if (sampledTime < lastInterval + .2f || sampledTime > nextInterval - .2f) return 0;
        else if (sampledTime < lastInterval + .4f || sampledTime > nextInterval - .4f) return 1;
        else return 2;
    }

    public void Check()
    {
        float sampledTime = (audioSource.timeSamples /
            (audioSource.clip.frequency * intervals[0].GetIntervalLength(bpm)));
        attackWindow = sampledTime < intervals[0].GetLastInterval() - .2f
                    || sampledTime > intervals[0].GetLastInterval() + .8f;
    }

    public float GetBPM() => bpm;
}

// ─── Song Section Definition ─────────────────────────────────────────────────

[System.Serializable]
public class SongSection
{
    public string name;
    public float loopStartBeat;     // Beat to loop back to (e.g. 0, 32, 64)
    public float loopEndBeat;       // Beat at which to loop/transition (e.g. 32, 64, 96)

    [HideInInspector] public int pendingTransitionTarget = -1;

    /// <summary>How many full movements this section spans.</summary>
    public int MovementCount => Mathf.RoundToInt((loopEndBeat - loopStartBeat) / 32f);
}

// ─── Intervals ───────────────────────────────────────────────────

[System.Serializable]
public class Intervals
{
    [SerializeField] private float steps;
    [SerializeField] private UnityEvent trigger;
    private int lastInterval;

    public float GetIntervalLength(float bpm) => 60f / (bpm * steps);

    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != lastInterval)
        {
            lastInterval = Mathf.FloorToInt(interval);
            trigger.Invoke();
        }
    }

    public int GetLastInterval() => lastInterval;
}
