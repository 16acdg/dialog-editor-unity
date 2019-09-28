using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Class to encapsulate the dialog graph for a certain character.
/// </summary>
//[ExecuteInEditMode]
public class Conversation : MonoBehaviour
{
    [SerializeField]
    public int id = 0;

    // List of nodes in format for interpretation by the DialogEditorWindow class
    //[SerializeField]
    private List<WindowDialogNode> _graphGui;
    /// <summary>
    /// Gets the graph for the dialog editor GUI.
    /// </summary>
    /// <value>The graph GUI as a list of WindowDialogNodes.</value>
    public List<WindowDialogNode> GraphGUI
    {
        get
        {
            return this._graphGui == null ? new List<WindowDialogNode>() : this._graphGui;
        }
    }

    private List<DialogNode> _graph;
    public List<DialogNode> Graph
    {
        get
        {
            List<DialogNode> nodes = new List<DialogNode>();
            for (int i = 0; i < this._graphGui.Count; i++)
            {
                nodes.Add(this._graphGui[i].content);
            }
            return nodes;
        }
    }

    /// <summary>
    /// Updates the graph and its associated data for when viewed in the Dialog Editor.
    /// </summary>
    public void UpdateDialogGraph(List<WindowDialogNode> graph)
    {
        this._graphGui = graph;
        List<DialogNode> newGraph = new List<DialogNode>();
        for (int i = 0; i < _graphGui.Count; i++)
        {
            newGraph.Add(this._graphGui[i].content);
        }
        this._graph = newGraph;
    }

    /// <summary>
    /// Opens the dialog editor window for this Conversation instance.
    /// </summary>
    public void OpenDialogEditorWindow()
    {
        LoadSavedDialog();
        (new DialogEditorWindow()).ShowWindowForCharacter(this);
    }

    /// <summary>
    /// Starts the conversation.
    /// </summary>
    public void StartConversation()
    {
        // Reference to the class that contains defitions for functions triggered by this conversation
        // In effect, this is the part of the NPCs behaviour.
        IConversable npcRef = this.gameObject.GetComponentInParent(typeof(IConversable)) as IConversable; 
        DialogManager.Instance.StartConversation(this.Graph, npcRef);
    }

    public bool LoadSavedDialog()
    {
        string path = "Assets/Resources/Conversations/" + id + ".json";
        bool fileExists = File.Exists(path);
        if (fileExists)
        {
            StreamReader reader = new StreamReader(path);
            string data = reader.ReadToEnd();
            reader.Close();
            _graphGui = JsonHelper.FromJson<WindowDialogNode>(data);
        }
        return fileExists;
    }

    /// <summary>
    /// Serializes the data.
    /// </summary>
    public void SaveData()
    {
        string path = "Assets/Resources/Conversations/" + id +".json";
        string data = JsonHelper.ToJson(_graphGui);
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(data);
        writer.Close();
    }


} // end Conversation class
