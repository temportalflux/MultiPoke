using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameState))]
public class GameStateEditor : Editor
{

    /// <summary>
    /// Creates a <see cref="ScriptableObject"/> for GameState from a Unity Menu context
    /// </summary>
    [MenuItem("Assets/Create/Asset/Game State")]
    public static void Create()
    {
        // Get the path to the selected asset
        string selectedPath = "Assets";
        UnityEngine.Object selectedObj = Selection.activeObject;
        if (selectedObj != null) selectedPath = AssetDatabase.GetAssetPath(selectedObj.GetInstanceID());

        // Create the save panel
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Game State", "New Game State",
            "asset", "Save Game State",
            selectedPath
        );
        // Check if path was invalid
        if (path == "")
            return;

        // Create the brush asset
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameState>(), path);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GameState gameState = this.target as GameState;

        // Draw script line
        GUI.enabled = false;
        EditorGUILayout.ObjectField(
            "Script",
            MonoScript.FromScriptableObject(gameState),
            typeof(MonoScript), false
        );
        GUI.enabled = true;

        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("allMonsters"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("starters"), true);

        gameState.editorFoldoutPlayers = EditorGUILayout.Foldout(gameState.editorFoldoutPlayers, "Players");
        if (gameState.editorFoldoutPlayers)
        {
            EditorGUI.indentLevel++;

            // Draw players list
            foreach (uint id in gameState.players.Keys)
            {
                GameState.Player player = gameState.players[id];
                if (!gameState.editorFoldouts.ContainsKey(id))
                    gameState.editorFoldouts[id] = false;

                gameState.editorFoldouts[id] = EditorGUILayout.Foldout(gameState.editorFoldouts[id], "Player " + id);
                if (gameState.editorFoldouts[id])
                {
                    EditorGUI.indentLevel++;
                    GUI.enabled = false;

                    EditorGUILayout.IntField("ID", (int)player.playerID);

                    EditorGUILayout.TextField("Name", player.name);

                    EditorGUILayout.ColorField("Color", player.color);

                    EditorGUILayout.Vector3Field("Position", player.position);

                    EditorGUILayout.Vector3Field("Velocity", player.velocity);

                    EditorGUILayout.Vector3Field("Accelleration", player.accelleration);

                    EditorGUILayout.Toggle("In Battle", player.inBattle);

                    EditorGUILayout.LabelField("Cretins #" + player.monsterIDs.Count);
                    foreach (uint monsterID in player.monsterIDs)
                    {
                        //EditorGUILayout.ObjectField("Cretin", mdo, typeof(MonsterDataObject), true);
                        EditorGUILayout.LabelField("Cretin " + gameState.allMonsters[monsterID].GetMonsterName);
                    }

                    GUI.enabled = true;
                    EditorGUI.indentLevel--;
                }

            }

            EditorGUI.indentLevel--;
        }

        // Save changed properties
        this.serializedObject.ApplyModifiedProperties();

    }

}
