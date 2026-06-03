using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// Drives Unity beat events from an FMOD event's timeline and provides
/// rhythm-accuracy scoring for player input.
///
/// Setup:
///   1. Attach this component to any GameObject.
///   2. Assign your FMOD event reference to 'FmodEventRef'.
///   3. Set 'BPM' to match your track.
///   4. Wire up OnBeat / OnHalfBeat Unity Events in the Inspector.
///   5. Optionally tune the Perfect / Good timing windows.
/// </summary>
public class BPMInteract : MonoBehaviour
{
    [Header("FMOD")]
    [Tooltip("The FMOD Studio event to play and sync with.")]
    [SerializeField] private EventReference fmodEventRef;

    [Header("BPM")]
    [Tooltip("Beats per minute of the track. Must match the FMOD event.")]
    [SerializeField] private float bpm = 120f;

    [Header("Beat Events")]
    [Tooltip("Fired once per beat (whole-time).")]
    public UnityEvent OnBeat;

    [Tooltip("Fired twice per beat (double-time / eighth-note grid).")]
    public UnityEvent OnHalfBeat;

    [Header("Timing Windows")]
    [Tooltip("Seconds from a beat centre counted as PERFECT (returns 0).")]
    [SerializeField] private float perfectWindow = 0.06f;   // ± 60 ms

    [Tooltip("Seconds from a beat centre counted as GOOD (returns 1). Anything beyond returns 2.")]
    [SerializeField] private float goodWindow = 0.15f;      // ± 150 ms

    [Header("Loop Detection")]
    [Tooltip("If the FMOD timeline jumps backward by more than this many seconds, a loop is detected and the accumulator resyncs.")]
    [SerializeField] private float loopDetectionThreshold = 0.2f;


    private EventInstance _instance;
    int musicPhase = 0;
    private bool _isPlaying;

    // Seconds for one full beat / half beat
    private double BeatInterval => 60.0 / bpm;
    private double HalfBeatInterval => 30.0 / bpm;

    // Time accumulator that advances with deltaTime and resyncs on loop/seek.
    // Beat detection and ScoreInput both read from this value.
    private double _accumulatedTime;

    // Last raw FMOD position (seconds) — used to detect discontinuities.
    private double _lastFmodPosition;

