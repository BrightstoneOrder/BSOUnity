using UnityEngine;

namespace Brightstone
{
    /**
     * 
     * 
     */

    public class TileMap : BaseComponent
    {
        [SerializeField]
        private int mGridWidth = 10;
        [SerializeField]
        private int mGridHeight = 10;
        private float mTileSize = 64.0f;
        private TileGrid mGrid = new TileGrid();
        [SerializeField]
        private Sprite mSprite = null;

        private Vector2 mAspectRatio = Vector2.zero;

        public class TileData : BaseObject
        {
            public override void Serialize(BaseStream stream)
            {
                
            }
        }


        private static Vector2 GetAspectRatio(int x, int y)
        {
            float f = (float)x / (float)y;
            int i = 0;
            while (true)
            {
                ++i;
                if (System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                {
                    break;
                }
            }
            return new Vector2((float)System.Math.Round(f * i, 2), i);
        }

        public class Tile
        {
            private GameObject mGameObject = null;
            
            public void InitGameObject(GameObject master, Vector3 position, float scale, Sprite sprite, int order)
            {
                mGameObject = new GameObject();
                mGameObject.transform.SetParent(master.transform);
                mGameObject.transform.position = position;
                mGameObject.transform.localScale = Vector3.one;  // new Vector3(scale, scale, scale);
                SpriteRenderer spriteRenderer = mGameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = order;
                spriteRenderer.sprite = sprite;
                mGameObject.name = "Tile";

            }

            

            public void Update(TileMap tileMap)
            {
                Vector2 aspectRatio = tileMap.GetAspectRatio();

                Vector3 localScale = mGameObject.transform.localScale;
                Vector3 pos = mGameObject.transform.position;
                Vector3 scale = new Vector3(localScale.x * (aspectRatio.x / 16.0f), localScale.y * (aspectRatio.y / 9.0f), localScale.z);
                Vector3 position = new Vector3(pos.x * (aspectRatio.x / 16.0f), pos.y * (aspectRatio.y / 9.0f), pos.z);
                mGameObject.transform.localScale = scale;
                mGameObject.transform.position = position;
            }

            
        }

        public class TileGrid
        {
            private int mWidth = 0;
            private int mHeight = 0;
            private float mScale = 1.0f;
            private Vector3 mStart = new Vector3();
            private Tile[,] mTiles = null;

            public void GenerateTiles(GameObject master, Sprite sprite)
            {
                Vector3 currentPosition = mStart;
                mTiles = new Tile[mWidth, mHeight];

                for(int y = 0; y < mHeight; ++y)
                {
                    for(int x = 0; x < mWidth; ++x)
                    {
                        Tile tile = new Tile();
                        tile.InitGameObject(master, currentPosition, mScale, sprite, 0);
                        mTiles[x, y] = tile;
                        currentPosition = new Vector3(currentPosition.x + 2.0f, currentPosition.y, currentPosition.z);
                    }
                    currentPosition = new Vector3(mStart.x, currentPosition.y + 2.0f, currentPosition.z);
                }
            }

            public void UpdateTiles(TileMap tileMap)
            {
                for(int y = 0; y < mHeight; ++y)
                {
                    for(int x = 0; x < mWidth; ++x)
                    {
                        Tile tile = mTiles[x, y];
                        tile.Update(tileMap);
                    }
                }
            }

            public int GetWidth() { return mWidth; }
            public int GetHeight() { return mHeight; }
            public float GetScale() { return mScale; }
            public Vector3 GetStart() { return mStart; }

            public void SetWidth(int width) { mWidth = width; }
            public void SetHeight(int height) { mHeight = height; }
            public void SetScale(float scale) { mScale = scale; }
            public Vector3 SetStart(Vector3 position) { return mStart = position; }
        }

        private void Start()
        {
            InternalInit();
        }

        protected override void OnInit()
        {
            mGrid.SetWidth(mGridWidth);
            mGrid.SetHeight(mGridHeight);
            mGrid.SetScale(GetTileScale());
            mGrid.SetStart(Vector3.zero);
            mGrid.GenerateTiles(gameObject, mSprite);
        }

        private void Update()
        {
            // mAspectRatio = GetAspectRatio(Screen.width, Screen.height);
            // Camera.main.orthographicSize = (1080 * mAspectRatio.y / 9.0f / 2.0f) / 100.0f;
            // 
            // mGrid.UpdateTiles(this);
            Camera.main.orthographicSize = GetOrthoSize();
        }


        public float GetTileScale()
        {
            return 100.0f / mTileSize;
        }        

        public float GetOrthoSize()
        {
            float ppu = 32;
            float ppuScale = 2.0f;
            return (Screen.height)/ (ppu * ppuScale) *0.5f;
        }

        public Vector2 GetAspectRatio()
        {
            return mAspectRatio;
        }
    }
}

