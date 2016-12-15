using UnityEngine;
using System.IO;

namespace Brightstone
{
    public struct QuadCollisionData
    {
        public float height;
    }

    public class QuadCollision
    {
        public const int VERSION = 1;

        public QuadCollisionData[] data = null;
        public int size = 0;

        public void Init(int size, byte[] data)
        {
            TerrainUtility.MarshalCollision(this, data);
        }

        public float GetSample(int x, int y)
        {
            int index = 0;
            MathUtils.Coord2Index(x, y, size, out index);
            return data[index].height;
        }

        // Normalized sampling.
        public float GetSample(float x, float y)
        {
            x = Mathf.Clamp01(x);
            y = Mathf.Clamp01(y);
            float fSize = (float)size;
            int iX = Mathf.RoundToInt(x * fSize);
            int iY = Mathf.RoundToInt(y * fSize);
            return GetSample(iX, iY);
        }

        public void Save(string fullPath)
        {
            FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            if(stream != null)
            {
                // Marshal and compress
                byte[] bytes = TerrainUtility.MarshalCollision(this);
                bytes = Compression.Compress(bytes);

                BinaryWriter writer = new BinaryWriter(stream);
                // Write Header
                writer.Write(StreamCommon.HT_HEIGHTMAP);
                writer.Write(VERSION);
                writer.Write(size);

                // Write Data
                writer.Write(bytes.Length);
                writer.Write(bytes);
                stream.Close();
            }

        }

        public void Load(string fullpath)
        {
            FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read);
            if(stream != null)
            {

                BinaryReader reader = new BinaryReader(stream);
                // Read header
                int header = reader.ReadInt32();
                if(header != StreamCommon.HT_HEIGHTMAP)
                {
                    StreamCommon.InvalidHeader(StreamCommon.HT_HEIGHTMAP, header, fullpath);
                    stream.Close();
                    return;
                }
                int version = reader.ReadInt32();
                if(version != VERSION)
                {
                    StreamCommon.InvalidVersion(VERSION, version, fullpath);
                    stream.Close();
                    return;
                }
                size = reader.ReadInt32();
                // Read Data
                int length = reader.ReadInt32();
                byte[] bytes = reader.ReadBytes(length);
                stream.Close();

                // Decompress and Marshal
                bytes = Compression.Decompress(bytes);
                TerrainUtility.MarshalCollision(this, bytes);
            }
        }



        // Creates a QuadCollision with all 0 height
        public static QuadCollision Create(HeightmapSize size)
        {
            return Create(TerrainUtility.GetHeightmapSize(size));
        }
        public static QuadCollision Create(int size)
        {
            QuadCollision collision = new QuadCollision();
            collision.size = size;
            collision.data = new QuadCollisionData[size * size];

            for (int i = 0; i < collision.data.Length; ++i)
            {
                collision.data[i].height = 0.0f;
            }
            return collision;
        }

        // Creates a QuadCollision sampling height from 'collision'
        public static QuadCollision Resize(QuadCollision collision, int size)
        {
            QuadCollision outCollision = new QuadCollision();
            outCollision.size = size;
            outCollision.data = new QuadCollisionData[size * size];

            for(int i = 0; i < outCollision.data.Length; ++i)
            {
                int x, y;
                MathUtils.Index2Coord(i, size, out x, out y);
                float u = (float)x / (float)size;
                float v = (float)y / (float)size;
                outCollision.data[i].height = collision.GetSample(u, v);
            }

            return collision;
        }
    }

}