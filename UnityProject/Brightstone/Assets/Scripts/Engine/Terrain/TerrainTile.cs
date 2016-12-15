using UnityEngine;

namespace Brightstone
{
    public class TerrainTile
    {
        public struct TileId
        {
            public int x;
            public int y;
            public Vector3 position;

            public TileId(int inX, int inY, Vector3 inPosition)
            {
                x = inX;
                y = inY;
                position = inPosition;
            }
            
        }

        public const int COMPUTE_THREAD_COUNT = 8;

        // private Material mDebugMaterial;
        private Material mMaterial = null;
        //private string mHeightmapData;
        private QuadTile mTile = null;
        private QuadMesh mMesh = null;
        private ComputeBuffer mPositionBuffer = null;
        private ComputeBuffer mDataBuffer = null; // Heightmap/Splat/Collision
        private QuadCollision mRawDataBuffer = null;
        private bool mBufferDirty = true;
        private bool mCreated = false;
        private GameObject mGameObject = null;
        private MeshCollider mMeshCollider = null;
        private TileId mId = default(TileId);
        private float mUpdateCollision = -1.0f;


        private Bounds mBounds = default(Bounds);

        
        public void OnInit(TileId id, QuadCollision collision, QuadTile.TileProperties properties)
        {
            if(mCreated)
            {
                Log.Sys.Error("Error initializing TerrainTile: Tile already created. Must call Clear before OnInit.");
                return;
            }

            if(mMaterial == null)
            {
                Log.Sys.Error("Error initializing TerrainTile: Requires a material.");
                return;
            }

            mId = id;

            // Initialize Tile Properties
            mTile = new QuadTile();
            int trueSize = TerrainUtility.GetHeightmapSize(properties.size);
            mTile.width = trueSize - 1;
            mTile.length = trueSize - 1;
            mTile.height = properties.height;
            mTile.quadSize = properties.quadSize;
            mTile.center = properties.center;

            // Initialize Mesh
            mMesh = new QuadMesh();
            mMesh.Clear();
            mMesh.Init();
            mMesh.material = mMaterial;

            // Generate Vertex && Collision Data
            mTile.GenerateVertexData(mMesh.vertexData);
            mTile.GenerateCollisionData(mMesh.collisionData, collision);


            // Register Collision with unity..
            mGameObject = new GameObject("TerrainTile[" + id.x + "," + id.y + "]");
            mGameObject.transform.position = id.position;
            mMeshCollider = mGameObject.AddComponent<MeshCollider>();

            // Create Compute Buffers.
            // TODO: Look into using GPUMemory instead.
            mPositionBuffer = new ComputeBuffer(trueSize * trueSize, Util.SizeOf(typeof(Vector3)), ComputeBufferType.Default);
            mDataBuffer = new ComputeBuffer(trueSize * trueSize, Util.SizeOf(typeof(QuadCollisionData)), ComputeBufferType.Default);

            // Send data to buffers...
            mRawDataBuffer = collision;
            mPositionBuffer.SetData(mMesh.collisionData.positions.ToArray());
            mDataBuffer.SetData(collision.data);

            // Commit mesh vertex data to memory.
            mMesh.Commit();
            mMeshCollider.sharedMesh = mMesh.collisionMesh;
            mCreated = true;



            mBounds = new Bounds(mId.position, Vector3.one * (mTile.width * mTile.quadSize));
            mMaterial.SetFloat(GetId(TerrainUtility.Keyword.BRUSH_ALPHA), 0.0f);
        }
        
        // Call to release resource. MUST call as compute buffers hold native data.
        public void OnRelease()
        {
            if(!mCreated)
            {
                return;
            }
            
            mPositionBuffer.Release();
            mPositionBuffer.Dispose();
            mPositionBuffer = null;
            mDataBuffer.Release();
            mDataBuffer.Dispose();
            mDataBuffer = null;

            if(mMeshCollider != null)
            {
                mMeshCollider.sharedMesh = null;
                Object.Destroy(mGameObject);
                mMeshCollider = null;
            }
            
            mGameObject = null;
            mPositionBuffer = null;
            mDataBuffer = null;

            mId = default(TileId);
            mTile = null;
            mMesh.Clear();
            mMesh = null;
            mCreated = false;
        }

        // Call to enable/disable collision at runtime.
        public void SetCollisionEnabled(bool enabled)
        {
            if(mGameObject != null)
            {
                mGameObject.SetActive(enabled);
            }
        }

        // Call to destroy collision. (Ie if were hitting 65535 collider count)
        public void DestroyCollision()
        {
            if(mMeshCollider != null)
            {
                Object.Destroy(mMeshCollider);
            }
        }

