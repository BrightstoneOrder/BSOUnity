using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace Brightstone
{
    public class WorldTerrainData : ScriptableObject
    {
        [Serializable]
        public struct TileData
        {
            public int x;
            public int y;
            public Vector3 position;
            public string subDataLocation;
        }

        [SerializeField]
        private string mDataLocation = string.Empty;
        [SerializeField]
        private HeightmapSize mTileSize = HeightmapSize.HS_32;
        [SerializeField]
        private int mTileMaxHeight = 64;
        [SerializeField]
        private float mTileQuadSize = 1.0f;
        [SerializeField]
        private bool mTileCenter = false;

        // Save Data like Tile Properties..
        // Individual Tiles..
        [SerializeField]
        private List<TileData> mTileData = new List<TileData>();


        public HeightmapSize GetTileSize() { return mTileSize; }
        public int GetTileMaxHeight() { return mTileMaxHeight; }
        public float GetTileQuadSize() { return mTileQuadSize; }
        public bool GetTileCenter() { return mTileCenter; }
        public TileData[] GetTiles() { return mTileData.ToArray(); }

        public void AddTile(int x, int y, Vector3 position)
        {
            TileData tile = new TileData();
            tile.x = x;
            tile.y = y;
            tile.position = position;
            tile.subDataLocation = "TerrainTile_" + x + "_" + y + StreamCommon.DATA_EXTENSION;
            mTileData.Add(tile);
        }

        public void RemoveTile(int x, int y)
        {
            for(int i = 0; i < mTileData.Count; ++i)
            {
                if(mTileData[i].x == x && mTileData[i].y == y)
                {
                    mTileData.RemoveAt(i);
                    break;
                }
            }
        }

        private bool FindTile(int x, int y, out TileData outData)
        {
            for (int i = 0; i < mTileData.Count; ++i)
            {
                if (mTileData[i].x == x && mTileData[i].y == y)
                {
                    outData = mTileData[i];
                    return true;
                }
            }
            outData = new TileData();
            return false;
        }

        public string GetCollisionDataPath(int x, int y)
        {
            TileData data;
            if(FindTile(x,y, out data))
            {
                return mDataLocation + data.subDataLocation;
            }
            return string.Empty;
        }

        public QuadCollision GetCollisionData(int x, int y)
        {
            TileData data;
            if(FindTile(x,y, out data))
            {
                string fullPath = mDataLocation + data.subDataLocation;
                QuadCollision collision = null;
                if (!File.Exists(fullPath))
                {
                    collision = QuadCollision.Create(mTileSize);
                    return collision;
                }
                collision = new QuadCollision();
                collision.Load(fullPath);
                return collision;
            }
            return null;
        }

        

        public void MarkDirty()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif 
        }
    }

}

