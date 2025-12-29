using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    NextLevel nextLevel;
    [SerializeField]PlayerMovement playerMovement;

    [Header("Dialogue Settings")]
    [SerializeField] lines[] dialogue;
    public speakerAttributes[] speakers;
    public bool transitionDialogue;
    int readNum;

    [Header("Setup")]
    [SerializeField] float delay;
    [SerializeField] GameObject dialogueCanvas;
    //[SerializeField] GameObject dialogueHUD;
    //[SerializeField] UnityEngine.UI.Image profile;
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text title;
    public bool canInteract;
    public bool active;
    bool changingLevels;
    public bool triggerOnEnter;
    public bool disableAfterUse;
    [HideInInspector] public bool disabled;



    void Start()
    {
        nextLevel = GameObject.FindFirstObjectByType<NextLevel>();
    }


    void Update()
    {
       
    }

    public void InteractWith()
    {
        
        if (changingLevels || disabled)
            return;

        if (canInteract)
        {
            active = true;
            canInteract = false;
            text.text = "";
            ReadDialogueForCommands();
            HandleDialogue();
        }
        else
        {
            Debug.Log("Skip dialogue");
            StopAllCoroutines();
            SkipRollingText();
        }
    }

    //DIALOGUE MANAGEMENT
    #region Dialogue Management
    private void HandleDialogue()
    {

        playerMovement.controlable = false;
        playerMovement.GetComponent<Rigidbody2D> ().linearVelocity = Vector2.zero;

        if (readNum >= dialogue.Length)
        {
            if (transitionDialogue)
            {
                nextLevel.LoadNextLevel();
                changingLevels = true;
            }
   
            readNum = 0;
            text.text = "";
            dialogueCanvas.SetActive(false);
            canInteract = true;
            active = false;
            playerMovement.controlable = true;

            if(disableAfterUse)
            {
                disabled = true;
            }
        }
        else
        {
            if (readNum == 0)
            {
                dialogueCanvas.SetActive(true);
            }
            StartCoroutine(RollingText(delay, SkipCommands(readNum)));
            //profile.sprite = dialogue[readNum].profileSprite;
            readNum++;
        }
    }

    IEnumerator RollingText(float delay, string line)
    {
        string display = "";
        char c;

        canInteract = false;

        for (int i = 0; i <= (line.Length - 1); i++)
        {
            c = line[i];
            display += c;

            yield return new WaitForSecondsRealtime(delay);

            text.text = display;
        }

        canInteract = true;
    }

    private void SkipRollingText()
    {
        StopAllCoroutines();
        text.text = SkipCommands(readNum -1);
        canInteract = true;
    }

    private void ReadDialogueForCommands()
    {
        if(readNum >= dialogue.Length)
        {
            return;
        }
        else
        {
            string[] words = dialogue[readNum].text.Split(' ');

            foreach (string word in words)
            {
                if (word == "/")
                {
                    break;
                }
                else
                {
                    foreach (speakerAttributes speaker in speakers)
                    {
                        if (speaker.name == word)
                        {
                            title.text = speaker.name;
                            text.color = speaker.textColor;
                        }
                    }
                }
            }
        }          
    }

    private string SkipCommands(int i)
    {
        string actualLine = dialogue[i].text.Substring(dialogue[i].text.IndexOf('/') + 1);       
        return actualLine;
    }
    #endregion

    //COLLISION HANDLING
    #region Collision Handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(disabled)
            return;

        canInteract = true;
        text.text = "";

        if(triggerOnEnter)
        {
            InteractWith();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(disabled)
            return;

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
        canInteract = false;
        StopAllCoroutines();
        readNum = 0;
    }
    #endregion

    //STRUCTS
    #region Structs
    [Serializable]
    public struct speakerAttributes
    {
        public string name;
        public Color textColor;
        
    }

    [Serializable]
    public struct lines
    {
        public string text;
        public Sprite profileSprite;
    }
    #endregion
}
