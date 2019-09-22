using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogEditorWindow : EditorWindow
{
    private static readonly int maxHeight = 600;
    private static readonly int maxWidth = 800;
    private static readonly int minHeight = 600;
    private static readonly int minWidth = 800;

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


    [MenuItem("Window/Dialog Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogEditorWindow));
    }

    // The actual window code goes here
    private void OnGUI()
    {
        // Lock sizes of window
        this.minSize = new Vector2(minWidth, minHeight);
        this.maxSize = new Vector2(maxWidth, maxHeight);

        Vector2 backgroundSize = new Vector2(position.width * 0.95f, position.height * 0.75f);

        // Draw editor title
        GUILayout.Label("Dialog Editor", EditorStyles.boldLabel);

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
           Debug.Log("Added new dialog");
           DialogNode content = new DialogNode("Dialog content");
           Vector2 nodePosition = new Vector2(Random.Range(backgroundPosition.x, backgroundSize.x - nodeWidth / 2), Random.Range(backgroundPosition.y, backgroundSize.y - nodeHeight / 2));
           nodes.Add(new WindowDialogNode(nodePosition, content));
        }

        Draw all the nodes in the tree
        BeginWindows();
        for (int i = 0; i < nodes.Count; i++)
        {
           Rect nodeBox = nodes[i].Rect;
           nodes[i].Rect = GUI.Window(i, nodes[i].Rect, DrawNodeWindow, "Node " + i);

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
                           DialogResponse response = new DialogResponse("", i);
                           nodes[draggingFromIndex].content.responses.Add(response);
                       }
                       draggingNodeLink = false;
                  }
                   // If the flag is false, then the user is trying to start with the head of the connection
                   else
                   {
                       draggingFromIndex = i;
                       draggingNodeLink = true;
                   }
                   current.Use();
               }
               // If you left click on a node, display the node's info at the bottom of the gui
               else if (current.type == EventType.Layout)
               {
                   selectedNodeIndex = i;
               }
           } // end if

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

        // Draws a line from selected node to the cursor position when creating an edge in the graph
        if (draggingNodeLink)
        {
           Vector2 mousePos = Event.current.mousePosition;
           Vector2 nodePos = nodes[draggingFromIndex].Rect.position;
           Handles.color = Color.blue;
           Handles.DrawLine(new Vector3(mousePos.x, mousePos.y, 0), new Vector3(nodePos.x + WindowDialogNode.width / 2, nodePos.y + WindowDialogNode.height / 2, 0));
           Repaint();
        }

        // End connection attempt if not met
        Event secondEvent = Event.current;
        if (secondEvent.type == EventType.ContextClick)
        {
           draggingNodeLink = false;
           secondEvent.Use();
        }



        GUIStyle deleteConnectionButton = new GUIStyle("delConBtn");
        deleteConnectionButton.fontSize = 13;
        deleteConnectionButton.alignment = TextAnchor.MiddleCenter;
        deleteConnectionButton.normal.textColor = Color.white;

        Texture2D redBackground = new Texture2D(deleteConBtnSize, deleteConBtnSize);
        for (int i = 0; i < deleteConBtnSize; i++)
        {
           for (int j = 0; j < deleteConBtnSize; j++)
           {
               redBackground.SetPixel(j, i, Color.red);
           }
        }
        redBackground.Apply();
        deleteConnectionButton.normal.background = redBackground;


        currentSentence = selectedNodeIndex == -1 ? nodes[selectedNodeIndex].content.sentence : "Dialog content will appear here when you click on a node in the graph.";
        currentResponses = selectedNodeIndex == -1 ? nodes[selectedNodeIndex].content.responses : null;

        Edit selected node section
        GUI.Label(new Rect(20, position.height * 0.83f, 200, 30), "Edit the selected node's content:");
        nodes[selectedNodeIndex].content.sentence = GUI.TextArea(new Rect(20, position.height * 0.86f, 150, 70), currentSentence);

        GUI.Label(new Rect(220, position.height * 0.83f, 200, 30), "Edit the response options:");
        int responsesLength = currentResponses == null ? 3 : currentResponses.Count;

        scrollPosition = GUI.BeginScrollView(new Rect(220, position.height * 0.83f, 400, 100), scrollPosition, new Rect(0, 0, responsesLength * 175, 80));
        for (int i = 0; i < responsesLength; i++)
        {
           nodes[selectedNodeIndex].content.responses[i].response = GUI.TextArea(new Rect(175 * i, 18, 150, 40), nodes[selectedNodeIndex].content.responses[i].response);
           GUI.Label(new Rect(175 * i, 60, 150, 20), "Goes to node " + nodes[selectedNodeIndex].content.responses[i].nextSentence);
           if (GUI.Button(new Rect((175 * i) + 130, 59, deleteConBtnSize, deleteConBtnSize), "X", deleteConnectionButton))
           {
               nodes[selectedNodeIndex].content.responses.RemoveAt(i);
           }
        }
        GUI.EndScrollView();

        if (GUI.Button(new Rect(660, position.height * 0.86f, 100, 70), "Update Dialog!"))
        {
           // Prints all nodes in the graph
           for (int i = 0; i < nodes.Count; i++)
           {
               Debug.Log("Node " + i + ":\n" + nodes[i].content.ToString());
           }
        }

    } // end OnGUI method

    void DrawNodeWindow(int id)
    {  
       GUI.DragWindow();
    }


} // end DialogEditorWindow class



