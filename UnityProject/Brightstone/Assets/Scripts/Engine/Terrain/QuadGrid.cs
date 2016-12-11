using UnityEngine;

namespace Brightstone
{
    public class QuadGrid : Actor
    {
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
        private int mTileSize = 31;
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

        [Tooltip("Number of tiles to create along the x axis.")]
        [SerializeField]
        private int mWidth = 1;
        [Tooltip("Number of tiles to create along the z axis.")]
        [SerializeField]
        private int mLength = 1;

        // TODO: Have this in the QuadMesh class and serialize data
        [SerializeField]
        private Material mMaterial = null;
        private QuadTile mTile = null;
        private QuadMesh mMesh = null;
        private RenderTexture mHeightMap = null;
        private Texture2D mCollisionHeightMap = null;
        // Draw to heightmap...

        // Cannot modify tile size during gameplay.. It'll mess with the textures..
        private TileProperties mTileProperties = new TileProperties();
        // -- Edit Features
        private Timer mUpdateCollisionTimer = new Timer();
        private bool mCollisionQueued = false;


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

            mTileProperties.size = mTileSize;
            mTileProperties.height = mTileHeight;
            mTileProperties.quadSize = mTileQuadSize;
            mTileProperties.center = mTileCenter;
            mTile = new QuadTile();
            mMesh = new QuadMesh();
            mHeightMap = new RenderTexture(mTileProperties.size + 1, mTileProperties.size + 1, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            mCollisionHeightMap = new Texture2D(mTileProperties.size + 1, mTileProperties.size + 1, TextureFormat.ARGB32, false, true);

            QueueBatchLoad();
        }

        public override void OnBatchComplete()
        {
            base.OnBatchComplete();
            CreateQuad(mTile, mMesh);
            CreateQuadCollision(mTile, mMesh, mHeightMap, mCollisionHeightMap);
            mMesh.Commit();
        }

        private void Update()
        {
            if(mCollisionQueued && mUpdateCollisionTimer.GetElapsedSeconds() > 5.0f)
            {

                CreateQuadCollision(mTile, mMesh, mHeightMap, mCollisionHeightMap);
                mMesh.UpdateCollision();
                mCollisionQueued = false;
            }
            UpdateMaterials();

            mMesh.Render(GetTransform().localToWorldMatrix, mWorld.GetGameCamera());

        }

        private void CreateQuad(QuadTile tile, QuadMesh mesh)
        {
            mesh.Clear();
            mesh.Init();

            tile.width = mTileProperties.size;
            tile.length = mTileProperties.size;
            tile.height = mTileProperties.height;
            tile.quadSize = mTileProperties.quadSize;
            tile.center = mTileProperties.center;

            tile.GenerateVertexData(mesh.vertexData);
            mesh.material = mMaterial;
        }


        private void CreateQuadCollision(QuadTile tile, QuadMesh mesh, RenderTexture heightmap, Texture2D collisionMap)
        {
            // TODO: I can probably move this texture nonsense into Compute Shader...
            RenderTexture oldTarget = RenderTexture.active;
            RenderTexture.active = heightmap;
            collisionMap.ReadPixels(new Rect(0.0f, 0.0f, heightmap.width, heightmap.height), 0, 0);
            RenderTexture.active = oldTarget;

            tile.GenerateCollisionData(mesh.collisionData, collisionMap);
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