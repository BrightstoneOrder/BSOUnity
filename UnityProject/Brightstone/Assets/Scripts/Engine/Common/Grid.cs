using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class Grid : Actor 
    {
        [SerializeField]
        private Material mMaterial = null;
        HexMesh mHexMesh = new HexMesh();

        [SerializeField]
        private HexSide[] mSides = null;

        [SerializeField]
        private float mIndentAmount = 0.0f;
        [Range(0.1f,0.75f)]
        [SerializeField]
        private float mIndentWidth = 0.15f;
        [Range(0.1f, 0.9f)]
        [SerializeField]
        private float mExtrudeFactor = 0.75f;
        [Range(0.1f, 100.0f)]
        [SerializeField]
        private float mRadius = 10.0f;
        private float mInnerRadius = 10.0f;
        [SerializeField]
        private int mWidth = 6;
        [SerializeField]
        private int mHeight = 6;
        private List<HexTile> mTiles = new List<HexTile>();

        private float mRefreshQueue = 0.1f;
        private HexTile.Desc[] mChangeDesc = new HexTile.Desc[2];

        [SerializeField]
        private int mDbgSelect = 0;
        [Range(5.0f, 100.0f)]
        [SerializeField]
        private float mCameraSelect = 10.0f;

        public Vector3 dbgPoint = Vector3.zero;
        public int dbgX = 0;
        public int dbgY = 0;
        public int dbgIndex = 0;
        public Transform dbgXform = null;

        // Quad Shit
        public bool mForceRefresh = true;
        public Texture2D mQuadHeight = null;
        private QuadMesh mQuadMesh = new QuadMesh();
        private QuadTile mQuadTile = new QuadTile();
        private List<GameObject> mQuadCollisions = new List<GameObject>();

        [SerializeField]
        private int mDrawWidth = 5;
        [SerializeField]
        private int mDrawHeight = 5;

        [SerializeField]
        private Mesh mMeshInspect = null;

        private void Start()
        {
            InternalInit();
        }

        private void OnDestroy()
        {
            InternalDestroy();
        }

        protected override void OnInit()
        {
            base.OnInit();
            mChangeDesc[0] = new HexTile.Desc();
            mChangeDesc[1] = new HexTile.Desc();
            mChangeDesc[0].indentSides = new HexSideBitfield(0);
            mChangeDesc[1].indentSides = new HexSideBitfield(0);
            // CreateHexagonGrid(mHexMesh);
            
        }

        private bool ChangeDetected(HexTile.Desc a, HexTile.Desc b)
        {
            //return //a.center != b.center ||
            bool r0 = !MathUtils.Equals(a.extrudeFactor, b.extrudeFactor);
            bool r1 = !MathUtils.Equals(a.indentAmount, b.indentAmount);
            bool r2 = a.indentSides.value != b.indentSides.value;
            bool r3 = !MathUtils.Equals(a.indentWidthPct, b.indentWidthPct);
            bool r4 = !MathUtils.Equals(a.radius,b.radius);

            return r0 || r1 || r2 || r3 || r4;
        }
        

        private void Update()
        {
            if(mForceRefresh)
            {
                CreateQuad();
                mForceRefresh = false;

                float tWidth = mQuadTile.quadSize * mQuadTile.width;
                float tHeight = mQuadTile.quadSize * mQuadTile.length;

                for(int i = 0; i < mQuadCollisions.Count; ++i)
                {
                    Destroy(mQuadCollisions[i]);
                }
                mQuadCollisions.Clear();

                for (int i = 0; i < mDrawWidth * mDrawHeight; ++i)
                {
                    int x, y;
                    MathUtils.Index2Coord(i, mDrawWidth, out x, out y);
                    Vector3 pos = new Vector3(tWidth * x, 0.0f, tHeight * y);

                    GameObject meshGameObject = new GameObject("Mesh[" + x.ToString() + "," + y.ToString() + "]");
                    meshGameObject.transform.position = pos;
                    MeshCollider meshCollider = meshGameObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = mQuadMesh.collisionMesh;

                    mQuadCollisions.Add(meshGameObject);

                }

                mMeshInspect = mQuadMesh.mesh;

            }

            // if(mRefreshQueue > 0.0f)
            // {
            //     mRefreshQueue -= Time.deltaTime;
            //     if(mRefreshQueue <= 0.0f)
            //     {
            //         mForceRefresh = true;
            //         mRefreshQueue = 5.0f;
            //     }
            // }

            // if(mRefreshQueue > 0.0f)
            // {
            //     Log.Game.Info("Refresh...");
            //     mRefreshQueue -= Time.deltaTime;
            //     if(mRefreshQueue <= 0.0f)
            //     {
            //         mInnerRadius = HexTile.CalcInnerRadius(mRadius);
            //         mTiles.Clear();
            //         mHexMesh.Clear();
            //         CreateHexagonGrid(mHexMesh);
            //     }
            // }
            // else
            // {
            //     mChangeDesc[1].extrudeFactor = mExtrudeFactor;
            //     mChangeDesc[1].indentAmount = mIndentAmount;
            //     mChangeDesc[1].indentSides = new HexSideBitfield(mSides);
            //     mChangeDesc[1].indentWidthPct = mIndentWidth;
            //     mChangeDesc[1].radius = mRadius;
            //     if(ChangeDetected(mChangeDesc[0], mChangeDesc[1]))
            //     {
            //         mRefreshQueue = 0.35f;
            //     }
            //     mChangeDesc[0] = mChangeDesc[1];
            //     mChangeDesc[1] = new HexTile.Desc();
            // }
            // 
            // mMaterial.SetFloat("_Selected", (float)mDbgSelect);
            // mDbgSelect = Mathf.Clamp(mDbgSelect, 0, mTiles.Count);
            // 
            // Vector3 projPoint = mouseData.screenToWorldPosition;
            // Vector3 projDir = mouseData.screenToWorldDirection;
            // 
            // 
            // 
            // Vector3 gridPoint = projPoint + projDir * mCameraSelect;
            // 
            // HexTile.Coordinate coord = GetCoordinate(gridPoint);
            // int index = GetIndex(coord);
            // 
            // // HexTile.Coordinate coord = HexTile.Coordinate.FromPosition(gridPoint, mRadius * 0.8f, mInnerRadius * 0.8f);
            // dbgX = coord.x;
            // dbgY = coord.z;
            // dbgIndex = index;
            // dbgPoint = gridPoint;
            // if(dbgXform != null)
            // {
            //     dbgXform.position = dbgPoint;
            // }
            // 
            // mHexMesh.Render(GetTransform().localToWorldMatrix, mWorld.GetGameCamera());
            mMaterial.SetFloat("_Height", mQuadTile.height);

            float totalWidth = mQuadTile.quadSize * mQuadTile.width;
            float totalHeight = mQuadTile.quadSize * mQuadTile.length;

            for(int i = 0; i < mDrawWidth * mDrawHeight; ++i)
            {
                int x, y;
                MathUtils.Index2Coord(i, mDrawWidth, out x, out y);
                Vector3 pos = new Vector3(totalWidth * x, 0.0f, totalHeight * y);
                Matrix4x4 xform = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);

                mQuadMesh.Render(xform, mWorld.GetGameCamera());
            }


            InputMouseData mouseData = mWorld.GetInputMgr().GetMouseData();
            dbgXform.position = mouseData.worldPosition;

        }


        private void CreateQuad()
        {
            mQuadMesh.Clear();
            if(mQuadMesh.mesh == null)
            {
                mQuadMesh.mesh = new Mesh();
            }
            if(mQuadMesh.collisionMesh == null)
            {
                mQuadMesh.collisionMesh = new Mesh();
            }
            mQuadMesh.mesh.name = "QuadMesh";
            mQuadMesh.collisionMesh.name = "QuadCollision";

            mQuadTile.width = mWidth;
            mQuadTile.length = mHeight;
            mQuadTile.quadSize = 1.0f;
            mQuadTile.center = true;
            mQuadTile.GenerateVertexData(mQuadMesh.vertexData);
            mQuadTile.GenerateCollisionData(mQuadMesh.collisionData, mQuadHeight);
            mQuadMesh.Commit();
            mQuadMesh.material = mMaterial;

        }

        private float CalculateInnerHexagonRadius(float outerRadius)
        {
            float halfRadius = outerRadius * 0.5f;
            return Mathf.Sqrt((outerRadius * outerRadius) - (halfRadius * halfRadius));
        }

        private Vector3[] GenerateHexagonVertexPositions(float outerRadius, float innerRadius)
        {
            return new Vector3[]
            {
                new Vector3(0.0f, 0.0f, outerRadius),
                new Vector3(innerRadius, 0.0f, 0.5f * outerRadius),
                new Vector3(innerRadius, 0.0f, -0.5f * outerRadius),
                new Vector3(0.0f, 0.0f, -outerRadius),
                new Vector3(-innerRadius, 0.0f, -0.5f * outerRadius),
                new Vector3(-innerRadius, 0.0f, 0.5f * outerRadius),
                new Vector3(0.0f, 0.0f, outerRadius) // Add an extra corner to avoid index out of range.
            };
        }


        private void CreateHexagonCell(int x, int z, int i, Vector3[] localPositions)
        {
            Vector3 position = new Vector3();
            position.x = (x + z * 0.5f - z / 2) * (mInnerRadius * 2.0f);
            position.y = 0.0f; // Mathf.PerlinNoise(x, z);
            position.z = z * mRadius * 1.5f;

            
            HexTile.Desc desc = new HexTile.Desc();
            desc.center = position;
            desc.extrudeFactor = mExtrudeFactor;
            desc.indentAmount = mIndentAmount;
            desc.indentSides = new HexSideBitfield(0);
            desc.indentWidthPct = mIndentWidth;
            desc.radius = mRadius;


            HexTile tile = new HexTile();
            tile.Setup(ref desc);
            tile.SetLocalVertices(localPositions);
            tile.SetId(i);
            tile.GenerateData();

            mTiles.Add(tile);
        }


        private HexTile.Coordinate GetCoordinate(Vector3 position)
        {
            int zCoord = (int)(position.z / 10.0f);
            if(zCoord % 2 == 1)
            {
                position.x += 5.0f;
            }
            int xCoord = (int)(position.x / 10.0f);
            return new HexTile.Coordinate(xCoord, 0, zCoord);
        }

        private int GetIndex(HexTile.Coordinate coord)
        {
            int index = 0;
            MathUtils.Coord2Index(coord.x, coord.z, mWidth, out index);
            return Mathf.Clamp(index, 0, mTiles.Count);
        }

        private void CreateHexagonGrid(HexMesh hex)
        {
            // Create hexmesh
            hex.Clear();
            Vector3[] hexVertices = GenerateHexagonVertexPositions(mRadius, mInnerRadius);
            hex.mesh = new Mesh();
            hex.mesh.name = "HexagonMesh";
            // Generate positions
            for(int z = 0, i= 0; z < mHeight; ++z)
            {
                for(int x = 0; x < mWidth; ++x)
                {
                    CreateHexagonCell(x, z, i, hexVertices);
                    ++i;
                }
            }
            // HexMesh.HexSlideBitfield bitfield = new HexMesh.HexSlideBitfield();
            // for(int i = 0; i < mSides.Length; ++i)
            // {
            //     bitfield.Set(mSides[i]);
            // }
            // Generate actual hexagon

            for(int i =0; i < mTiles.Count; ++i)
            {
                mTiles[i].GenerateVertexData(hex.vertexData);
            }

            //for(int i = 0, size = hexagonPositions.Length; i < size; ++i)
            //{
            //    hex.AddHexagon(hexagonPositions[i], bitfield, mIndentAmount);
            //}

            // Commit 
            hex.Commit();
            hex.material = mMaterial;
        }

        //
        // Generate Hexagons
        // Modify Height
        // Modify Cell Type
        //      Water/Road Config
        //      Base Textures { Top, Sidesx6 } 
        // Join Hexagons


        #region OLD SQUARE GRID
        /*
        public class GridData
        {
            private Mesh mMesh = null;
            private Vector3 mPosition = new Vector3();

            public Mesh mesh { get { return mMesh; } set { mMesh = value; } }
            public Vector3 position { get { return mPosition; } set { mPosition = value; } }
        }

        [SerializeField]
        private bool mGenerateGrid = true;
        [SerializeField]
        private float mGridSize = 10.0f;
        [SerializeField]
        private float mGapSize = 1.0f;
        [SerializeField]
        private float mNumSubGrids = 3;
        [SerializeField]
        private Vector3 mOffset = Vector3.zero;
        List<GridData> mGrids = new List<GridData>();

        private GridData GenerateGridData(float size, float gapSize)
        {
            const int GRID_SIZE = 10;
            float xPos = Mathf.Floor(-size * 0.5f);
            float zPos = Mathf.Floor(size * 0.5f);

            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indicies = new List<int>();
            // Unique Vertex...
            Mesh mesh = new Mesh();
            mesh.name = "Custom Mesh";
            for (int z = 0; z < GRID_SIZE; ++z)
            {
                for (int x = 0; x < GRID_SIZE; ++x)
                {
                    // indicies.Capacity = indicies.Capacity + 6;
                    for (int i = vertices.Count, maxIter = vertices.Count + 6; i < maxIter; ++i)
                    {
                        indicies.Add(i);
                        //colors.Add(Color.white);
                    }
                    float vxPos = xPos + (gapSize * x);
                    float vzPos = zPos + (gapSize * z);

                    // 0,3 ---- 1
                    //  |       |
                    //  5  ----2,4

                    vertices.Add(new Vector3(vxPos, 0.0f, vzPos));
                    vertices.Add(new Vector3(vxPos + gapSize, 0.0f, vzPos));
                    vertices.Add(new Vector3(vxPos + gapSize, 0.0f, vzPos - gapSize));
                    vertices.Add(new Vector3(vxPos, 0.0f, vzPos));
                    vertices.Add(new Vector3(vxPos + gapSize, 0.0f, vzPos - gapSize));
                    vertices.Add(new Vector3(vxPos, 0.0f, vzPos - gapSize));

                    uvs.Add(new Vector2(0.0f, 0.0f));
                    uvs.Add(new Vector2(1.0f, 0.0f));
                    uvs.Add(new Vector2(1.0f, 1.0f));
                    uvs.Add(new Vector2(0.0f, 0.0f));
                    uvs.Add(new Vector2(1.0f, 1.0f));
                    uvs.Add(new Vector2(0.0f, 1.0f));

                    colors.Add(new Color(0.0f, 0.0f, 1.0f, 0.0f));
                    colors.Add(new Color(1.0f, 0.0f, 1.0f, 1.0f));
                    colors.Add(new Color(1.0f, 1.0f, 0.0f, 0.0f));
                    colors.Add(new Color(0.0f, 0.0f, 1.0f, 1.0f));
                    colors.Add(new Color(1.0f, 1.0f, 0.0f, 1.0f));
                    colors.Add(new Color(0.0f, 1.0f, 0.0f, 0.0f));

                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.colors = colors.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = indicies.ToArray();
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000.0f);
            GridData data = new GridData();
            data.mesh = mesh;
            return data;
        }

        private void GenerateGrids()
        {
            mGrids.Clear();
            // 3x3 grids
            mGrids.Add(GenerateGridData(mGridSize, mGapSize));

            // Grid width = gapSize * (numTiles + 1)
            float size = mGapSize * (mGridSize + 1);
            Vector3 startPos = new Vector3(-(size * mNumSubGrids * 0.5f), 0.0f, -(size * mNumSubGrids * 0.5f));


            for (int z = 0; z < mNumSubGrids; ++z)
            {
                for (int x = 0; x < mNumSubGrids; ++x)
                {
                    if (z == 0 && x == 0)
                    {
                        mGrids[0].position = startPos;
                        continue;
                    }
                    GridData grid = new GridData();
                    grid.position = startPos + new Vector3(size * x, 0.0f, size * z);
                    grid.mesh = mGrids[0].mesh;
                    mGrids.Add(grid);
                }
            }
        }
        */
        #endregion

    }
}

