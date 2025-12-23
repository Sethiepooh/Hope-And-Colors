using UnityEngine;
using UnityEngine.InputSystem;

public class CallAndResponse : MonoBehaviour
{
    public Sprite[] directionSprite; // 0: Up, 1: Down, 2: Left, 3: Right
    public RhythmPattern[] rhythmPatterns;
    public PlayerInput playerInput;

    InputDirection lastInputDirection;
    int currentPatternIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rhythmPatterns[currentPatternIndex].Initialize(this);
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    public void OnRhythmInput(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        if (context.performed)
        {
            if (dir == Vector2.up)
            {
                lastInputDirection = InputDirection.Up;
            }
            else if (dir == Vector2.down)
            {
                lastInputDirection = InputDirection.Down;
            }
            else if (dir == Vector2.left)
            {
                lastInputDirection = InputDirection.Left;
            }
            else if (dir == Vector2.right)
            {
                lastInputDirection = InputDirection.Right;
            }

            rhythmPatterns[currentPatternIndex].CheckHitBeat(lastInputDirection);
        }
        else
        {
            lastInputDirection = InputDirection.None;
        }

    }

    public void AddToCurrentPatternIndex()
    {
        currentPatternIndex++;


        if (currentPatternIndex >= rhythmPatterns.Length)
        {
            Debug.Log("Challenge Complete");
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void AddBeatToCurrentPattern()
    {
        rhythmPatterns[currentPatternIndex].AddBeat();
    }
}

public enum InputDirection {None, Up, Down, Left, Right}

[System.Serializable]
public struct RhythmInput
{
    public int index;

    [Header("Activation & Input Timing")]
    public int revealBeat;
    public int activationBeat;
    public int expectedInputBeat;
    int expirationBeat;
    public InputDirection expectedInput;
    public bool isActive;
    public bool revealed;
    public bool hit;

    public void Initialize()
    {
        expirationBeat = expectedInputBeat + 1;
        isActive = false;
        revealed = false;
    }

    public void SetHit(bool status)
    {
        hit = status;
    }

    public void SetIsActive(bool status)
    {
        isActive = status;
    }

    public void SetRevealed(bool status)
    {
        revealed = status;
    }

    public void ResetInput()
    {
        isActive = false;
        revealed = false;
        hit = false;
    }

    public bool CheckPerfectHit(int currentBeat)
    {
        if(currentBeat == expectedInputBeat)
        {
            return true;
        }

        return false;
    }

    public void CheckReveal(int currentBeat, RhythmPattern rhythmPattern)
    {
        if (currentBeat == revealBeat)
        {
            SetRevealed(true);
            rhythmPattern.SetInputPromptImage(index, expectedInput);
        }
    }

    public void CheckActivate(int currentBeat, RhythmPattern rhythmPattern)
    {
        if (currentBeat == activationBeat)
        {
            SetIsActive(true);
            rhythmPattern.SetInputPromptActive(index);
        }
    }

    public void CheckExpiration(int currentBeat, RhythmPattern rhythmPattern)
    {
        if(hit)
            return;
        if (currentBeat == expirationBeat)
        {
            SetIsActive(false);
            rhythmPattern.SetInputPromptExpired(index);
        }
    }
}

[System.Serializable]
public struct RhythmPattern
{
    int finalBeat;

    //Hit Tracking
    public int beatsToHit;
    public int successfulHits;

    int currentBeat;
    int nextExpectedInputIndex;
    CallAndResponse parentScript;
    public RhythmInput[] pattern;
    public GameObject[] inputPromptObjects; 

    public void Initialize(CallAndResponse callAndResponse)
    {
        parentScript = callAndResponse;
        successfulHits = 0;

        for (int i = 0; i < pattern.Length; i++)
        {
            beatsToHit++;
            pattern[i].index = i;
            pattern[i].Initialize();
        }

        finalBeat = pattern[pattern.Length - 1].expectedInputBeat + 2;
    }

    public void AddBeat()
    {
        currentBeat++;
        if (currentBeat == finalBeat)
        {
            if (successfulHits == beatsToHit)
            {
                Debug.Log("Pattern Complete!");
                parentScript.AddBeatToCurrentPattern();
            }
            else
            {
                nextExpectedInputIndex = 0;
                currentBeat = 0;
                successfulHits = 0;
            }

            for (int i = 0; i < pattern.Length; i++)
            {
                pattern[i].ResetInput();
            }

            for (int i = 0; i < inputPromptObjects.Length; i++)
            {
                inputPromptObjects[i].GetComponent<SpriteRenderer>().color = Color.white;
                inputPromptObjects[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i].isActive)
                {
                    pattern[i].CheckExpiration(currentBeat, this);
                }
                else
                {
                    if (!pattern[i].revealed)
                    {
                        pattern[i].CheckReveal(currentBeat, this);
                        Debug.Log("Checking Reveal for Input Index: " + i);
                    }
                    else
                        pattern[i].CheckActivate(currentBeat, this);
                }
            }
        }
            

    }

    public void SetInputPromptImage(int rhythmInputIndex, InputDirection dir)
    {
        inputPromptObjects[rhythmInputIndex].SetActive(true);
        SpriteRenderer sRend = inputPromptObjects[rhythmInputIndex].GetComponent<SpriteRenderer>();
        switch (dir)
        {
            case InputDirection.Up:
                sRend.sprite = parentScript.directionSprite[0];
                break;
            case InputDirection.Down:
                sRend.sprite = parentScript.directionSprite[1];
                break;
            case InputDirection.Left:
                sRend.sprite = parentScript.directionSprite[2];
                break;
            case InputDirection.Right:
                sRend.sprite = parentScript.directionSprite[3];
                break;
            default:
                break;
        }
    }

    public void SetInputPromptActive(int rhythmInputIndex)
    {
        inputPromptObjects[rhythmInputIndex].GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void SetInputPromptExpired(int rhythmInputIndex)
    {
        inputPromptObjects[rhythmInputIndex].GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void CheckHitBeat(InputDirection inputDirection)
    {
        if (nextExpectedInputIndex >= pattern.Length)
            return;
        RhythmInput expectedInput = pattern[nextExpectedInputIndex];
        //Debug.Log("Expected Input: " + expectedInput.expectedInput + " | Player Input: " + inputDirection + " | Is Active: " + expectedInput.isActive);
        if (expectedInput.isActive && inputDirection == expectedInput.expectedInput)
        {
            if(expectedInput.CheckPerfectHit(currentBeat))
                Debug.Log("Perfect Hit!");
            else
                Debug.Log("Hit!");

            inputPromptObjects[expectedInput.index].GetComponent<SpriteRenderer>().color = Color.green;
            pattern[nextExpectedInputIndex].SetIsActive(false);
            pattern[nextExpectedInputIndex].SetHit(true);
            successfulHits++;
            nextExpectedInputIndex++;
        }
        else if (expectedInput.isActive && inputDirection != expectedInput.expectedInput)
        {
            Debug.Log("Miss!");
            inputPromptObjects[expectedInput.index].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}