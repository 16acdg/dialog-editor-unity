using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogManager))]
public class DialogManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DialogManager dm = target as DialogManager;
        if (dm.TypewriterEffect)
        {
            dm.TypewriterSpeed = EditorGUILayout.Slider("    Typewriter Speed", dm.TypewriterSpeed, 1, 100);
        }
    }

} // end DialogManagerEditor class
