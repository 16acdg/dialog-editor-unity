using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector/editor for a GameObject with a Conversable component.
/// </summary>
[CustomEditor(typeof(Conversable))]
public class ConversableEditor : Editor
{
    public Conversable instance;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
       
        // Open the dialog editor window for this instance of Conversable
        if (GUILayout.Button("Edit Dialog")) 
        {
            (target as Conversable).OpenDialogEditorWindow();
        }
    }
} // end ConversableEditor class