        // Call to recreate collision if destroyed.
        public void CreateCollision()
        {
            if(mGameObject != null && mMeshCollider == null)
            {
                mMeshCollider = mGameObject.AddComponent<MeshCollider>();
                mMeshCollider.sharedMesh = mMesh.collisionMesh;
            }
        }

        // An expensive function which updates the collision mesh. (Acquires data from compute shader)
        public void UpdateCollision()
        {
            // Note: Don't need to set mesh collider mesh again because collision mesh is never destroyed.
            if(mCreated)
            {
                mDataBuffer.GetData(mRawDataBuffer.data);
                mTile.GenerateCollisionData(mMesh.collisionData, mRawDataBuffer);
                mMesh.UpdateCollision();

                if(mMeshCollider != null)
                {
                    mMeshCollider.sharedMesh = mMesh.collisionMesh;
                }
            }
        }

        // Call to paint on the terrain.
        public bool RenderBrush(TerrainBrush brush, ComputeShader shader, int kernel)
        {
            // Don't paint if were not targeting anything.
            if(brush.target == null)
            {
                mMaterial.SetFloat(GetId(TerrainUtility.Keyword.BRUSH_ALPHA), 0.0f);
                return false;
            }


            // And don't paint if the brush is to far from the terrain tile.
            if(brush.isCircle)
            {
                BoundingSphere sphere = new BoundingSphere(brush.position, brush.size);
                if(!MathUtils.SphereIntersectBounds(sphere, mBounds))
                {
                    mMaterial.SetFloat(GetId(TerrainUtility.Keyword.BRUSH_ALPHA), 0.0f);
                    return false;
                }
            }
            else if(brush.isSquare)
            {
                Bounds bounds = new Bounds(brush.position, Vector3.one * brush.size);
                if(!bounds.Intersects(mBounds))
                {
                    mMaterial.SetFloat(GetId(TerrainUtility.Keyword.BRUSH_ALPHA), 0.0f);
                    return false;
                }

            }


            shader.SetVector(GetString(TerrainUtility.Keyword.OFFSET_POSITION), mId.position);
            shader.SetInt(GetString(TerrainUtility.Keyword.TERRAIN_MAP_SIZE), mTile.width + 1);
            shader.SetBuffer(kernel, GetString(TerrainUtility.Keyword.VERTEX_POSITIONS), mPositionBuffer);
            shader.SetBuffer(kernel, GetString(TerrainUtility.Keyword.TERRAIN_DATA), mDataBuffer);
            shader.Dispatch(kernel, (mTile.width+1) / COMPUTE_THREAD_COUNT, (mTile.length +1)/ COMPUTE_THREAD_COUNT, 1);
            mMaterial.SetFloat(GetId(TerrainUtility.Keyword.BRUSH_ALPHA), 1.0f);
            mMaterial.SetVector(GetId(TerrainUtility.Keyword.BRUSH_POSITION), brush.position);
            mBufferDirty = true;
            return true;
        }

        public void RenderDebugTexture(Texture2D texture)
        {
            byte[] data = new byte[(mTile.width +1)* (mTile.length +1) * Util.SizeOf(typeof(QuadCollisionData))];
            mDataBuffer.GetData(data);
            texture.LoadRawTextureData(data);
            texture.Apply();
        }

        // Call to render the actual terrain.
        public void Render(Camera camera)
        {
            if(mMesh != null && mMaterial != null)
            {
                Matrix4x4 transform = Matrix4x4.TRS(mId.position, Quaternion.identity, Vector3.one);

                if(mBufferDirty)
                {
                    mMaterial.SetBuffer(GetString(TerrainUtility.Keyword.TERRAIN_DATA), mDataBuffer);
                    mBufferDirty = false;
                }
                mMaterial.SetFloat(GetId(TerrainUtility.Keyword.TERRAIN_MAX_HEIGHT), mTile.height);
                mMesh.Render(transform, camera);
            }
        }

        public void Update(float delta)
        {
            if(mUpdateCollision > 0.0f)
            {
                mUpdateCollision -= delta;
                if(mUpdateCollision <= 0.0f)
                {
                    UpdateCollision();
                }
            }
        }

        public void QueueUpdateCollision()
        {
            mUpdateCollision = 2.0f;
        }

        private int GetId(TerrainUtility.Keyword keyword)
        {
            return TerrainUtility.GetId(keyword);
        }
        private string GetString(TerrainUtility.Keyword keyword)
        {
            return TerrainUtility.GetString(keyword);
        }

        public Material GetMaterial() { return mMaterial; }
        public void SetMaterial(Material material) { mMaterial = new Material(material); }
        public QuadTile GetQuadTile() { return mTile; }
        public QuadMesh GetQuadMesh() { return mMesh; }
        public TileId GetId() { return mId; }
        public QuadCollision GetCollisionData() { return mRawDataBuffer; }

    }
}