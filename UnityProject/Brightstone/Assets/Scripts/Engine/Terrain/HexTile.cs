using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class HexTile
    {
        // Some Notes.
        // To get the inner radius of the hexagon split a triangle in 2 right angle
        // triangles. Then take the height of that one edge
        // H = OUTER_RADIUS
        // A = OUTER_RADIUS / 2
        // O = a^2 + b^2 = c^2 
        //     sqrt(c^2-b^2)
        //     sqrt(100-25) => sqrt(75) => 8.66025
        // private const float OUTER_RADIUS = 10.0f;
        // private const float INNER_RADIUS = OUTER_RADIUS * 0.866025404f;

        // Data used to describe positions of vertices of each triangle.
        // Each HexTile has 6.
        public struct TriData
        {
            public Vector3 center;
            public Vector3 outLeftCorner;
            public Vector3 outRightCorner;
            public Vector3 outLeft1;
            public Vector3 outLeft2;
            public Vector3 outRight1;
            public Vector3 outRight2;
            public Vector3 topLeft1;
            public Vector3 topLeft2;
            public Vector3 topRight1;
            public Vector3 topRight2;
            public Vector3 topMiddle;
            public Vector3 innerMiddle;
            public Vector3 innerLowerMiddle;
            public Vector3 innerLeft;
            public Vector3 innerRight;
        }

        // The variables needed to create a hex tile.
        public struct Desc
        {
            public float radius; // [1,100]
            public float extrudeFactor; // [0.1,0.9]
            public float indentAmount; // [0.0, 10]
            public float indentWidthPct; // [0.1,0.5]
            public HexSideBitfield indentSides; 
            public Vector3 center;
            
        }

        public struct Coordinate
        {
            public int x;
            public int y;
            public int z;

            public Coordinate(int inX, int inY, int inZ)
            {
                x = inX;
                y = inY;
                z = inZ;
            }

            public static Coordinate FromPosition(Vector3 position, float outerRadius, float innerRadius)
            {
                float x = position.x / (innerRadius * 2.0f);
                float y = -x;

                float offset = position.z / (outerRadius * 3.0f);
                x -= offset;
                y -= offset;

                return new Coordinate(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(-x - y));
            }

        }

        public void Setup(ref Desc desc)
        {
            mIsDirty = true;
            SetOuterRadius(desc.radius);
            SetExtrudeFactor(desc.extrudeFactor);
            mIndentAmount = Mathf.Clamp(desc.indentAmount, 0.0f, 10.0f);
            mIndentWidth = Mathf.Clamp(desc.indentWidthPct, 0.1f, 0.5f);
            mIndentSides = desc.indentSides;
            mCenter = desc.center;
        }

        private Vector2 MapUv(Vector3 min, Vector3 max, Vector3 value)
        {
            Vector3 result = MathUtils.InverseLerp(min, max, value);
            return new Vector2(result.x, result.z);
        }

        private void AddTriangle(HexMesh.VertexData vd, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 minUv, Vector3 maxUv)
        {
            int i = vd.positions.Count;
            vd.positions.Add(v1);
            vd.positions.Add(v2);
            vd.positions.Add(v3);

            vd.triangles.Add(i);
            vd.triangles.Add(i + 1);
            vd.triangles.Add(i + 2);

            vd.barycentricCoords.Add(new Vector4(1.0f, 0.0f, 0.0f, mId));
            vd.barycentricCoords.Add(new Vector4(0.0f, 1.0f, 0.0f, mId));
            vd.barycentricCoords.Add(new Vector4(0.0f, 0.0f, 1.0f, mId));

            vd.texCoords.Add(MapUv(minUv, maxUv, v1));
            vd.texCoords.Add(MapUv(minUv, maxUv, v2));
            vd.texCoords.Add(MapUv(minUv, maxUv, v3));
        }

        private void AddQuad(HexMesh.VertexData vd, Vector3 bLeft, Vector3 bRight, Vector3 tLeft, Vector3 tRight, Vector3 minUv, Vector3 maxUv)
        {
            AddTriangle(vd, tLeft, bRight, bLeft, minUv, maxUv);
            AddTriangle(vd, tLeft, tRight, bRight, minUv, maxUv);
        }

        // This function gets called to "triangulate" vertex positions for each
        // triangle in the hexagon.
        // TODO: Have an optimized version of vertices that take up the same 
        // area but have less vertices.
        private void CreateTriPositions(ref TriData tri, int i, float distPercent)
        {
            // Corner Verts
            tri.outLeftCorner = tri.center + GetVertex(i);
            tri.outRightCorner = tri.center + GetVertex(i + 1);
            Vector3 leftDir = tri.outLeftCorner - tri.center;
            Vector3 rightDir = tri.outRightCorner - tri.center;

            // Vertex Outter (Left Middle Right)
            tri.outLeft1 = tri.center + (leftDir * distPercent);
            tri.outLeft2 = tri.center + GetInnerVertex(i);
            tri.outRight1 = tri.center + (rightDir * distPercent);
            tri.outRight2 = tri.center + GetInnerVertex(i + 1);
            tri.topMiddle = (tri.outLeftCorner + tri.outRightCorner) / 2.0f;
            Vector3 centerDir = tri.topMiddle - tri.center;
            centerDir.Normalize();

            if (!MathUtils.LineLineIntersection(out tri.topLeft1, new Line(tri.outLeft1, centerDir), new Line(tri.topMiddle, (tri.outLeftCorner - tri.topMiddle).normalized)))
            {
                Log.Sys.Error("Failed to intersect line.");
                return;
            }
            if (!MathUtils.LineLineIntersection(out tri.topLeft2, new Line(tri.outLeft2, centerDir), new Line(tri.topMiddle, (tri.outLeftCorner - tri.topMiddle).normalized)))
            {
                Log.Sys.Error("Failed to intersect line.");
                return;
            }
            if (!MathUtils.LineLineIntersection(out tri.topRight1, new Line(tri.outRight1, centerDir), new Line(tri.topMiddle, (tri.outRightCorner - tri.topMiddle).normalized)))
            {
                Log.Sys.Error("Failed to intersect line.");
                return;
            }

            if (!MathUtils.LineLineIntersection(out tri.topRight2, new Line(tri.outRight2, centerDir), new Line(tri.topMiddle, (tri.outRightCorner - tri.topMiddle).normalized)))
            {
                Log.Sys.Error("Failed to intersect line.");
                return;
            }

            // Find Inner Vertices
            tri.innerMiddle = (tri.outLeft2 + tri.outRight2) / 2.0f;
            tri.innerLowerMiddle = (tri.outLeft1 + tri.outRight1) / 2.0f;
            if (!MathUtils.LineLineIntersection(out tri.innerLeft, new Line(tri.outLeft1, centerDir), new Line(tri.innerMiddle, (tri.outLeft2 - tri.innerMiddle).normalized)))
            {
                Log.Sys.Error("Failed to intersect line.");
                return;
            }
            if (!MathUtils.LineLineIntersection(out tri.innerRight, new Line(tri.outRight1, centerDir), new Line(tri.innerMiddle, (tri.outRight2 - tri.innerMiddle).normalized)))
            {
                Log.Sys.Error("Failed to intersect line.");
                return;
            }
        }

        // Called to do road/water indents into the mesh.
        private bool CreateTriIndent(ref TriData tri, HexSide side, HexSideBitfield type, float indentAmount)
        {
            if (type.Has(side))
            {
                Vector3 indent = new Vector3(0.0f, indentAmount, 0.0f);
                tri.innerMiddle -= indent;
                tri.topMiddle -= indent;
                tri.innerLowerMiddle -= indent;
                return true;
            }
            return false;
        }

        // Called to generate position data, and possibly color for texture lookups.
        public void GenerateData()
        {
            if(mIsDirty)
            {
                Clear();
            }

            int numIndents = mIndentSides.GetBitsOn();
            mTriangleData = new TriData[6];
            for (int i = 0; i < 6; ++i)
            {
                TriData tri = new TriData();
                tri.center = mCenter;
                // Triangulate Positions
                CreateTriPositions(ref tri, i, mIndentWidth);
                // Calc rivers and roads
                CreateTriIndent(ref tri, (HexSide)i, mIndentSides, mIndentAmount);
                if (numIndents > 1)
                {
                    tri.center -= Vector3.up * mIndentAmount;
                }
                mTriangleData[i] = tri;
            }
            mIsDirty = false;
        }

        // Called to generate vertex data.
        public void GenerateVertexData(HexMesh.VertexData vd)
        {
            if(mIsDirty)
            {
                return;
            }
            if ( mTriangleData == null || mTriangleData.Length == 0)
            {
                Log.Sys.Error("GenerateVertexData called but HexTile has not been initialized.");
                return;
            }

            Vector3 min = mCenter + new Vector3(-mOuterRadius, 0.0f, mOuterRadius);
            Vector3 max = mCenter + new Vector3(mOuterRadius, 0.0f, -mOuterRadius);

            for (int i = 0; i < 6; ++i)
            {
                TriData tri = mTriangleData[i];
                // Left Side
                // Inner Quad & Tri
                AddQuad(vd, tri.outLeft1, tri.innerLowerMiddle, tri.innerLeft, tri.innerMiddle, min, max);
                AddTriangle(vd, tri.outLeft1, tri.innerLowerMiddle, tri.center, min, max);
                // Outer tris
                AddTriangle(vd, tri.outLeft2, tri.innerLeft, tri.outLeft1, min, max);
                AddTriangle(vd, tri.outLeftCorner, tri.topLeft2, tri.outLeft2, min, max);
                // Top quad
                AddQuad(vd, tri.outLeft2, tri.innerLeft, tri.topLeft2, tri.topLeft1, min, max);
                AddQuad(vd, tri.innerLeft, tri.innerMiddle, tri.topLeft1, tri.topMiddle, min, max);

                // Right Side
                // Inner Quad & Tri
                AddQuad(vd, tri.innerLowerMiddle, tri.outRight1, tri.innerMiddle, tri.innerRight, min, max);
                AddTriangle(vd, tri.innerLowerMiddle, tri.outRight1, tri.center, min, max);
                // Outer Tris
                AddTriangle(vd, tri.innerRight, tri.outRight2, tri.outRight1, min, max);
                AddTriangle(vd, tri.topRight2, tri.outRightCorner, tri.outRight2, min, max);
                // Top Quad
                AddQuad(vd, tri.innerMiddle, tri.innerRight, tri.topMiddle, tri.topRight1, min, max);
                AddQuad(vd, tri.innerRight, tri.outRight2, tri.topRight1, tri.topRight2, min, max);
            }
        }

        public void SetNeighbours(HexSide side, HexTile neighbour)
        {
            mNeighbours[(int)side] = neighbour;
        }

        public HexTile GetNeighbour(HexSide side)
        {
            return mNeighbours[(int)side];
        }

        public void Clear()
        {
            mTriangleData = null;
        }

        private Vector3 GetVertex(int dir)
        {
            return mLocalVertices[dir];
        }

        private Vector3 GetInnerVertex(int dir)
        {
            return mLocalVertices[dir] * mOneMinusExtrudeFactor;
        }

        private void SetOuterRadius(float outerRadius)
        {
            mOuterRadius = 10.0f;
            //mInnerRadius = CalcInnerRadius(outerRadius);
        }

        public static float CalcInnerRadius(float outerRadius)
        {
            float halfRadius = outerRadius * 0.5f;
            return Mathf.Sqrt((outerRadius * outerRadius) - (halfRadius * halfRadius));
        }

        private void SetExtrudeFactor(float extrudeFactor)
        {
            extrudeFactor = Mathf.Clamp01(extrudeFactor);
            //mExtrudeFactor = extrudeFactor;
            mOneMinusExtrudeFactor = 1.0f - extrudeFactor;
        }

        public void SetLocalVertices(Vector3[] vertices)
        {
            mIsDirty = true;
            mLocalVertices = vertices;
        }

        public void SetId(int id)
        {
            mId = id;
        }

        // public Vector3[] localVertices { get { return mLocalVertices; }set { mIsDirty = true; mLocalVertices = value; } }


        private float mOuterRadius = 10.0f;
        //private float mInnerRadius = 10.0f;
        // The % of radius used for the "middle" hexagon
        //private float mExtrudeFactor = 0.25f;
        // The % of radius used for the "outer" hexagon
        private float mOneMinusExtrudeFactor = 0.75f;
        // How much to indent road/river
        private float mIndentAmount = 1.0f;
        // % of width of the river / road
        private float mIndentWidth = 0.25f;
        private HexSideBitfield mIndentSides = new HexSideBitfield(0);
        // Center world space location of the hexagon
        private Vector3 mCenter = Vector3.zero;

        private int mId = 0;

        // The 6 (+1 for wrap) vertices of the hexagon.
        private Vector3[] mLocalVertices = null;
        // 6 Triangle data.. Generated by GenerateData
        private TriData[] mTriangleData = null;
        // 6 Neighbours of the hex tile.
        private HexTile[] mNeighbours = new HexTile[6];

        private bool mIsDirty = false;
        


    }

}