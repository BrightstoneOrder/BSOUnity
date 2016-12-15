using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class WorldTerrain : Actor
    {
        private class ActionHandler : UIActionHandler
        {
            public WorldTerrain instance { get; set; }

            public override void OnAction(UIAction action, UIBase sender)
            {
                instance.OnAction(sender.id);
            }
        }

        private enum ActionId
        {
            BRUSH_NONE,
            BRUSH_LOWER,
            BRUSH_RAISE,

            SAVE_HEIGHT
        }

        // Shared Data
        [SerializeField]
        private WorldTerrainData mData = null;

        // TODO: Where to put this material?
        [SerializeField]
        private Material mMaterial = null;
        [SerializeField]
        private ComputeShader mComputeShader = null;
        [SerializeField]
        private float mBrushStrength = 2.0f;
        [SerializeField]
        private float mBrushRadius = 6.0f;

        // Runtime Data
        private QuadTile.TileProperties mTileProperties = new QuadTile.TileProperties();
        private List<TerrainTile> mTiles = new List<TerrainTile>();
        private TerrainBrush mBrush = new TerrainBrush();
        private int mComputeKernel = 0;
        private Texture2D mDebugTexture = null;
        




    private void OnAction(int id)
        {

        }

        // Functions

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
            if(mData != null)
            {
                mTileProperties.center = mData.GetTileCenter();
                mTileProperties.height = mData.GetTileMaxHeight();
                mTileProperties.quadSize = mData.GetTileQuadSize();
                mTileProperties.size = mData.GetTileSize();
            }
            for(int i = 0; i < 5; ++i)
            {
                for(int j = 0; j < 5; ++j)
                {
                    CreateTile(i, j);
                }
            }

            if(mComputeShader != null)
            {
                mComputeKernel = mComputeShader.FindKernel("CSMain");
            }

            int actualSize = TerrainUtility.GetHeightmapSize(mTileProperties.size);
            mDebugTexture = new Texture2D(actualSize, actualSize, TextureFormat.RFloat, false, false);
            mDebugTexture.filterMode = FilterMode.Point;
            mDebugTexture.Apply();
            QueueBatchLoad();
            

        }

        public override void OnBatchComplete()
        {
            base.OnBatchComplete();
            UIImage image = mWorld.GetUIMgr().FindElement(UIElement.UE_IMAGE, "DebugHeightmap") as UIImage;
            if (image != null)
            {
                image.SetTexture(mDebugTexture);
            }
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            for(int i = 0; i < mTiles.Count; ++i)
            {
                mTiles[i].OnRelease();
            }
        }

        private void Update()
        {
            // Setup Brush
            InputMouseData mouseData = mWorld.GetInputMgr().GetMouseData();
            mBrush.SetCircle();
            mBrush.position = mouseData.worldPosition;
            mBrush.target = mouseData.hitGameObject;
            mBrush.strength = mBrushStrength * mWorld.GetGameDelta();
            mBrush.size = mBrushRadius;

            bool leftDown = Input.GetMouseButton(1);
            bool rightDown = Input.GetMouseButton(0);
            // Setup Compute Shader..
            if(mComputeShader != null)
            {
                mComputeShader.SetVector(TerrainUtility.GetString(TerrainUtility.Keyword.BRUSH_POSITION), mBrush.position);
                mComputeShader.SetFloat(TerrainUtility.GetString(TerrainUtility.Keyword.BRUSH_STATE), leftDown ? 1.0f : rightDown ? -1.0f : 0.0f);
                mComputeShader.SetFloat(TerrainUtility.GetString(TerrainUtility.Keyword.BRUSH_RADIUS), mBrush.size);
                mComputeShader.SetFloat(TerrainUtility.GetString(TerrainUtility.Keyword.BRUSH_STRENGTH), mBrush.strength);
                
                for(int i = 0; i < mTiles.Count; ++i)
                {
                    if(mTiles[i].RenderBrush(mBrush, mComputeShader, mComputeKernel) && (leftDown || rightDown))
                    {
                        mTiles[i].QueueUpdateCollision();
                        mTiles[i].RenderDebugTexture(mDebugTexture);
                    }
                    
                }

            }

            for(int i = 0; i < mTiles.Count; ++i)
            {
                mTiles[i].Render(mWorld.GetGameCamera());
                mTiles[i].Update(mWorld.GetGameDelta());
            }
        }

        private void CreateTile(int x, int y)
        {
            // Id, collision, properties
            TerrainTile tile = new TerrainTile();

            TerrainTile.TileId id = new TerrainTile.TileId(x, y, GetQuadCenter(x, y));
            tile.SetMaterial(mMaterial);
            tile.OnInit(id, QuadCollision.Create(mTileProperties.size), mTileProperties);
            mTiles.Add(tile);
            //mData.AddTile(x, y, id.position);
            //mData.MarkDirty();
        }

        private void LoadTile(int x, int y)
        {
            TerrainTile tile = new TerrainTile();
            TerrainTile.TileId id = new TerrainTile.TileId(x, y, GetQuadCenter(x, y));
            tile.SetMaterial(mMaterial);
            QuadCollision collision = mData.GetCollisionData(x, y);
            if(collision != null)
            {
                tile.OnInit(id, collision, mTileProperties);
            }
        }

        private void SaveTiles()
        {
            for(int i = 0; i < mTiles.Count; ++i)
            {
                TerrainTile.TileId id = mTiles[i].GetId();
                string fullPath = mData.GetCollisionDataPath(id.x, id.y);
                mTiles[i].UpdateCollision();
                mTiles[i].GetCollisionData().Save(fullPath);
            }
        }

        private Vector3 GetQuadCenter(int x, int z)
        {
            int actualSize = TerrainUtility.GetHeightmapSize(mTileProperties.size);
            return new Vector3(x * (actualSize -1), 0.0f, z * (actualSize-1));
        }

    }

    //public class WorldTerrain : MonoBehaviour
    //{
    //    [SerializeField]
    //    private int mWidth = 512;
    //    [SerializeField]
    //    private int mLength = 512;
    //    [SerializeField]
    //    private bool mUpdateMesh = false;
    //
    //    MeshRenderer mRenderer = null;
    //    MeshFilter mMeshFilter = null;
    //    Mesh mMesh = null;
    //    
    //    void Start()
    //    {
    //        mRenderer = gameObject.GetComponent<MeshRenderer>();
    //        mMeshFilter = gameObject.GetComponent<MeshFilter>();
    //        mMesh = new Mesh();
    //        CreatePlane(mMesh);
    //        mMeshFilter.mesh = mMesh;
    //
    //
    //        
    //    }
    //
    //    /**
    //     * 1,0 ------- 1,1
    //     *  |           |
    //     *  |           |
    //     *  |           |
    //     *  |           |
    //     * 0,0 ------- 0,1
    //    */
    //    void CreatePlane(Mesh mesh)
    //    {
    //        float y = transform.position.y;
    //
    //        Vector3 v00 = new Vector3(0.0f, y, 0.0f); // top-left
    //        Vector3 v01 = new Vector3(1.0f, y, 0.0f); // top-right
    //        Vector3 v11 = new Vector3(0.0f, y, -1.0f); // bottom-left
    //        Vector3 v10 = new Vector3(1.0f, y, -1.0f); // bottom-right
    //
    //        Vector2 t00 = new Vector2(0.0f, 1.0f);
    //        Vector2 t01 = new Vector2(1.0f, 1.0f);
    //        Vector2 t11 = new Vector2(0.0f, 0.0f);
    //        Vector2 t10 = new Vector2(1.0f, 0.0f);
    //
    //        Color c00 = Color.red;
    //        Color c01 = Color.yellow;
    //        Color c11 = Color.black;
    //        Color c10 = Color.blue;
    //
    //        Vector3 up = Vector3.up;
    //
    //        mesh.Clear();
    //        mesh.name = "PlaneMesh";
    //        mesh.vertices = new Vector3[] { v00, v01, v11, v10 };
    //        mesh.normals = new Vector3[] { up, up, up, up };
    //        mesh.uv = new Vector2[] { t00, t01, t11, t10 };
    //        mesh.colors = new Color[] { c00, c01, c11, c10 };
    //        // Clockwise.. unity does counter clockwise
    //        mesh.SetIndices(new int[] { 0, 1, 2, 3, 2, 1 }, MeshTopology.Triangles, 0, true);
    //        // Counter Clockwise
    //        // mesh.SetIndices(new int[] { 0, 1, 2, 0 2, 3 }, MeshTopology.Triangles, 0, true);
    //    }
    //
    //    void UpdateMesh()
    //    {
    //        // Colors
    //        // texCoords
    //        // vertices
    //        // triangles (indices)
    //        // normals
    //
    //        List<Color> colors = new List<Color>();
    //        List<Vector2> texCoords = new List<Vector2>();
    //        List<Vector3> verts = new List<Vector3>();
    //        List<int> indicies = new List<int>();
    //
    //
    //        float halfWidth = mWidth / 2.0f;
    //        float halfLength = mLength / 2.0f;
    //        float startY = transform.position.y;
    //
    //        Vector3 topLeft = transform.position - new Vector3(halfWidth, 0.0f, halfLength);
    //        Vector3 bottomRight = transform.position + new Vector3(halfWidth, 0.0f, halfLength);
    //        
    //        for(int y = 0; y < mLength; y+=2)
    //        {
    //            for(int x = 0; x < mWidth; ++x)
    //            {
    //                float posX = topLeft.x + x;
    //                float posY = topLeft.z - y;
    //                Vector3 v00 = new Vector3(posX, startY, posY);
    //                Vector3 v01 = new Vector3(posX + 1, startY, posY);
    //                Vector3 v11 = new Vector3(posX + 1, startY, posY - 1);
    //                Vector3 v10 = new Vector3(posX, startY, posY - 1);
    //
    //                
    //                verts.Add(new Vector3(posX, startY, posY));
    //            }
    //        }
    //
    //        // Got positions..
    //        
    //    }
    //
    //    static void Wrap01(ref Vector2 vec)
    //    {
    //        if(vec.x > 1.0f)
    //        {
    //            vec.x -= 1.0f;
    //        }
    //        else if(vec.x < 0.0f)
    //        {
    //            vec.x += 1.0f;
    //        }
    //        if(vec.y > 1.0f)
    //        {
    //            vec.y -= 1.0f;
    //        }
    //        else if(vec.y < 0.0f)
    //        {
    //            vec.y += 1.0f;
    //        }
    //    }
    //
    //    void Update()
    //    {
    //        if(mUpdateMesh)
    //        {
    //            UpdateMesh();
    //            mUpdateMesh = false;
    //        }
    //        // Material mat = mRenderer.material;
    //        // Vector2 offset = mat.GetTextureOffset("mTexture");
    //        // offset.x += Time.deltaTime * 2.0f;
    //        // Wrap01(ref offset);
    //        // mat.SetTextureOffset("mTexture", offset);
    //    }
    //}
}



