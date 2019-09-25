using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogEditorWindow : EditorWindow
{
    private static readonly int MAX_HEIGHT = 600;
    private static readonly int MAX_WIDTH = 800;
    private static readonly int MIN_HEIGHT = 600;
    private static readonly int MIN_WIDTH = 800;

    private readonly int START_NODE_INDEX = 0;
    private readonly int END_NODE_INDEX = 1;
    
private static readonly Vector2 backgroundPosition = new Vector2(20, 45);

    private static readonly int nodeWidth = 80;
    private static readonly int nodeHeight = 50;

    private string currentSentence = "";
    private List<DialogResponse> currentResponses;

    private int selectedNodeIndex = -1;    

    private List<WindowDialogNode> nodes = new List<WindowDialogNode>();

    private bool draggingNodeLink = false;
    private int draggingFromIndex = -1;

    private readonly float arrowheadHeight = 10f;

    private readonly int deleteConBtnSize = 20;

    private Vector2 scrollPosition = new Vector2(0, 0);

    GUIStyle deleteConnectionButton = new GUIStyle();
    
    private Conversable selectedCharacter;  // Character's dialog that is being viewed in the editor window
    
    // Allow editor window to open when link clicked in Window -> Dialog Editor in menu
    [MenuItem("Window/Dialog Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogEditorWindow));
    }


    /// <summary>
    /// Opens the dialog editor window and sets the current list of nodes to the specified character's dialog graph.
    /// </summary>
    /// <param name="character">Character with a conversable component.</param>
    public void ShowWindowForCharacter(Conversable character)
    {
        if (character != null)
        {
            this.nodes = character.GraphGUI;
            this.selectedCharacter = character;
        }
        GetWindow(typeof(DialogEditorWindow));
    }

    // The actual window code goes here
    private void OnGUI()
    {
        // Set custom textures:
        Texture2D redBackground = new Texture2D(deleteConBtnSize, deleteConBtnSize);
        for (int i = 0; i < deleteConBtnSize; i++)
        {
            for (int j = 0; j < deleteConBtnSize; j++)
            {
                redBackground.SetPixel(j, i, new Color((180 + (2 * i)) / 255f, 0, 0));
            }
        }
        redBackground.Apply();
        Texture2D greenBackground = new Texture2D(deleteConBtnSize, deleteConBtnSize);
        for (int i = 0; i < deleteConBtnSize; i++)
        {
            for (int j = 0; j < deleteConBtnSize; j++)
            {
                greenBackground.SetPixel(j, i, new Color(0, (170 + (2 * i)) / 255f, 0));
            }
        }
        greenBackground.Apply();

        // Set styles for the delete node connection button
        deleteConnectionButton.fontSize = 13;
        deleteConnectionButton.alignment = TextAnchor.MiddleCenter;
        deleteConnectionButton.normal.textColor = Color.white;
        deleteConnectionButton.normal.background = redBackground;
        deleteConnectionButton.border = new RectOffset(10,10,10,10);

        // Set styles for start button
        GUIStyle startNode = new GUIStyle("startBtn");
        startNode.fontSize = 13;
        startNode.alignment = TextAnchor.MiddleCenter;
        startNode.normal.textColor = Color.white;
        startNode.normal.background = greenBackground;
        startNode.border = new RectOffset(5, 5, 5, 5);


        // Set styles for the end button
        GUIStyle endNode = new GUIStyle("endBtn");
        endNode.fontSize = 13;
        endNode.alignment = TextAnchor.MiddleCenter;
        endNode.normal.textColor = Color.white;
        endNode.normal.background = redBackground;


        // Lock sizes of window
        this.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
        this.maxSize = new Vector2(MAX_WIDTH, MAX_HEIGHT);

        Vector2 backgroundSize = new Vector2(position.width * 0.95f, position.height * 0.75f);

        // Draw editor title
        GUILayout.Label("Dialog Editor", EditorStyles.boldLabel);
        if (this.selectedCharacter != null)
        {
            GUILayout.Label("Editing dialog for " + this.selectedCharacter.name, EditorStyles.centeredGreyMiniLabel);
        }

        // Draw the dialog tree background
        Rect backgroundRect = new Rect(backgroundPosition.x, backgroundPosition.y, backgroundSize.x, backgroundSize.y);
        EditorGUI.DrawRect(backgroundRect, new Color(0.6f,0.6f,0.6f));

        // Add a new dialog node on button click
        GUIStyle newNodeButton = new GUIStyle("button");
        newNodeButton.fontSize = 20;
        newNodeButton.alignment = TextAnchor.UpperCenter;
        newNodeButton.fontStyle = FontStyle.Bold;
        if (GUI.Button(new Rect(position.width * 0.92f, 10, 30, 30), "+", newNodeButton))
        {
           DialogNode content = new DialogNode("Write the dialog prompt here.");
           Vector2 nodePosition = new Vector2(Random.Range(backgroundPosition.x, backgroundSize.x - nodeWidth / 2), Random.Range(backgroundPosition.y, backgroundSize.y - nodeHeight / 2));
           nodes.Add(new WindowDialogNode(nodePosition, content));
        }

        if (selectedNodeIndex != -1 && selectedNodeIndex != START_NODE_INDEX && selectedNodeIndex != END_NODE_INDEX)
        {
            if (GUI.Button(new Rect(position.width * 0.86f, 10, 30, 30), "X", deleteConnectionButton))
            {
                Debug.Log("Trying to delete node at index " + selectedNodeIndex);
                nodes.RemoveAt(selectedNodeIndex);
            }
        }

        // If no nodes have been added to the graph, add a start node and end node
        if (this.nodes.Count == 0)
        {
            // Add the green start node:
            DialogNode startContent = new DialogNode("Start Node");
            Vector2 startNodePosition = new Vector2(Random.Range(backgroundPosition.x, backgroundSize.x - nodeWidth / 2), Random.Range(backgroundPosition.y, backgroundSize.y - nodeHeight / 2));
            nodes.Add(new WindowDialogNode(startNodePosition, startContent));
            // Add a red end node:
            DialogNode endContent = new DialogNode("End Node");
            Vector2 endNodePosition = new Vector2(Random.Range(backgroundPosition.x, backgroundSize.x - nodeWidth / 2), Random.Range(backgroundPosition.y, backgroundSize.y - nodeHeight / 2));
            nodes.Add(new WindowDialogNode(endNodePosition, endContent));
        }



        //Draw all the nodes in the graph, including the arrows represeting the traversable paths of dialog.
        BeginWindows();
        for (int i = 0; i < nodes.Count; i++)
        {
            // If the node is not a start node or the end node, we still need to set its Rect
            if (i == START_NODE_INDEX) // Index 0 of nodes is always the start node
            {
                nodes[START_NODE_INDEX].Rect = GUI.Window(START_NODE_INDEX, nodes[START_NODE_INDEX].Rect, DrawNodeWindow, "Start", startNode);
            }
            else if (i == END_NODE_INDEX)    // Index 1 of nodes is always the end node
            {
                nodes[END_NODE_INDEX].Rect = GUI.Window(END_NODE_INDEX, nodes[END_NODE_INDEX].Rect, DrawNodeWindow, "End", endNode);
            }
            else // Every index greater than 1 holds all the dialog nodes
            {
                nodes[i].Rect = GUI.Window(i, nodes[i].Rect, DrawNodeWindow, "Node " + (i - 1));
            }


            // Handles graph interaction. Also draws connections between nodes
            Event current = Event.current;
           if (nodes[i].Rect.Contains(current.mousePosition))
           {
               // If node is right-clicked, start creating a connection to a new node
               if (current.type == EventType.ContextClick)
               {
                   // If flag is true, then the user is trying to form the tail of the node connection
                   if (draggingNodeLink)
                   {
                       // If this connection has not already been formed, then form it
                       bool canCreateConnection = true;
                       for (int j = 0; j < nodes[draggingFromIndex].content.responses.Count; j++)
                       {
                           if (nodes[draggingFromIndex].content.responses[j].nextSentence == i)
                           {
                               canCreateConnection = false;
                               break;
                           }
                       }
                       if (canCreateConnection)
                       {
                            DialogResponse response = new DialogResponse("Write a response to the prompt here.", i);
                            nodes[draggingFromIndex].content.responses.Add(response);
                        }
                        draggingNodeLink = false;
                  }
                   // If the flag is false, then the user is trying to start with the head (beginning) of the connection
                   else
                   {
                        int numResponses = nodes[START_NODE_INDEX].content.responses.Count;
                        // Don't allow dialog paths to be created originating from the end node
                        if (i != END_NODE_INDEX)
                        {
                            draggingFromIndex = i;
                            draggingNodeLink = true;
                        }
                   }
                   current.Use();
               }
               // If you left click on a node, display the node's info at the bottom of the gui
               else if (current.type == EventType.Layout)
               {
                    selectedNodeIndex = i;
                }
           }

           // Draw connections to neighbouring nodes
           Handles.color = Color.cyan;
           Vector2 currentNodePosition2d = nodes[i].Rect.position;
           Vector3 currentNodePosition = new Vector3(currentNodePosition2d.x + WindowDialogNode.width / 2, currentNodePosition2d.y + WindowDialogNode.height / 2, 0);
           if (nodes[i].content != null)
           {
               for (int j = 0; j < nodes[i].content.responses.Count; j++)
               {
                   // Get the index for the target node from the nextSentence variable of the j-th response of this node
                   int targetNodeIndex = nodes[i].content.responses[j].nextSentence;
                   Vector2 targetNodePosition2d = nodes[targetNodeIndex].Rect.position;
                   Vector3 targetNodePosition = new Vector3(targetNodePosition2d.x + WindowDialogNode.width / 2, targetNodePosition2d.y + WindowDialogNode.height / 2, 0);
                   Handles.DrawLine(currentNodePosition, targetNodePosition);


                   // Draw the arrow head:
                   Vector3 dirUnitVector = (targetNodePosition - currentNodePosition).normalized;
                   Vector3 perpendicularDirUnitVector = new Vector3(-dirUnitVector.y, dirUnitVector.x, 0).normalized;
                   Vector3 triangleHeight = dirUnitVector * arrowheadHeight;
                   targetNodePosition = 0.5f * (targetNodePosition + currentNodePosition);
                   Vector3 triangleBase = new Vector3(targetNodePosition.x - triangleHeight.x, targetNodePosition.y - triangleHeight.y, 0);
                   Vector3[] vertices = {
                       targetNodePosition, 
                       new Vector3(triangleBase.x + ((arrowheadHeight / 2) * perpendicularDirUnitVector.x), triangleBase.y + ((arrowheadHeight / 2) * perpendicularDirUnitVector.y), 0),
                       new Vector3(triangleBase.x - ((arrowheadHeight / 2) * perpendicularDirUnitVector.x), triangleBase.y - ((arrowheadHeight / 2) * perpendicularDirUnitVector.y), 0),};
                   Handles.color = Color.cyan;
                   Handles.DrawAAConvexPolygon(vertices);
               }
           }
        }
        EndWindows();

        // Draws a line from selected node to the cursor position when creating a new node connection in the graph
        if (draggingNodeLink)
        {
           Vector2 mousePos = Event.current.mousePosition;
           Vector2 nodePos = nodes[draggingFromIndex].Rect.position;
           Handles.color = Color.blue;
           Handles.DrawLine(new Vector3(mousePos.x, mousePos.y, 0), new Vector3(nodePos.x + WindowDialogNode.width / 2, nodePos.y + WindowDialogNode.height / 2, 0));
           Repaint();
        }

        // End node connection attempt if second left click not over a node
        Event secondEvent = Event.current;
        if (secondEvent.type == EventType.ContextClick)
        {
           draggingNodeLink = false;
           secondEvent.Use();
        }

        // If a node is selected in the graph, display its contents at the bottom of the gui window.
        if (selectedNodeIndex != -1)   
        {
            DisplayNodeInfo();
        }

        // Label to say which node is currently highlighted in the dialog graph
        GUI.Label(new Rect(660, position.height * 0.86f, 100, 70), "Selected Node:");
        GUI.Label(new Rect(660, position.height * 0.89f, 100, 70), selectedNodeIndex == -1 ? "No node selected." : "Node " + (selectedNodeIndex + 1));

        // Save the updated dialog graph:
        if (this.selectedCharacter != null)
        {
            this.selectedCharacter.UpdateDialogGraph(this.nodes);
        }

    } // end OnGUI method

    /// <summary>
    /// Draws the node window.
    /// </summary>
    /// <param name="id">Identifier.</param>
    void DrawNodeWindow(int id)
    {  
       GUI.DragWindow();    // Enables drag and drop functionality of nodes.
    }

    private void DisplayNodeInfo()
    {
        currentSentence = nodes[selectedNodeIndex].content.sentence;
        currentResponses = nodes[selectedNodeIndex].content.responses;

        // Edit selected node section
        GUI.Label(new Rect(20, position.height * 0.83f, 200, 30), "Edit the selected node's content:");
        nodes[selectedNodeIndex].content.sentence = GUI.TextArea(new Rect(20, position.height * 0.86f, 150, 70), currentSentence);

        // Only show the prompt responses if the selected node is not the end node
        if (selectedNodeIndex != END_NODE_INDEX)
        {
            GUI.Label(new Rect(220, position.height * 0.83f, 200, 30), "Edit the response options:");
            int responsesLength = currentResponses == null ? 3 : currentResponses.Count;

            scrollPosition = GUI.BeginScrollView(new Rect(220, position.height * 0.83f, 400, 100), scrollPosition, new Rect(0, 0, responsesLength * 175, 80));
            for (int i = 0; i < responsesLength; i++)
            {
                nodes[selectedNodeIndex].content.responses[i].response = GUI.TextArea(new Rect(175 * i, 18, 150, 40), nodes[selectedNodeIndex].content.responses[i].response);
                int nextSentenceInstance = nodes[selectedNodeIndex].content.responses[i].nextSentence;
                string connectionDestinationString = (nextSentenceInstance == END_NODE_INDEX) ? "Exits conversation" : nextSentenceInstance == START_NODE_INDEX ? "Goes to the start" : "Goes to node " + (nextSentenceInstance - 1);
                GUI.Label(new Rect(175 * i, 60, 150, 20), connectionDestinationString);
                if (GUI.Button(new Rect((175 * i) + 130, 59, deleteConBtnSize, deleteConBtnSize), "X", deleteConnectionButton))
                {
                    nodes[selectedNodeIndex].content.responses.RemoveAt(i);
                    responsesLength--;
                }
            }
            GUI.EndScrollView();
        }
    } // end DisplayNodeInfo method

    // Save the character dialog data to a json file
    private void OnDisable()
    {
        this.selectedCharacter.SerializeData();
    }

} // end DialogEditorWindow class



