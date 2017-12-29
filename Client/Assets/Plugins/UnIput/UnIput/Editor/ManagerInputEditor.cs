using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ManagerInput))]
public class ManagerInputEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset Gamepads"))
        {
            (this.target as ManagerInput).RemoveAllGamepads();
        }

    }

}
