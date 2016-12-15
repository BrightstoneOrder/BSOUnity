using System.IO;
using System;
namespace Brightstone
{
    public static class Compression
    {
        // Compress with LZMA
        public static byte[] Compress(byte[] inBytes)
        {
            MemoryStream inStream = new MemoryStream(inBytes);
            MemoryStream outStream = new MemoryStream();
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();

            coder.WriteCoderProperties(outStream);
            outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 8);

            coder.Code(inStream, outStream, inStream.Length, -1, null);
            return outStream.ToArray();
        }

        // Decompress LZMA
        public static byte[] Decompress(byte[] inBytes)
        {
            MemoryStream inStream = new MemoryStream(inBytes);
            MemoryStream outStream = new MemoryStream();
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);

            byte[] fileLengthBytes = new byte[8];
            inStream.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(inStream, outStream, inStream.Length, fileLength, null);
            return outStream.ToArray();
        }

    }
}