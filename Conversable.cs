using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Class to encapsulate the dialog graph for a certain character.
/// </summary>
[Serializable]
public class Conversable : MonoBehaviour
{
    public new string name;

    // List of nodes in format for interpretation by the DialogEditorWindow class
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
    /// Opens the dialog editor window for this Conversable instance.
    /// </summary>
    public void OpenDialogEditorWindow()
    {
        Debug.Log(this._graphGui);
        LoadSavedDialog();
        (new DialogEditorWindow()).ShowWindowForCharacter(this);
    }

    private void LoadSavedDialog()
    {
        Debug.Log("Saved data: ");
        var data = EditorPrefs.GetString(this.name + "-dialog", JsonUtility.ToJson(this, false));
        Debug.Log(data);
        JsonUtility.FromJsonOverwrite(data, this);
    }

    /// <summary>
    /// Serializes the data.
    /// </summary>
    public void SerializeData()
    {
        var data = JsonUtility.ToJson(this._graphGui, false);
        EditorPrefs.SetString(this.name + "-dialog", data);
    }

} // end Conversable class
