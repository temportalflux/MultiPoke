using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{

    public enum TileSide
    {
        CENTER = 0,
        INTER_NW = 5,
        ANTI_NW = 9,
        NORTH = 1,
        INTER_NE = 6,
        ANTI_NE = 10,
        EAST = 2,
        INTER_SE = 7,
        ANTI_SE = 11,
        SOUTH = 3,
        INTER_SW = 8,
        ANTI_SW = 12,
        WEST = 4,
    }

    [System.Serializable]
    public struct PathItem
    {
        public TileSide side;
        public TileBase tile;

        public PathItem(TileSide side)
        {
            this.side = side;
            this.tile = null;
        }

        public PathItem(TileSide side, PathItem[] tiles)
        {
            this.side = side;
            if (tiles != null) this.tile = tiles[(int)side].tile;
            else this.tile = null;
        }

    }

    [CustomGridBrush(false, true, false, "Path Brush")]
    public class BrushPath : GridBrush
    {

        public static TileSide[] TILE_SIDE_CARDINAL = new TileSide[] { TileSide.NORTH, TileSide.EAST, TileSide.SOUTH, TileSide.WEST };
        public static TileSide[] TILE_SIDE_INTER = new TileSide[] { TileSide.INTER_NW, TileSide.INTER_NE, TileSide.INTER_SW, TileSide.INTER_SE };
        public static TileSide[] TILE_SIDE_ANTI = new TileSide[] { TileSide.ANTI_NW, TileSide.ANTI_NE, TileSide.ANTI_SW, TileSide.ANTI_SE };

        // Create the asset for the brush
        [MenuItem("Assets/Create/Asset/Brush - Path")]
        public static void CreateBrush()
        {
            // Get the path to the selected asset
            string selectedPath = "Assets";
            Object selectedObj = Selection.activeObject;
            if (selectedObj != null) selectedPath = AssetDatabase.GetAssetPath(selectedObj.GetInstanceID());

            // Create the save panel
            string path = EditorUtility.SaveFilePanelInProject("Save Brush (Path)", "New Brush (Path)", "asset", "Save Brush (Path)", selectedPath);
            // Check if path was invalid
            if (path == "")
                return;
            // Create the brush asset
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BrushPath>(), path);
        }

        public static Vector3Int offset(Vector3Int position, TileSide side, int mult = 1)
        {
            if (side == TileSide.NORTH ||
                side == TileSide.INTER_NW ||
                side == TileSide.INTER_NE ||
                side == TileSide.ANTI_NW ||
                side == TileSide.ANTI_NE
                )
            {
                position += Vector3Int.up * mult;
            }
            if (side == TileSide.SOUTH ||
                side == TileSide.INTER_SW ||
                side == TileSide.INTER_SE ||
                side == TileSide.ANTI_NW ||
                side == TileSide.ANTI_NE
                )
            {
                position += Vector3Int.up * mult * -1;
            }
            if (side == TileSide.EAST ||
                side == TileSide.INTER_NE ||
                side == TileSide.INTER_SE ||
                side == TileSide.ANTI_NE ||
                side == TileSide.ANTI_SE
                )
            {
                position += Vector3Int.right * mult;
            }
            if (side == TileSide.WEST ||
                side == TileSide.INTER_NW ||
                side == TileSide.INTER_SW ||
                side == TileSide.ANTI_NW ||
                side == TileSide.ANTI_SW
                )
            {
                position += Vector3Int.right * mult * -1;
            }
            return position;
        }

        public static TileSide opposite(TileSide cardinal)
        {
            switch (cardinal)
            {
                case TileSide.NORTH: return TileSide.SOUTH;
                case TileSide.SOUTH: return TileSide.NORTH;
                case TileSide.EAST: return TileSide.WEST;
                case TileSide.WEST: return TileSide.EAST;
                default: return TileSide.CENTER;
            }
        }

        public static PathItem[] newTileList(PathItem[] root = null, bool small = false)
        {
            if (!small)
            {
                return new PathItem[] {
                    new PathItem(TileSide.CENTER, root),
                    new PathItem(TileSide.INTER_NW, root),
                    new PathItem(TileSide.ANTI_NW, root),
                    new PathItem(TileSide.NORTH, root),
                    new PathItem(TileSide.INTER_NE, root),
                    new PathItem(TileSide.ANTI_NE, root),
                    new PathItem(TileSide.EAST, root),
                    new PathItem(TileSide.INTER_SE, root),
                    new PathItem(TileSide.ANTI_SE, root),
                    new PathItem(TileSide.SOUTH, root),
                    new PathItem(TileSide.INTER_SW, root),
                    new PathItem(TileSide.ANTI_SW, root),
                    new PathItem(TileSide.WEST, root)
                };
            }
            else
            {
                return new PathItem[] {
                    new PathItem(TileSide.CENTER, root),
                    new PathItem(TileSide.INTER_NW, root),
                    new PathItem(TileSide.ANTI_NW, null),
                    new PathItem(TileSide.NORTH, root),
                    new PathItem(TileSide.INTER_NE, root),
                    new PathItem(TileSide.ANTI_NE, null),
                    new PathItem(TileSide.EAST, root),
                    new PathItem(TileSide.INTER_SE, root),
                    new PathItem(TileSide.ANTI_SE, null),
                    new PathItem(TileSide.SOUTH, root),
                    new PathItem(TileSide.INTER_SW, root),
                    new PathItem(TileSide.ANTI_SW, null),
                    new PathItem(TileSide.WEST, root)
                };
            }
        }

        public PathItem[] tiles = newTileList();

        public PathItem at(TileSide side, PathItem[] tiles = null)
        {
            if (tiles == null)
                tiles = this.tiles;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].side == side) return tiles[i];
            }
            return tiles[0];
        }

        public void set(TileSide side, TileBase tile, ref PathItem[] tiles)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].side == side) tiles[i].tile = tile;
            }
        }

        public bool isTile(TileBase tile)
        {
            if (tile == null) return false;
            foreach (PathItem item in this.tiles)
            {
                if (item.tile == tile) return true;
            }
            return false;
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {

            if (brushTarget == null)
                return;

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            this.paint(gridLayout, brushTarget, position, tilemap, tilemap.SetTile);

            //base.Paint(gridLayout, brushTarget, position);
        }

        public delegate void DoPaint(Vector3Int pos, TileBase tile);

        public TileSide getTileFor(Tilemap tilemap, Vector3Int position, List<TileSide> forceConnected,
            System.Func<TileSide, TileSide> translateSideToGlboal)
        {
            bool[] hasConnection = new bool[4];
            System.Action<TileSide, bool> markConnected = (TileSide side, bool connected) => { hasConnection[(int)side - 1] = connected; };
            // Checks if a certain side is marked as connected to this position
            System.Func<TileSide, bool> isConnected = (TileSide side) => { return hasConnection[(int)side - 1]; };
            // Returns if the tile at the position in some direction matches the tile for some side in the config
            System.Func<TileSide, TileSide, Vector3Int, bool> isSideFromPos =
                (TileSide dir, TileSide key, Vector3Int pos) =>
                {
                    TileBase tilemapTile = tilemap.GetTile(offset(pos, dir));
                    if (forceConnected.Contains(dir) && !this.isTile(tilemapTile))
                    {
                        tilemapTile = this.at(translateSideToGlboal == null ? dir : translateSideToGlboal(dir)).tile;
                    }
                    
                    // Offset position by direction, and get the tile
                    // compare against the config for some side
                    return tilemapTile == this.at(key).tile;
                };
            System.Func<TileSide, TileSide, bool> isSide =
                (TileSide direction, TileSide key) =>
                {
                    // Offset position by direction, and get the tile
                    // compare against the config for some side
                    return isSideFromPos(direction, key, position);
                };
            TileSide firstUnconnected = TileSide.CENTER, lastConnected = TileSide.CENTER;
            int count = 0;

            foreach (TileSide cardinal in TILE_SIDE_CARDINAL)
            {
                // Check to see if the tile at the position+direction is a valid tile
                markConnected(cardinal,
                    (forceConnected != null && forceConnected.Contains(cardinal)) ||
                    this.isTile(tilemap.GetTile(offset(position, cardinal)))
                );
                if (isConnected(cardinal))
                {
                    count++;
                    lastConnected = cardinal;
                }
                else if (firstUnconnected == TileSide.CENTER) firstUnconnected = cardinal;
            }

            TileSide thisTileType = TileSide.CENTER;
            // If all directions have a valid tile
            if (count >= 4)
            {
                if (
                    (isSide(TileSide.NORTH, TileSide.EAST) || isSide(TileSide.NORTH, TileSide.INTER_NE)) &&
                    (isSide(TileSide.EAST, TileSide.NORTH) || isSide(TileSide.EAST, TileSide.INTER_NE))
                    )
                {
                    thisTileType = TileSide.ANTI_NE;
                }
                else if (
                    (isSide(TileSide.NORTH, TileSide.WEST) || isSide(TileSide.NORTH, TileSide.INTER_NW)) &&
                    (isSide(TileSide.WEST, TileSide.NORTH) || isSide(TileSide.WEST, TileSide.INTER_NW))
                    )
                {
                    thisTileType = TileSide.ANTI_NW;
                }
                else if (
                    (isSide(TileSide.SOUTH, TileSide.EAST) || isSide(TileSide.SOUTH, TileSide.INTER_SE)) &&
                    (isSide(TileSide.EAST, TileSide.SOUTH) || isSide(TileSide.EAST, TileSide.INTER_SE))
                    )
                {
                    thisTileType = TileSide.ANTI_SE;
                }
                else if (
                    (isSide(TileSide.SOUTH, TileSide.WEST) || isSide(TileSide.SOUTH, TileSide.INTER_SW)) &&
                    (isSide(TileSide.WEST, TileSide.SOUTH) || isSide(TileSide.WEST, TileSide.INTER_SW))
                )
                {
                    thisTileType = TileSide.ANTI_SW;
                }
            }
            else if (count == 3)
            {
                thisTileType = firstUnconnected;
            }
            else if (count == 2)
            {
                // 2 sides are connected => 2 sides are missing
                // the 2 connected sides indicate the corner direction
                if (isConnected(TileSide.NORTH) && isConnected(TileSide.EAST))
                {
                    thisTileType = TileSide.INTER_SW;
                }
                else if (isConnected(TileSide.NORTH) && isConnected(TileSide.WEST))
                {
                    thisTileType = TileSide.INTER_SE;
                }
                else if (isConnected(TileSide.SOUTH) && isConnected(TileSide.EAST))
                {
                    thisTileType = TileSide.INTER_NW;
                }
                else if (isConnected(TileSide.SOUTH) && isConnected(TileSide.WEST))
                {
                    thisTileType = TileSide.INTER_NE;
                }
                // The two connections are parallel
                else
                {
                    if (isConnected(TileSide.NORTH) && isConnected(TileSide.SOUTH))
                    {
                        bool isWestFacing = false;
                        isWestFacing = isWestFacing || isSide(TileSide.NORTH, TileSide.WEST) || isSide(TileSide.SOUTH, TileSide.WEST);
                        isWestFacing = isWestFacing || isSide(TileSide.NORTH, TileSide.INTER_NW) || isSide(TileSide.SOUTH, TileSide.INTER_NW);
                        isWestFacing = isWestFacing || isSide(TileSide.NORTH, TileSide.INTER_SW) || isSide(TileSide.SOUTH, TileSide.INTER_SW);
                        bool isEastFacing = false;
                        isEastFacing = isEastFacing || isSide(TileSide.NORTH, TileSide.EAST) || isSide(TileSide.SOUTH, TileSide.EAST);
                        isEastFacing = isEastFacing || isSide(TileSide.NORTH, TileSide.INTER_NE) || isSide(TileSide.SOUTH, TileSide.INTER_NE);
                        isEastFacing = isEastFacing || isSide(TileSide.NORTH, TileSide.INTER_SE) || isSide(TileSide.SOUTH, TileSide.INTER_SE);
                        if (isWestFacing)
                        {
                            thisTileType = TileSide.WEST;
                        }
                        else if (isEastFacing)
                        {
                            thisTileType = TileSide.EAST;
                        }
                    }
                    else if (isConnected(TileSide.EAST) && isConnected(TileSide.WEST))
                    {
                        bool isNorthFacing = false;
                        isNorthFacing = isNorthFacing || isSide(TileSide.WEST, TileSide.NORTH) || isSide(TileSide.EAST, TileSide.NORTH);
                        isNorthFacing = isNorthFacing || isSide(TileSide.WEST, TileSide.INTER_NW) || isSide(TileSide.EAST, TileSide.INTER_NW);
                        isNorthFacing = isNorthFacing || isSide(TileSide.WEST, TileSide.INTER_NE) || isSide(TileSide.EAST, TileSide.INTER_NE);
                        bool isSouthFacing = false;
                        isSouthFacing = isSouthFacing || isSide(TileSide.WEST, TileSide.SOUTH) || isSide(TileSide.EAST, TileSide.SOUTH);
                        isSouthFacing = isSouthFacing || isSide(TileSide.WEST, TileSide.INTER_SW) || isSide(TileSide.EAST, TileSide.INTER_SW);
                        isSouthFacing = isSouthFacing || isSide(TileSide.WEST, TileSide.INTER_SE) || isSide(TileSide.EAST, TileSide.INTER_SE);
                        if (isNorthFacing)
                        {
                            thisTileType = TileSide.NORTH;
                        }
                        else if (isSouthFacing)
                        {
                            thisTileType = TileSide.SOUTH;
                        }
                    }
                }
            }
            else if (count == 1)
            {
                thisTileType = opposite(lastConnected);
            }

            return thisTileType;
        }

        public void paint(GridLayout gridLayout, GameObject brushTarget,
            Vector3Int position, Tilemap tilemap, DoPaint doPaint, PathItem[] tilesIn = null)
        {
            PathItem[] tilesOut = newTileList(tilesIn, true);
            
            List<TileSide> overrides = new List<TileSide>();

            overrides.Clear();
            foreach (TileSide cardinal in TILE_SIDE_CARDINAL)
                overrides.Add(cardinal);
            TileSide thisTileType = this.getTileFor(tilemap, position, overrides, null);

            Vector3Int pos;
            foreach (TileSide cardinal in TILE_SIDE_CARDINAL)
            {
                overrides.Clear();
                pos = offset(position, cardinal);
                overrides.Add(opposite(cardinal));
                switch (cardinal)
                {
                    case TileSide.NORTH:
                        overrides.Add(TileSide.EAST);
                        overrides.Add(TileSide.WEST);
                        break;
                    case TileSide.EAST:
                        overrides.Add(TileSide.NORTH);
                        overrides.Add(TileSide.SOUTH);
                        break;
                    case TileSide.SOUTH:
                        overrides.Add(TileSide.EAST);
                        overrides.Add(TileSide.WEST);
                        break;
                    case TileSide.WEST:
                        overrides.Add(TileSide.NORTH);
                        overrides.Add(TileSide.SOUTH);
                        break;
                    default:
                        continue;
                }
                TileSide tile = this.getTileFor(tilemap, pos, overrides,
                    (TileSide localSide) =>
                    {
                        if (cardinal == TileSide.NORTH)
                        {
                            if (localSide == TileSide.WEST) return TileSide.INTER_NW;
                            if (localSide == TileSide.EAST) return TileSide.INTER_NE;
                        }
                        else if (cardinal == TileSide.EAST)
                        {
                            if (localSide == TileSide.NORTH) return TileSide.INTER_NE;
                            if (localSide == TileSide.SOUTH) return TileSide.INTER_SE;
                        }
                        else if (cardinal == TileSide.SOUTH)
                        {
                            if (localSide == TileSide.WEST) return TileSide.INTER_SW;
                            if (localSide == TileSide.EAST) return TileSide.INTER_SE;
                        }
                        else if (cardinal == TileSide.WEST)
                        {
                            if (localSide == TileSide.NORTH) return TileSide.INTER_NW;
                            if (localSide == TileSide.SOUTH) return TileSide.INTER_SW;
                        }
                        return localSide;
                    }
                );
                this.set(cardinal, this.at(tile).tile, ref tilesOut);
            }
            foreach (TileSide corner in TILE_SIDE_INTER)
            {
                overrides.Clear();
                pos = position;
                switch (corner)
                {
                    case TileSide.INTER_NW:
                        pos = offset(pos, TileSide.NORTH);
                        pos = offset(pos, TileSide.WEST);
                        overrides.Add(TileSide.SOUTH);
                        overrides.Add(TileSide.EAST);
                        break;
                    case TileSide.INTER_NE:
                        pos = offset(pos, TileSide.NORTH);
                        pos = offset(pos, TileSide.EAST);
                        overrides.Add(TileSide.SOUTH);
                        overrides.Add(TileSide.WEST);
                        break;
                    case TileSide.INTER_SE:
                        pos = offset(pos, TileSide.SOUTH);
                        pos = offset(pos, TileSide.EAST);
                        overrides.Add(TileSide.NORTH);
                        overrides.Add(TileSide.WEST);
                        break;
                    case TileSide.INTER_SW:
                        pos = offset(pos, TileSide.SOUTH);
                        pos = offset(pos, TileSide.WEST);
                        overrides.Add(TileSide.NORTH);
                        overrides.Add(TileSide.EAST);
                        break;
                    default:
                        continue;
                }
                TileSide tile = this.getTileFor(tilemap, pos, overrides, null);
                this.set(corner, this.at(tile).tile, ref tilesOut);
            }

            this.set(TileSide.CENTER, this.at(thisTileType).tile, ref tilesOut);

            foreach (PathItem item in tilesOut)
            {
                if (item.tile != null)
                {
                    doPaint(offset(position, item.side), item.tile);
                }
            }

        }

    }

    [CustomEditor(typeof(BrushPath))]
    public class BrushPathEditor : GridBrushEditor
    {
        private BrushPath theBrush { get { return target as BrushPath; } }
        private GameObject lastBrushTarget;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Draw script line
            GUI.enabled = false;
            EditorGUILayout.ObjectField(
                "Script",
                MonoScript.FromScriptableObject(this.theBrush),
                typeof(MonoScript), false
            );
            GUI.enabled = true;

            // Draw each enum line
            for (int i = 0; i < this.theBrush.tiles.Length; i++)
            {
                this.theBrush.tiles[i].tile = (TileBase)EditorGUILayout.ObjectField(this.theBrush.tiles[i].side.ToString(), this.theBrush.tiles[i].tile, typeof(TileBase), false);
            }

            new SerializedObject(this.theBrush).ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(this.theBrush);
        }

        public override void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.PaintPreview(gridLayout, null, position);

            if (brushTarget == null)
                return;

            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            //tilemap.SetEditorPreviewTile(position, this.theBrush.at(TileSide.CENTER).tile);

            this.theBrush.paint(gridLayout, brushTarget, position, tilemap, tilemap.SetEditorPreviewTile);

            lastBrushTarget = brushTarget;

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

    }

}
