using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class Grid : Actor 
    {
        public class GridData
        {
            private Mesh mMesh = null;
            private Vector3 mPosition = new Vector3();

            public Mesh mesh { get { return mMesh; }set { mMesh = value; } }
            public Vector3 position { get { return mPosition; }set { mPosition = value; } }
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

        [SerializeField]
        private Material mMaterial = null;

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
            for(int z = 0; z < GRID_SIZE; ++z)
            {
                for(int x =0; x < GRID_SIZE; ++x)
                {
                    // indicies.Capacity = indicies.Capacity + 6;
                    for(int i = vertices.Count, maxIter = vertices.Count + 6; i < maxIter; ++i)
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

        private void Start()
        {
            InternalInit();
        }

        private void OnDestroy()
        {
            InternalDestroy();
        }

        private void GenerateGrids()
        {
            mGrids.Clear();
            // 3x3 grids
            mGrids.Add(GenerateGridData(mGridSize, mGapSize));

            // Grid width = gapSize * (numTiles + 1)
            float size = mGapSize * (mGridSize + 1);
            Vector3 startPos = new Vector3(-(size * mNumSubGrids * 0.5f), 0.0f, -(size * mNumSubGrids * 0.5f));


            for(int z = 0; z < mNumSubGrids; ++z)
            {
                for(int x = 0; x < mNumSubGrids; ++x)
                {
                    if(z==0 && x==0)
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

        private void Update()
        {
            
            if (mGenerateGrid)
            {
                GenerateGrids();
                mGenerateGrid = false;
            }

            Camera gameCamera = mWorld.GetGameCamera();
            if (gameCamera != null && mMaterial != null && mGrids.Count > 0)
            {
                // Dumb Hack to force bounds in view!
                Vector3 camPos = gameCamera.transform.position;
                Vector3 camForward = Vector3.Normalize(gameCamera.transform.forward);
                float boundsDistance = (gameCamera.farClipPlane - gameCamera.nearClipPlane) / 2.0f + gameCamera.nearClipPlane;
                Vector3 boundsTarget = camPos + (camForward * boundsDistance);
                Vector3 relativeBoundsTarget = GetTransform().InverseTransformPoint(boundsTarget);
                mGrids[0].mesh.bounds = new Bounds(relativeBoundsTarget, Vector3.one);

                for(int i =0; i < mGrids.Count; ++i)
                {
                    Graphics.DrawMesh(mGrids[i].mesh, mGrids[i].position + mOffset, Quaternion.identity, mMaterial, 0);
                }
            }
        }
    }
}

