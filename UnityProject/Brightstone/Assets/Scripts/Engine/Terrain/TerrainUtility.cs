using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Brightstone
{
    public enum HeightmapSize
    {
        HS_32,
        HS_64,
        HS_128,
        HS_256,
        HS_512,
        HS_1024
    }

    public struct TerrainBrush
    {
        private const int TYPE_CIRCLE = 1;
        private const int TYPE_SQUARE = 2;

        public GameObject target;
        public Vector3 position;
        public float size;
        public float strength;
        public int type;

        public bool isCircle { get { return type == TYPE_CIRCLE; } }
        public bool isSquare { get { return type == TYPE_SQUARE; } }

        public void SetCircle() { type = TYPE_CIRCLE; }
    }

    public static class TerrainUtility
    {
        private static readonly int[] HEIGHT_MAP_SIZE_TABLE = new int[]
        {
            32,
            64,
            128,
            256,
            512,
            1024
        } ;

        private struct KeywordTable
        {
            public int id;
            public string name;

            public KeywordTable(string inName)
            {
                id = Shader.PropertyToID(inName);
                name = inName;
            }
        }

        public enum Keyword
        {
            TERRAIN_DATA,
            TERRAIN_MAX_HEIGHT,
            TERRAIN_MAP_SIZE,
            VERTEX_POSITIONS,
            BRUSH_POSITION,
            BRUSH_ALPHA,
            BRUSH_STATE,
            BRUSH_RADIUS,
            BRUSH_STRENGTH,
            OFFSET_POSITION

        }

        private static readonly KeywordTable[] KEYWORDS = new KeywordTable[]
        {
            new KeywordTable("mTerrainData"),
            new KeywordTable("mTerrainMaxHeight"),
            new KeywordTable("mTerrainMapSize"),
            new KeywordTable("mVertexPositions"),
            new KeywordTable("mBrushPosition"),
            new KeywordTable("mBrushAlpha"),
            new KeywordTable("mBrushState"),
            new KeywordTable("mBrushRadius"),
            new KeywordTable("mBrushStrength"),
            new KeywordTable("mOffsetPosition")
        };

        public static int GetId(Keyword keyword)
        {
            return KEYWORDS[(int)keyword].id;
        }
        public static string GetString(Keyword keyword)
        {
            return KEYWORDS[(int)keyword].name;
        }

        public static int GetHeightmapSize(HeightmapSize size)
        {
            return HEIGHT_MAP_SIZE_TABLE[(int)size];
        }

        /// <summary>
        /// Helper function to marshal QuadCollision data into bytes.
        /// </summary>
        public static byte[] MarshalCollision(QuadCollision collision)
        {
            int stride = Marshal.SizeOf(typeof(QuadCollision));
            byte[] destination = new byte[collision.data.Length * stride];

            GCHandle handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(collision.data, GCHandleType.Pinned);
                IntPtr sourcePtr = handle.AddrOfPinnedObject();
                Marshal.Copy(sourcePtr, destination, 0, destination.Length);

            }
            finally
            {
                if(handle != default(GCHandle))
                {
                    handle.Free();
                }
            }
            return destination;
        }

        /// <summary>
        /// Helper function to marshal QuadCollision data from bytes.
        /// </summary>
        public static void MarshalCollision(QuadCollision collision, byte[] source)
        {
            int stride = Marshal.SizeOf(typeof(QuadCollision));

            GCHandle handle = default(GCHandle);
            try
            {
                collision.data = new QuadCollisionData[source.Length / stride];
                handle = GCHandle.Alloc(collision.data, GCHandleType.Pinned);
                IntPtr sourcePtr = handle.AddrOfPinnedObject();
                Marshal.Copy(source, 0, sourcePtr, source.Length);
            }
            finally
            {
                if(handle != default(GCHandle))
                {
                    handle.Free();
                }
            }

        }

    }
}