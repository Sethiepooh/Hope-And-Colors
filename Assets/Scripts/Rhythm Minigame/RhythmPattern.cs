using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RhythmPattern
{
    [SerializeField] RhythmPatternBeat[] beatsToHit;
    List<int> beatTimings = new List<int>();
    int currentBeatIndex = 0;

    [HideInInspector] public float patternDuration;
    int successfulBeatsHit;
    int earlyfulBeatsHit;

    public void InitializePattern()
    {
        int currentBeatTiming = 0;

        foreach (RhythmPatternBeat beat in beatsToHit)
        {
            beatTimings.Add(currentBeatTiming);
            if (beat.beatType == BeatTypeEnum.BeatType.Whole)
            {
                currentBeatTiming += 8;
            }
            else if(beat.beatType == BeatTypeEnum.BeatType.Quarter)
            {
                currentBeatTiming += 2;
            }
            else if (beat.beatType == BeatTypeEnum.BeatType.Eighth)
            {
                currentBeatTiming += 1;
            }
            else if (beat.beatType == BeatTypeEnum.BeatType.DottedQuarter)
            {
                currentBeatTiming += 3;
            }
        }
    }

    public float CalculateCompletionPercent()
    {
        float totalPoints = 0;

        totalPoints += earlyfulBeatsHit;
        totalPoints += successfulBeatsHit * 2;

        float totalPossiblePoints = beatsToHit.Length * 2;

        return totalPoints / totalPossiblePoints;
    }

    public void CalculatePatternDuration(float BPM)
    {
        int totalBeats = beatTimings[beatTimings.Count - 1] + 2; //Add 2 to account for the last beat being a whole note, and the pattern ending on the next whole note after that


        Debug.Log("Total Beats in Pattern: " + totalBeats);
        float secondsPerBeat = 60f / (BPM * 2);
        patternDuration = totalBeats * secondsPerBeat;
    }

    public RhythmPatternBeat GetNextBeat(int beatIndex)
    {
        if (beatIndex <= beatTimings[beatTimings.Count - 1])
        {
            if(beatIndex == beatTimings[currentBeatIndex])
            {
                currentBeatIndex++;
                return beatsToHit[currentBeatIndex - 1];
            }
            return null;
        }
        else
        {
            return null;
        }
    }

    public RhythmPatternBeat GetCurrentBeat(int beatIndex)
    {
        return beatsToHit[beatIndex];
    }

    public void AddSuccessfulHit()
    {
        successfulBeatsHit++;
        Debug.Log("Successful Beats Hit: " + successfulBeatsHit);
    }

    public void AddEarlyfulHit()
    {
        earlyfulBeatsHit++;
        Debug.Log("Early Beats Hit: " + earlyfulBeatsHit);
    }
}
