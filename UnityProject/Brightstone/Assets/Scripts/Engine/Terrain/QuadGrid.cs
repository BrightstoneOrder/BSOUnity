using UnityEngine;
using System.Runtime.InteropServices;

namespace Brightstone
{
    public class QuadGrid : Actor
    {
        // Editor Mode:
        // int state = PAINTING ?
        //      1. Execute compute shader (WRITE)
        //          Require Position Buffer
        //          Modify Heights Buffer if within radius RWStructuredBuffer<float>
        //      2. Execute vertex shader (READ)
        //          
        //

        // Recap: 
        // I have a position/heightmap buffer...
        // They can be 

        // Input handler for UI
        private class ActionHandler : UIActionHandler
        {
            public QuadGrid instance { get; set; }

            public override void OnAction(UIAction action, UIBase sender)
            {
                instance.OnAction(sender.id);
            }
        }

        private enum ActionId
        {
            SAVE_HEIGHT
        }

        

        

        public struct TileProperties
        {
            public int size;
            public int height;
            public float quadSize;
            public bool center;

        }
        // Manages QuadTiles..

        // private QuadTile[] mTiles = null;
        // private QuadMesh[] mMeshes = null;

        [Tooltip("The width/length of each tile.")]
        [SerializeField]
        private HeightmapSize mTileSize = HeightmapSize.HS_32;
        [Tooltip("The maximum height of each tile.")]
        [SerializeField]
        private int mTileHeight = 64;
        [Tooltip("The size of each quad in a tile.")]
        [SerializeField]
        private float mTileQuadSize = 1.0f;
        [Tooltip("Center the quads around the tile.")]
        [SerializeField]
        private bool mTileCenter = true;

        // -- Grid

        // [Tooltip("Number of tiles to create along the x axis.")]
        // [SerializeField]
        // private int mWidth = 1;
        // [Tooltip("Number of tiles to create along the z axis.")]
        // [SerializeField]
        // private int mLength = 1;

        // TODO: Have this in the QuadMesh class and serialize data
        [SerializeField]
        private Material mMaterial = null;
        private QuadTile mTile = null;
        private QuadMesh mMesh = null;
        // Draw to heightmap...

        // Cannot modify tile size during gameplay.. It'll mess with the textures..
        private TileProperties mTileProperties = new TileProperties();
        // -- Edit Features
        private Timer mUpdateCollisionTimer = new Timer();
        private bool mCollisionQueued = false;
        private ActionHandler mButtonHandler = new ActionHandler();


        private ComputeBuffer mPositionBuffer = null;
        private ComputeBuffer mHeightMapBuffer = null;
        private int mComputeKernel = 0;
        [SerializeField]
        private ComputeShader mComputeShader = null;
        private Texture2D mDebugTexture = null;

        [Range(1.0f, 15.0f)]
        [SerializeField]
        private float mBrushRadius = 3.0f;

        MeshCollider mMeshCollider = null;

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

            mTileProperties.size = TerrainUtility.GetHeightmapSize(mTileSize);
            mTileProperties.height = mTileHeight;
            mTileProperties.quadSize = mTileQuadSize;
            mTileProperties.center = mTileCenter;
            mTile = new QuadTile();
            mMesh = new QuadMesh();

            QueueBatchLoad();

            
        }

