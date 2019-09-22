using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public Text DialogText;
    public GameObject OneResponseOption;
    public GameObject TwoResponseOptions;
    public GameObject ThreeResponseOptions;
    public GameObject Highlight;

    private DialogNode CurrentNode;

    // References to the Text objects that make up the response options
    private Text responseOneOne;
    private Text responseTwoOne;
    private Text responseTwoTwo;
    private Text responseThreeOne;
    private Text responseThreeTwo;
    private Text responseThreeThree;

    private readonly float[][] highlightVerticalPositions = new float[][] {
        new float[] { -0.02f },
        new float[] { 0.72f, -0.4f },
        new float[] { 0.73f, -0.02f, -0.77f }
    };

    private int numResponses = 1;   // 1, 2, or 3

    private int currentSelection = 0;   // index of response that is currently highlighted

    private bool canListenForResponse = false;  // true when a user can submit a dialog response, false when the initial dialog is displayed

    private bool endOfConversation = false; // ends the converation

    private DialogNode[] DialogGraph;

    private GameObject Speaker;

    private bool isHidden = false;  // initialize to false so the HideDialogWindow call on start actually hides the window

    private void Start()
    {
        Instance = this;    // save singleton reference
        // Get references to the Text objects for the responses
        responseOneOne = OneResponseOption.transform.GetChild(0).gameObject.GetComponent<Text>();
        responseTwoOne = TwoResponseOptions.transform.GetChild(0).gameObject.GetComponent<Text>();
        responseTwoTwo = TwoResponseOptions.transform.GetChild(1).gameObject.GetComponent<Text>();
        responseThreeOne = ThreeResponseOptions.transform.GetChild(0).gameObject.GetComponent<Text>();
        responseThreeTwo = ThreeResponseOptions.transform.GetChild(1).gameObject.GetComponent<Text>();
        responseThreeThree = ThreeResponseOptions.transform.GetChild(2).gameObject.GetComponent<Text>();
        HideDialogWindow();
    }

    private void Update()
    {
        if (canListenForResponse)
        {
            ListenForResponse();
        }
        else
        {
            DisplayCharacterDialog();
        }
    } // end Update method

    /// <summary>
    /// Starts the conversation.
    /// </summary>
    /// <param name="Conversation">The dialog graph that will be interpreted.</param>
    public void StartConversation(DialogNode[] Conversation, GameObject Character)
    {
        this.Speaker = Character;
        this.DialogGraph = Conversation;
        UpdateDialog(this.DialogGraph[0]);
        // reset conversation parameters
        currentSelection = 0;
        endOfConversation = false;
        ShowDialogWindow();
    }

    /// <summary>
    /// Updates the dialog UI with data from a DialogNode instance.
    /// </summary>
    /// <param name="Dialog">Dialog.</param>
    public void UpdateDialog(DialogNode Dialog)
    {
        canListenForResponse = false;
        CurrentNode = Dialog;
        Highlight.SetActive(false);
        OneResponseOption.SetActive(false);
        TwoResponseOptions.SetActive(false);
        ThreeResponseOptions.SetActive(false);
        DialogText.gameObject.SetActive(true);
        if (CurrentNode.responses != null)
        {
            numResponses = CurrentNode.responses.Count;
        }
        else
        {
            endOfConversation = true;
        }
        DialogText.text = CurrentNode.sentence;
    }

    private void ListenForResponse()
    {
        // Navigate through dialog options with the arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelection--;
            if (currentSelection < 0)
            {
                currentSelection = numResponses - 1;
            }
            UpdateHighlightPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection++;
            if (currentSelection == numResponses)
            {
                currentSelection = 0;
            }
            UpdateHighlightPosition();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateDialog(DialogGraph[CurrentNode.responses[currentSelection].nextSentence]);
        }
    } // end ListenForResponse method

    // Shows the character's dialog, and prepares text responses for user
    private void DisplayCharacterDialog()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (endOfConversation)
            {
                HideDialogWindow();
            }
            else
            {
                // reset selection to the min number of responses minus one
                currentSelection = 0;
                // Populate the response text fields
                Highlight.SetActive(true);
                canListenForResponse = true;
                // Set the text for the responses
                switch (numResponses)
                {
                    case 1:
                        OneResponseOption.SetActive(true);
                        TwoResponseOptions.SetActive(false);
                        ThreeResponseOptions.SetActive(false);
                        responseOneOne.text = CurrentNode.responses[0].response;
                        break;
                    case 2:
                        OneResponseOption.SetActive(false);
                        TwoResponseOptions.SetActive(true);
                        ThreeResponseOptions.SetActive(false);
                        responseTwoOne.text = CurrentNode.responses[0].response;
                        responseTwoTwo.text = CurrentNode.responses[1].response;
                        break;
                    case 3:
                        OneResponseOption.SetActive(false);
                        TwoResponseOptions.SetActive(false);
                        ThreeResponseOptions.SetActive(true);
                        responseThreeOne.text = CurrentNode.responses[0].response;
                        responseThreeTwo.text = CurrentNode.responses[1].response;
                        responseThreeThree.text = CurrentNode.responses[2].response;
                        break;
                }
                UpdateHighlightPosition();
                DialogText.gameObject.SetActive(false);
            }
        }
    } // end DisplayCharacterDialog method

    // Moves the text response highlight to position next to the current selection
    private void UpdateHighlightPosition()
    {
        Debug.Log("currentsel " + currentSelection);
        Highlight.transform.localPosition = new Vector3(-1.92f, highlightVerticalPositions[numResponses - 1][currentSelection], -0.1f);
    }

    public void HideDialogWindow()
    {
        if (!isHidden)
        {
            transform.position -= new Vector3(0, 0, 100);
            isHidden = true;
        }
    }

    public void ShowDialogWindow()
    {
        if (isHidden)
        {
            transform.position += new Vector3(0, 0, 100);
            isHidden = false;
        }
    }

    /*
     Sample dialog graph:
     // Hardcoded Dialog Graph representening all possible dialog structures
    private readonly DialogNode[] DialogGraph =
    {
        new DialogNode("", new string[]{
                "",
                "" },
            new int[]{ 1, 1 }),
        new DialogNode("", new string[]{
                "",
                "",
                "" },
            new int[]{ 2, 3, 4 }),
        new DialogNode("", new string[]{
                "" },
            new int[]{ 5 }),
        new DialogNode("", null, null),
        new DialogNode("", null, null),
        new DialogNode("", null, null)
    };   
     */

} // end DialogManager class
