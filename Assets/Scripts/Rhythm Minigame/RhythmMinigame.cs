using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using FMODUnity;

public class RhythmMinigame : MonoBehaviour, IInteractable
{
    [SerializeField] bool _interactable;
    public bool interactable { get => _interactable; set => _interactable = value; }
    public bool activeInteraction { get; set; }
    public Vector2 position { get; }

    int patternIndex = -1;
    int nextNote = 0;
    float totalCompletionPercent;

    [Header("Minigame Settings")]
    [SerializeField] float BPM;
    [SerializeField] Transform inputPromptCenter;
    [SerializeField] GameObject beatTimerDisplay;
    [SerializeField] GameObject inputPromptDisplay;
    [SerializeField] Sprite[] inputPromptSprites;
    //0 = up, 1 = down, 2 = right, 3 = left,
    //4 = up eigth, 5 = down eigth, 6 = right eigth, 7 = left eigth
    //8 = up dotted, 9 = down dotted, 10 = right dotted, 11 = left dotted
    //12 = up whole, 13 = down whole, 14 = right whole, 15 = left whole

    [SerializeField] float beatSpacing = 1f;
    [SerializeField] InteractionManager interactionManager;
    [SerializeField] PlayerInput playerInput;

    [Header("Rhythm Patterns")]
    [SerializeField] RhythmPattern[] patternsInSequence;
    List<GameObject> displayedBeats = new List<GameObject>();
    List<GameObject> displayedBeatTimers = new List<GameObject>();
    List<float> beatInputTimings = new List<float>();
    List<Coroutine> activeCoroutines = new List<Coroutine>();
    int beatCount = 0;
    int eigthNoteIndex = 0;
    bool waitingForDownbeat = false;

    [Header("Rhythm Pattern Events")]
    public UnityEvent onBeatHit;
    public UnityEvent onBeatEarly;
    public UnityEvent onBeatMissed;
    public UnityEvent onPatternCompleted;
    public UnityEvent OnMinigameCompleted;
    public UnityEvent OnMinigameFailed;

    [Header("SFX")]
    [SerializeField] EventReference _beatSpawnSfx;
    [SerializeField] EventReference _beatHitSfx;
    [SerializeField] EventReference _beatEarlySfx;
    [SerializeField] EventReference _beatMissSfx;

    private void Start()
    {
        foreach(RhythmPattern pattern in patternsInSequence)
        {
            onBeatHit.AddListener(pattern.AddSuccessfulHit);
            onBeatEarly.AddListener(pattern.AddEarlyfulHit);
            pattern.InitializePattern();
            pattern.CalculatePatternDuration(BPM);
        }
    }

    public void OnInteract()
    {
        activeInteraction = true;
        waitingForDownbeat = true;
        playerInput.SwitchCurrentActionMap("CallResponse");
    }

    public void OnEventInteract()
    {
        interactionManager.nearbyInteractable = this;
        activeInteraction = true;
        waitingForDownbeat = true;
        playerInput.SwitchCurrentActionMap("CallResponse");
    }

    public void OnRhythmInput()
    {
        if(!activeInteraction) return;

        if(interactionManager.nearbyInteractable != this) return;

        if (interactionManager.lastInputDirection == InputDirectionEnum.InputDirection.None) return;
        if (patternIndex < 0) return;

        if (interactionManager.lastInputDirection == patternsInSequence[patternIndex].GetCurrentBeat(nextNote).direction)
        {
            if(beatInputTimings[nextNote] < (patternsInSequence[patternIndex].patternDuration / 2))
            {
                //Debug.Log("Beat Hit! " + beatInputTimings[nextNote]);
                onBeatMissed.Invoke();
                if (!_beatMissSfx.IsNull) RuntimeManager.PlayOneShot(_beatMissSfx);
            }
            else if(beatInputTimings[nextNote] < ((patternsInSequence[patternIndex].patternDuration / 4) * 3))
            {
                //Debug.Log("Beat Hit! " + beatInputTimings[nextNote]);
                onBeatEarly.Invoke();
                if (!_beatEarlySfx.IsNull) RuntimeManager.PlayOneShot(_beatEarlySfx);
            }
            else
            {
                Debug.Log("Beat Hit! " + beatInputTimings[nextNote]);
                onBeatHit.Invoke();
                if (!_beatHitSfx.IsNull) RuntimeManager.PlayOneShot(_beatHitSfx);
            }
        }
        else
        {
            onBeatMissed.Invoke();
            if (!_beatMissSfx.IsNull) RuntimeManager.PlayOneShot(_beatMissSfx);
        }
        StopCoroutine(activeCoroutines[nextNote]);
        NextNote();
    }

    void NextNote()
    {
        nextNote++;
        if (nextNote >= beatInputTimings.Count)
        {
            if (patternIndex >= patternsInSequence.Length - 1)
            {
                if (ValidateTotalCompletion())
                {
                    Debug.Log("Minigame Completed!");
                    OnMinigameCompleted.Invoke();
                }
                else
                {
                    Debug.Log("Minigame Failed");
                    OnMinigameFailed.Invoke();
                }
                EjectFromMinigame();
            }
            else
            {
                NextPattern();

            }
        }
    }

