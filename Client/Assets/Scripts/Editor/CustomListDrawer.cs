using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer( typeof(CustomListAttribute))]
public class CustomListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        int buttonTotalWidth = 50;

        Rect amountRect = new Rect(position.x, position.y, position.width - buttonTotalWidth, position.height);
        Rect buttonUp = new Rect(amountRect.x + amountRect.width, position.y, buttonTotalWidth / 2, position.height);
        Rect buttonDown = new Rect(buttonUp.x + buttonUp.width, position.y, buttonTotalWidth / 2, position.height);

        string[] variableName = property.propertyPath.Split('.');
        SerializedProperty arrayProperty = property.serializedObject.FindProperty(variableName[0]);
        int index = (int)Char.GetNumericValue(label.text[label.text.Length - 1]);
        int count = arrayProperty.arraySize;

        if (index > 0 && GUI.Button(buttonUp, "^"))
        {
            arrayProperty.MoveArrayElement(index, index - 1);
        }

        if (index < count - 1 && GUI.Button(buttonDown, "v"))
        {
            arrayProperty.MoveArrayElement(index, index + 1);
        }
        
        EditorGUI.PropertyField(amountRect, property, GUIContent.none);
        EditorGUI.indentLevel = indent;
        
        EditorGUI.EndProperty();
    }
}
