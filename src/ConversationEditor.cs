using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector/editor for a GameObject with a Conversation component.
/// </summary>
[CustomEditor(typeof(Conversation))]
public class ConversationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Conversation instance = target as Conversation;
        // Open the dialog editor window for this instance of Conversation
        if (GUILayout.Button("Edit Dialog")) 
        {
            instance.OpenDialogEditorWindow();
        }
        //instance.id = EditorGUILayout.IntField(instance.id);
        if (instance.id == 0)
        {
            instance.id = instance.GetInstanceID();
        }
    }
    
} // end ConversationEditor class