        private QuadCollision mTempBuffer = null;
        public override void OnBatchComplete()
        {
            base.OnBatchComplete();

            byte[] texData;
            bool loadedHeightmap = QuadHeightmap.Load(Project.GetResourcePath("/Game/World/HeightMapR32.bsd"), out texData);
            if (!loadedHeightmap)
            {
                const int STRIDE = 4;
                texData = new byte[mTileProperties.size * mTileProperties.size * STRIDE];
                for (int i = 0; i < texData.Length; ++i)
                {
                    texData[i] = 0;
                }
            }

            //Texture2D heightmap = new Texture2D(mTileProperties.size, mTileProperties.size, TextureFormat.RFloat, false, true);
            //heightmap.LoadRawTextureData(texData);
            mTempBuffer = QuadCollision.Create(mTileProperties.size);
            TerrainUtility.MarshalCollision(mTempBuffer, texData);

            CreateQuad(mTile, mMesh);
            CreateQuadCollision(mTile, mMesh, mTempBuffer);


            GameObject meshColliderObject = new GameObject("MeshCollider");
            mMeshCollider = meshColliderObject.AddComponent<MeshCollider>();
            

            mButtonHandler.instance = this;
            mWorld.GetUIMgr().RegisterGlobalHandler(UIElement.UE_ACTION_BUTTON, mButtonHandler);

            // GCHandle handle = default(GCHandle);
            // try
            // {
            //     handle = GCHandle.Alloc(pos, GCHandleType.Pinned);
            //     System.IntPtr ptr = handle.AddrOfPinnedObject();
            //     Marshal.Copy(ptr, mDebugPositions, 0, 12 * pos.Length);
            // }
            // finally
            // {
            //     if(handle != default(GCHandle))
            //     {
            //         handle.Free();
            //     }
            // }

            // Read in the saved heightmap.

            // Position Buffer is a set of vertices.. for the editor.. (Vector3)
            int vector3Size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3));
            mPositionBuffer = new ComputeBuffer(mTileProperties.size * mTileProperties.size, vector3Size);
            Vector3[] data = mMesh.collisionData.positions.ToArray();
            mPositionBuffer.SetData(data);
            // Heightmap is a set of normalized values 0.0-1.0 ... 
            mHeightMapBuffer = new ComputeBuffer(mTileProperties.size * mTileProperties.size, sizeof(float));
            mHeightMapBuffer.SetData(texData);


            mComputeKernel = mComputeShader.FindKernel("CSMain");
            mComputeShader.SetBuffer(mComputeKernel, "mPositions", mPositionBuffer);
            mComputeShader.SetBuffer(mComputeKernel, "mHeightMap", mHeightMapBuffer);

            mDebugTexture = new Texture2D(mTileProperties.size, mTileProperties.size, TextureFormat.RFloat, false, false);
            mDebugTexture.filterMode = FilterMode.Point;
            mDebugTexture.LoadRawTextureData(texData);
            mDebugTexture.Apply();
            UIImage image = mWorld.GetUIMgr().FindElement(UIElement.UE_IMAGE, "DebugHeightmap") as UIImage;
            if (image != null)
            {
                image.SetTexture(mDebugTexture);
            }

            mMesh.Commit();
            mMeshCollider.sharedMesh = mMesh.collisionMesh;
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            mWorld.GetUIMgr().UnregisterGlobalHandler(UIElement.UE_ACTION_BUTTON, mButtonHandler);
            mPositionBuffer.Release();
            mHeightMapBuffer.Release();
        }

        private void Update()
        {
            if(IsBatchLoadRegistered())
            {
                return;
            }   

            if(mCollisionQueued && mUpdateCollisionTimer.GetElapsedSeconds() > 5.0f)
            {
                // TODO: Use array instead.
                mHeightMapBuffer.GetData(mTempBuffer.data);

                

                CreateQuadCollision(mTile, mMesh, mTempBuffer);
                mMesh.UpdateCollision();
                mCollisionQueued = false;
            }
            UpdateMaterials();

            

            Vector4 mousePos = mWorld.GetInputMgr().GetMouseData().worldPosition;
            mousePos.y = 0.0f;
            mComputeShader.SetInt("mState", Input.GetMouseButton(0) ? 1 : 0);
            mComputeShader.SetFloat("mBrushRadius", mBrushRadius);
            mComputeShader.SetVector("mWorldPosition", mousePos);
            mComputeShader.Dispatch(mComputeKernel, mTileProperties.size/8, mTileProperties.size/8, 1);
            DebugOutputTexture();
            mMaterial.SetBuffer("mHeightMapBuffer", mHeightMapBuffer);
            mMesh.Render(GetTransform().localToWorldMatrix, mWorld.GetGameCamera());
            if (mHeightMapBuffer == null)
            {
                Log.Sys.Error("Lolwut we need buffer...");
            }
            

            // CPU Implementation
            //if(mHeightMapBuffer != null)
            //{
            //    Vector4 mousePos = mWorld.GetInputMgr().GetMouseData().worldPosition;
            //    mousePos.y = 0.0f;
            //    float[] buffer = new float[mDebugTexture.width * mDebugTexture.height];
            //    byte[] srcbuffer = mDebugTexture.GetRawTextureData();
            //
            //    // marshal
            //    for(int i = 0; i < mDebugTexture.width * mDebugTexture.height * sizeof(float); i+=4)
            //    {
            //        buffer[i/4] = System.BitConverter.ToSingle(srcbuffer, i);
            //    }
            //
            //    for (int y = 0; y < mDebugTexture.height; ++y)
            //    {
            //        for (int x = 0; x < mDebugTexture.width; ++x)
            //        {
            //            int index = x + (32) * y;
            //            Vector3 position = mDebugPositions[index];
            //            float dist = (position - new Vector3(mousePos.x, mousePos.y, mousePos.z)).magnitude;
            //            if (dist < 8.0f)
            //            {
            //                buffer[index] = 1.0f;
            //            }
            //            else
            //            {
            //                buffer[index] = 0.0f;
            //            }
            //        }
            //    }
            //
            //    // marshal back..
            //    for(int i = 0; i < mDebugTexture.width * mDebugTexture.height * sizeof(float); i+=4)
            //    {
            //        byte[] fb = System.BitConverter.GetBytes(buffer[i / 4]);
            //        for(int j = 0; j < 4; ++j)
            //        {
            //            srcbuffer[i + j] = fb[j];
            //        }
            //    }
            //
            //    mDebugTexture.LoadRawTextureData(srcbuffer);
            //    mDebugTexture.Apply();
            //}



        }

        

        protected void OnAction(int id)
        {
            ActionId action = (ActionId)id;
            switch(action)
            {
                case ActionId.SAVE_HEIGHT:
                    OnSaveHeight();
                    break;
            }
        }

        private void DebugOutputTexture()
        {
            if(mHeightMapBuffer == null)
            {
                return;
            }
            byte[] data = new byte[mTileProperties.size * mTileProperties.size * 4];
            mHeightMapBuffer.GetData(data);
            mDebugTexture.LoadRawTextureData(data);
            mDebugTexture.Apply();
            
        }

        private void OnSaveHeight()
        {
            // Read  from buffer into Texture2D
            const int STRIDE = 4;
            byte[] data = new byte[mTileProperties.size * mTileProperties.size * STRIDE];
            mHeightMapBuffer.GetData(data);
            QuadHeightmap.Save(Project.GetResourcePath("/Game/World/HeightMapR32.bsd"), data);
            
        }

        

        private void CreateQuad(QuadTile tile, QuadMesh mesh)
        {
            mesh.Clear();
            mesh.Init();

            tile.width = mTileProperties.size - 1;
            tile.length = mTileProperties.size - 1;
            tile.height = mTileProperties.height;
            tile.quadSize = mTileProperties.quadSize;
            tile.center = mTileProperties.center;

            tile.GenerateVertexData(mesh.vertexData);
            mesh.material = mMaterial;
        }


        private void CreateQuadCollision(QuadTile tile, QuadMesh mesh, QuadCollision heightmap)
        {
            tile.GenerateCollisionData(mesh.collisionData, heightmap);
        }

        private void QueueCollisionTimer()
        {
            mCollisionQueued = true;
            mUpdateCollisionTimer.Reset();
            mUpdateCollisionTimer.Start();
        }

        private void UpdateMaterials()
        {
            mMaterial.SetFloat("_Height", mTileHeight);
        }
        
            }

}