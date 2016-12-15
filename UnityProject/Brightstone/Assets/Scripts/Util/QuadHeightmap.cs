using UnityEngine;
using System.IO;

namespace Brightstone
{
    // A bsd format file
    // TODO: Write actual header!
    public class QuadHeightmap
    {

        public static void Save(string fullpath, Texture2D texture)
        {
            byte[] texBuffer = texture.GetRawTextureData();
            byte type = 1; // TODO: Make an enum of subfile types

            FileStream stream = File.Open(fullpath, FileMode.Create, FileAccess.Write);
            if(stream != null)
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(type);

                byte[] compressed = Compression.Compress(texBuffer);
                writer.Write(compressed.Length);
                writer.Write(compressed);

                stream.Close();
            }
        }

        public static void Save(string fullpath, byte[] bytes)
        {
            byte type = 1; // TODO: Make an enum of subfile types

            FileStream stream = File.Open(fullpath, FileMode.Create, FileAccess.Write);
            if (stream != null)
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(type);

                byte[] compressed = Compression.Compress(bytes);
                writer.Write(compressed.Length);
                writer.Write(compressed);

                stream.Close();
            }
        }

        public static bool Load(string fullpath, Texture2D texture)
        {
            FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read);
            if(stream != null)
            {
                BinaryReader reader = new BinaryReader(stream);
                byte type = reader.ReadByte();
                // Verify
                if(type == 1)
                {
                    int size = reader.ReadInt32();
                    byte[] buffer = reader.ReadBytes(size);
                    byte[] decompressed = Compression.Decompress(buffer);
                    texture.LoadRawTextureData(decompressed);
                }
                stream.Close();
                return true;
            }
            return false;
        }

        public static bool Load(string fullpath, out byte[] bytes)
        {
            bytes = null;
            FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read);
            if (stream != null)
            {
                BinaryReader reader = new BinaryReader(stream);
                byte type = reader.ReadByte();
                // Verify
                if (type == 1)
                {
                    int size = reader.ReadInt32();
                    byte[] buffer = reader.ReadBytes(size);
                    bytes = Compression.Decompress(buffer);
                }
                stream.Close();
                return true;
            }
            return false;
        }
    }
}