    bool ValidateTotalCompletion()
    {
        float totalPercent = 0;
        foreach (RhythmPattern pattern in patternsInSequence)
        {
            totalPercent += pattern.CalculateCompletionPercent();
        }
        totalCompletionPercent = totalPercent / patternsInSequence.Length;

        return totalCompletionPercent >= .5f;
    }

    void EjectFromMinigame()
    {
        ClearDisplayedBeats();
        activeInteraction = false;
        playerInput.SwitchCurrentActionMap("Player");
        beatCount = 0;
        patternIndex = -1;
        eigthNoteIndex = 0;
    }

    public void NextPattern()
    {
        if (patternIndex != -1)
        {
            ClearDisplayedBeats();
        }

        patternIndex++;
        beatCount = 0;
    }

    public void AddBeatToRhythmPattern()
    { 
        eigthNoteIndex++;

        if (!activeInteraction) return;

        if (waitingForDownbeat)
        {
            if (eigthNoteIndex % 2 == 0)
            {
                waitingForDownbeat = false;
                NextPattern();  
            }
        }
        else
        {
            if (patternIndex < 0) return;
            RhythmPatternBeat nextBeat = patternsInSequence[patternIndex].GetNextBeat(beatCount);
            if (nextBeat != null)
            {
                AddToDisplayedBeats(nextBeat);
            }

            beatCount++;
        }           
    }

    public void ClearDisplayedBeats()
    {
        foreach (GameObject beat in displayedBeats)
        {
            GameObject.Destroy(beat);
        }
        displayedBeats.Clear();
        displayedBeatTimers.Clear();
        beatInputTimings.Clear();
        activeCoroutines.Clear();
        nextNote = 0;
    }

    public void AddToDisplayedBeats(RhythmPatternBeat beat)
    {
        Debug.Log("Adding Beat: " + beat.beatType + " " + beat.direction);

        GameObject newBeat = Instantiate(inputPromptDisplay, inputPromptCenter.position, Quaternion.identity, inputPromptCenter);
        GameObject newBeatTimer = Instantiate(beatTimerDisplay, inputPromptCenter.position, Quaternion.identity, newBeat.transform);

        //Debug.Log(beat);
        newBeat.GetComponent<SpriteRenderer>().sprite = GetBeatSprite(beat);

        displayedBeats.Add(newBeat);
        displayedBeatTimers.Add(newBeatTimer);
        beatInputTimings.Add(patternsInSequence[patternIndex].patternDuration);
        activeCoroutines.Add(StartCoroutine(HandleTimer(Vector3.one, beatInputTimings.Count - 1, newBeatTimer)));
        if (!_beatSpawnSfx.IsNull) RuntimeManager.PlayOneShot(_beatSpawnSfx);
        RecenterBeats();
    }

    Sprite GetBeatSprite(RhythmPatternBeat beat)
    {
        if(beat.beatType == BeatTypeEnum.BeatType.Whole)
        {
            return inputPromptSprites[(int)beat.direction + 12];
        }
        else if (beat.beatType == BeatTypeEnum.BeatType.Quarter)
        {
            return inputPromptSprites[(int)beat.direction];
        }
        else if (beat.beatType == BeatTypeEnum.BeatType.Eighth)
        {
            return inputPromptSprites[(int)beat.direction + 4];
        }
        else if (beat.beatType == BeatTypeEnum.BeatType.DottedQuarter)
        {
            return inputPromptSprites[(int)beat.direction + 8];
        }
        return null;
    }

    void RecenterBeats()
    {
        if (displayedBeats.Count == 0) return;

        float totalWidth = (displayedBeats.Count - 1) * beatSpacing;
        float startX = inputPromptCenter.position.x - totalWidth / 2f;

        for (int i = 0; i < displayedBeats.Count; i++)
        {
            Vector3 newPosition = new Vector3(
                startX + i * beatSpacing,
                inputPromptCenter.position.y,
                inputPromptCenter.position.z
            );

            displayedBeats[i].transform.position = newPosition;
            // BeatTimers are children of their beat, so they move with it.
            // Only reposition if they need an offset from the beat itself:
            displayedBeatTimers[i].transform.localPosition = Vector3.zero;
        }
    }

    IEnumerator HandleTimer(Vector3 targetScale, int timerIndex, GameObject obj)
    {
        //Debug.Log(beatInputTimings[timerIndex]);
        Vector3 startScale = obj.transform.localScale;
        float duration = beatInputTimings[timerIndex];
        beatInputTimings[timerIndex] = 0;

        while (beatInputTimings[timerIndex] < duration)
        {
            float t = beatInputTimings[timerIndex] / duration; // Normalized time from 0 to 1
            obj.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            beatInputTimings[timerIndex] += Time.deltaTime;
            yield return null;
        }
        // Ensure final value is exact
        obj.transform.localScale = targetScale;
        onBeatMissed.Invoke();
        if (!_beatMissSfx.IsNull) RuntimeManager.PlayOneShot(_beatMissSfx);
    }

    public void MissedBeat()
    {
        Debug.Log("Beat Missed!");  
        NextNote();
    }
}
