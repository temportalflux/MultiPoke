using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(InputResponse))]
public class InputResponseEditor : Editor {

    bool showBindings = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //EditorGUILayout.PropertyField(serializedObject.FindProperty("input"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("inputId"));

        //this.drawBindings();

        serializedObject.ApplyModifiedProperties();
    }

    private void drawBindings()
    {
        SerializedProperty serializedBindings = serializedObject.FindProperty("bindings");

        this.showBindings = EditorGUILayout.Foldout(showBindings, serializedBindings.displayName);
        if (this.showBindings)
        {
            EditorGUI.indentLevel += 1;
            {
                EditorGUILayout.PropertyField(serializedBindings.FindPropertyRelative("Array.size"));
                for (int i = 0; i < serializedBindings.arraySize; i++)
                {
                    SerializedProperty serializedBinding = serializedBindings.GetArrayElementAtIndex(i);
                    serializedBinding.isExpanded = EditorGUILayout.Foldout(serializedBinding.isExpanded, serializedBinding.displayName);
                    if (serializedBinding.isExpanded)
                    {
                        EditorGUI.indentLevel += 1;
                        {
                            EditorGUILayout.PropertyField(serializedBinding.FindPropertyRelative("action"));

                            SerializedProperty type = serializedBinding.FindPropertyRelative("type");
                            EditorGUILayout.PropertyField(type);
                            switch (type.enumValueIndex)
                            {
                                case 0: // BUTTON
                                    EditorGUILayout.PropertyField(serializedBinding.FindPropertyRelative("gamepadButton"));
                                    EditorGUILayout.PropertyField(serializedBinding.FindPropertyRelative("mappedButton"));
                                    break;
                                case 1: // AXIS
                                    EditorGUILayout.PropertyField(serializedBinding.FindPropertyRelative("gamepadAxis"));
                                    EditorGUILayout.PropertyField(serializedBinding.FindPropertyRelative("mappedAxis"));
                                    break;
                                case 2: // TRIGGER
                                    EditorGUILayout.PropertyField(serializedBinding.FindPropertyRelative("gamepadTrigger"));
                                    break;
                                default:
                                    break;
                            }
                        }
                        EditorGUI.indentLevel -= 1;
                    }

                }
            }
            EditorGUI.indentLevel -= 1;
        }
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("listeners"), true);

    }

}