    // The accumulated-time values at which the last beat/half-beat fired,
    // used to avoid double-firing within the same interval.
    private double _lastBeatFiredAt;
    private double _lastHalfBeatFiredAt;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        Play();
    }

    private void Update()
    {
        if (!_isPlaying) return;

        UpdateAccumulatedTime();
        CheckBeats();
    }

    private void OnDestroy()
    {
        Stop();
    }

    // -------------------------------------------------------------------------
    // Playback control
    // -------------------------------------------------------------------------

    /// <summary>Starts playback from the beginning and resets all beat counters.</summary>
    public void Play()
    {
        Stop();

        _instance = RuntimeManager.CreateInstance(fmodEventRef);
        _instance.start();
        _isPlaying = true;

        _accumulatedTime = 0;
        _lastFmodPosition = 0;
        _lastBeatFiredAt = -BeatInterval;     // ensure first beat fires immediately
        _lastHalfBeatFiredAt = -HalfBeatInterval;
    }

    /// <summary>Stops playback and releases the FMOD instance.</summary>
    public void Stop()
    {
        if (_isPlaying)
        {
            _instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _instance.release();
            _isPlaying = false;
        }
    }

    /// <summary>Pauses or resumes the FMOD event.</summary>
    public void SetPaused(bool paused)
    {
        if (_isPlaying)
            _instance.setPaused(paused);
    }

    public void TriggerNextPhase()
    {
        musicPhase++;
        _instance.setParameterByName("Phase", musicPhase);
    }

    public int GetMusicPhase()
    {
        return musicPhase;
    }

    // -------------------------------------------------------------------------
    // Beat tracking
    // -------------------------------------------------------------------------

    private void UpdateAccumulatedTime()
    {
        _instance.getTimelinePosition(out int posMs);
        double fmodPos = posMs / 1000.0;

        double delta = fmodPos - _lastFmodPosition;

        if (delta < 0 || delta > loopDetectionThreshold)
        {
            // A loop or seek has occurred — resync the accumulator so that
            // _accumulatedTime % beatInterval still lines up with the beat grid
            // at the new playback position.
            //
            // Strategy: keep the fractional beat phase from fmodPos so the very
            // next beat fires exactly one beat-length after the loop point.
            double beatPhase = fmodPos % BeatInterval;
            double halfBeatPhase = fmodPos % HalfBeatInterval;

            // Snap _accumulatedTime to the nearest multiple of BeatInterval
            // that preserves the new phase.
            double snappedBeats = Math.Floor(_accumulatedTime / BeatInterval) * BeatInterval + beatPhase;
            // If snapping moved us backwards, step forward one interval.
            if (snappedBeats < _accumulatedTime)
                snappedBeats += BeatInterval;

            _accumulatedTime = snappedBeats;
            _lastFmodPosition = fmodPos;

            // Rearm the fired-at guards so beats fire normally from the new position.
            _lastBeatFiredAt = _accumulatedTime - BeatInterval + beatPhase - BeatInterval;
            _lastHalfBeatFiredAt = _accumulatedTime - HalfBeatInterval + halfBeatPhase - HalfBeatInterval;
        }
        else
        {
            // Normal frame — advance by real elapsed time.
            _accumulatedTime += Time.deltaTime;
            _lastFmodPosition = fmodPos;
        }
    }

    private void CheckBeats()
    {
        // --- Whole beats ---
        // A beat is due when _accumulatedTime has passed the next expected beat time.
        double nextBeat = _lastBeatFiredAt + BeatInterval;
        while (_accumulatedTime >= nextBeat)
        {
            _lastBeatFiredAt = nextBeat;
            OnBeat?.Invoke();
            nextBeat += BeatInterval;
        }

        // --- Half beats (double time) ---
        double nextHalf = _lastHalfBeatFiredAt + HalfBeatInterval;
        while (_accumulatedTime >= nextHalf)
        {
            _lastHalfBeatFiredAt = nextHalf;
            OnHalfBeat?.Invoke();
            nextHalf += HalfBeatInterval;
        }
    }

    // -------------------------------------------------------------------------
    // Rhythm accuracy scoring
    // -------------------------------------------------------------------------

    /// <summary>
    /// Call this from player input code to score how close to a beat the call was.
    /// </summary>
    /// <param name="useHalfBeats">
    ///   If true, measures distance against the double-time (half-beat) grid.
    ///   If false, measures against the whole-beat grid.
    /// </param>
    /// <returns>
    ///   0 — Perfect  (within ±perfectWindow seconds of the nearest beat)<br/>
    ///   1 — Good     (within ±goodWindow seconds of the nearest beat)<br/>
    ///   2 — Miss     (outside the good window)
    /// </returns>
    public int CheckInput(bool useHalfBeats = false)
    {
        double interval = useHalfBeats ? HalfBeatInterval : BeatInterval;
        double offset = _accumulatedTime % interval;

        // Fold to the nearest beat edge.
        double distanceToNearest = Math.Min(offset, interval - offset);

        if (distanceToNearest <= perfectWindow) return 0;
        if (distanceToNearest <= goodWindow) return 1;
        return 2;
    }

    // -------------------------------------------------------------------------
    // Debug helpers
    // -------------------------------------------------------------------------

    /// <summary>Returns the loop-safe accumulated playback time in seconds.</summary>
    public double GetTrackTime() => _accumulatedTime;

    /// <summary>Returns the raw FMOD timeline position in seconds.</summary>
    public double GetFmodPosition() => _lastFmodPosition;

#if UNITY_EDITOR
    private void OnValidate()
    {
        bpm = Mathf.Max(1f, bpm);
        perfectWindow = Mathf.Clamp(perfectWindow, 0f, 0.5f);
        goodWindow = Mathf.Clamp(goodWindow, perfectWindow, 0.5f);
        loopDetectionThreshold = Mathf.Max(0.05f, loopDetectionThreshold);
    }

    [ContextMenu("Debug – Log Beat Timing")]
    private void DebugLogTiming()
    {
        Debug.Log($"[BPMInteract] BPM: {bpm}  Beat: {BeatInterval:F4}s  HalfBeat: {HalfBeatInterval:F4}s  " +
                  $"Perfect: ±{perfectWindow * 1000:F0}ms  Good: ±{goodWindow * 1000:F0}ms  " +
                  $"AccumulatedTime: {_accumulatedTime:F3}s  FmodPos: {_lastFmodPosition:F3}s");
    }
#endif
}
