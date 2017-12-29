using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{

    // Basic inspector element for a weighted random tile
    [System.Serializable]
    public struct Element
    {
        public TileBase tile;
        public float weight;
    }

    [CustomGridBrush(false, true, false, "Random Brush")]
    public class BrushRandom : GridBrush
    {

        // Create the asset for the brush
        [MenuItem("Assets/Create/Asset/Random Brush")]
        public static void CreateBrush()
        {
            // Get the path to the selected asset
            string selectedPath = "Assets";
            UnityEngine.Object selectedObj = Selection.activeObject;
            if (selectedObj != null) selectedPath = AssetDatabase.GetAssetPath(selectedObj.GetInstanceID());

            // Create the save panel
            string path = EditorUtility.SaveFilePanelInProject("Save Random Brush", "New Random Brush", "asset", "Save Random Brush", selectedPath);
            // Check if path was invalid
            if (path == "")
                return;
            // Create the brush asset
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BrushRandom>(), path);
        }

        public const int brushRadiusMin = 1, brushRadiusMax = 10;
        public int brushRadius = brushRadiusMin;

        [Tooltip("The list of weighted tiles")]
        public Element[] randomElements;

        public delegate void DoPaint(Vector3Int position, TileBase tile);

        public bool hasValidTileList()
        {
            return this.randomElements != null && this.randomElements.Length > 0;
        }

        public float getTotalWeight()
        {
            // Calculate the total weight of all elements
            float totalWeight = 0;
            foreach (Element e in this.randomElements)
            {
                totalWeight += e.weight;
            }
            return totalWeight;
        }
        
        // Gets some random tile from the list - O(n), n = length of randomElements
        public TileBase getRandomTile(float totalWeight)
        {
            // Find the random value of the weights
            float rand = totalWeight * UnityEngine.Random.value;
            // Find the element for the random value
            foreach (Element e in this.randomElements)
            {
                // If this is the weight (cannot remove the elements weight)
                if (rand < e.weight)
                {
                    // return the tile
                    return e.tile;
                }
                // Else, subtract the current tiles weight
                rand -= e.weight;
            }
            // ERROR: should never be reached
            // Return the first element
            return this.randomElements[0].tile;
        }

        public Vector3Int getSize()
        {
            return this.size * this.brushRadius;
        }

        public BoundsInt getBounds(Vector3Int center)
        {
            // Calculate the bounds of the selection
            Vector3Int size = this.getSize();
            Vector3Int offset = new Vector3Int(
                Mathf.FloorToInt(size.x / 2),
                Mathf.FloorToInt(size.y / 2),
                0
            );
            BoundsInt bounds = new BoundsInt(center - offset, size);
            return bounds;
        }

        public void fill(Tilemap tilemap, BoundsInt bounds, DoPaint doPaint)
        {
            // Check if the random list is valid
            if (this.hasValidTileList())
            {
                float totalWeight = this.getTotalWeight();
                // Iterate over all locations in the bounds
                foreach (Vector3Int location in bounds.allPositionsWithin)
                {
                    TileBase tile = this.getRandomTile(totalWeight);
                    doPaint(location, tile);
                }
            }
        }

        public void paint(GridLayout grid, GameObject brushTarget, Vector3Int position, Tilemap tilemap, DoPaint doPaint)
        {

            // Calulate the bounds of the target position
            Vector3Int min = position - pivot;
            
            // Fill the bounds with random tiles
            this.fill(tilemap, this.getBounds(min), doPaint);

        }

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Check if the random list is valid
            if (this.hasValidTileList())
            {
                // Do nothing if invalid target
                if (brushTarget == null)
                    return;

                // Get the tile map at the target
                Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
                // Do nothing if invalid tilemap
                if (tilemap == null)
                    return;

                this.paint(grid, brushTarget, position, tilemap, tilemap.SetTile);
            }
            else
            {
                base.Paint(grid, brushTarget, position);
            }
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds)
        {
            // Check if the random list is valid
            if (this.hasValidTileList())
            {

                // Do nothing if invalid target
                if (brushTarget == null)
                    return;

                // Get the tile map at the target
                var tilemap = brushTarget.GetComponent<Tilemap>();
                // Do nothing if invalid tilemap
                if (tilemap == null)
                    return;

                // Fill the bounds with random tiles
                this.fill(tilemap, bounds, tilemap.SetTile);
            }
            else
            {
                base.BoxFill(gridLayout, brushTarget, bounds);
            }
        }
        
    }

    [CustomEditor(typeof(BrushRandom))]
    public class RandomBrushEditor : GridBrushEditor
    {
        private BrushRandom randomBrush { get { return target as BrushRandom; } }
        private GameObject lastBrushTarget;
        private TileBase tilePlaceholder;

        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            int i = 0;
            while (this.tilePlaceholder == null)
            {
                this.tilePlaceholder = randomBrush.randomElements[i++].tile;
            }

            if (randomBrush.hasValidTileList())
            {
                base.PaintPreview(grid, null, position);

                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                Vector3Int min = position - randomBrush.pivot;
                BoundsInt bounds = this.randomBrush.getBounds(min);
                foreach (Vector3Int location in bounds.allPositionsWithin)
                {
                    tilemap.SetEditorPreviewTile(location, tilePlaceholder);
                }

                //this.randomBrush.paint(grid, brushTarget, position, tilemap, tilemap.SetEditorPreviewTile);

                lastBrushTarget = brushTarget;
            }
            else
            {
                base.PaintPreview(grid, brushTarget, position);
            }
        }

        public override void BoxFillPreview(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {

            if (brushTarget == null)
                return;

            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                tilemap.SetEditorPreviewTile(location, randomBrush.randomElements[0].tile);
            }

            lastBrushTarget = brushTarget;
            //base.BoxFillPreview(gridLayout, brushTarget, position);
        }

        public override void ClearPreview()
        {
            if (lastBrushTarget != null)
            {
                var tilemap = lastBrushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                tilemap.ClearAllEditorPreviewTiles();

                lastBrushTarget = null;
            }
            else
            {
                base.ClearPreview();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Draw script line
            GUI.enabled = false;
            EditorGUILayout.ObjectField(
                "Script",
                MonoScript.FromScriptableObject(this.randomBrush),
                typeof(MonoScript), false
            );
            GUI.enabled = true;

            SerializedObject target = new SerializedObject(this.randomBrush);

            SerializedProperty propRadius = target.FindProperty("brushRadius");
            EditorGUILayout.IntSlider(propRadius, BrushRandom.brushRadiusMin, BrushRandom.brushRadiusMax);

            SerializedProperty prop = target.FindProperty("randomElements");
            EditorGUILayout.PropertyField(prop, true);

            target.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(randomBrush);
        }
    }
}