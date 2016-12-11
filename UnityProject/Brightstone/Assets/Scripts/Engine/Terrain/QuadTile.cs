using System.Collections.Generic;
using UnityEngine;

namespace Brightstone
{
    public class QuadTile
    {

        private Vector2 MapUv(Vector3 min, Vector3 max, Vector3 value)
        {
            Vector3 result = MathUtils.InverseLerp(min, max, value);
            return new Vector2(result.x, result.z);
        }



        public void GenerateVertexData(QuadMesh.VertexData vd)
        {
            
            float totalWidth = (mQuadSize) * mWidth;
            float totalLength = (mQuadSize) * mLength;

            float halfWidth = totalWidth * 0.5f;
            float halfLength = totalLength * 0.5f;

            Vector3 startPoint = mCenter ? new Vector3(-halfWidth, 0.0f, -halfLength) : Vector3.zero;
            Vector3 point = startPoint;

            Vector3 min = new Vector3(-halfWidth, 0.0f, -halfLength);
            Vector3 max = new Vector3(halfWidth, 0.0f, halfLength);

            List<Vector3> positions = new List<Vector3>();
            // Calculate positions..
            for(int z = 0; z < mLength + 1; ++z)
            {
                for(int x = 0; x < mWidth + 1; ++x)
                {
                    //Color pixelColor = texture.GetPixel(x, z);
                    //point.y = Mathf.Lerp(0.0f, mHeight, pixelColor.r);
                    positions.Add(point);
                    point.x += mQuadSize;
                }
                point.x = startPoint.x;
                point.z += mQuadSize;

                

            }


            int offset = mWidth + 1;
            // Triangulate
            for(int z = 0, i =0; z < mLength; ++z)
            {
                for(int x = 0; x < mWidth; ++x, ++i)
                {
                    Vector3 topLeft = positions[i + offset];
                    Vector3 bottomRight = positions[i + 1];
                    Vector3 bottomLeft = positions[i];
                    Vector3 topRight = positions[i + offset + 1];

                    int tri = vd.positions.Count;

                    vd.positions.Add(topLeft);
                    vd.positions.Add(bottomRight);
                    vd.positions.Add(bottomLeft);

                    vd.positions.Add(topLeft);
                    vd.positions.Add(topRight);
                    vd.positions.Add(bottomRight);


                    vd.triangles.Add(tri + 0);    
                    vd.triangles.Add(tri + 1);
                    vd.triangles.Add(tri + 2);
                    vd.triangles.Add(tri + 3);
                    vd.triangles.Add(tri + 4);
                    vd.triangles.Add(tri + 5);

                    vd.texCoords.Add(new Vector2(0.0f, 0.0f));
                    vd.texCoords.Add(new Vector2(1.0f, 1.0f));
                    vd.texCoords.Add(new Vector2(0.0f, 1.0f));
                    vd.texCoords.Add(new Vector2(0.0f, 0.0f));
                    vd.texCoords.Add(new Vector2(1.0f, 0.0f));
                    vd.texCoords.Add(new Vector2(1.0f, 1.0f));


                    vd.heightmapCoords.Add(MapUv(min, max, topLeft));
                    vd.heightmapCoords.Add(MapUv(min, max, bottomRight));
                    vd.heightmapCoords.Add(MapUv(min, max, bottomLeft));
                    vd.heightmapCoords.Add(MapUv(min, max, topLeft));
                    vd.heightmapCoords.Add(MapUv(min, max, topRight));
                    vd.heightmapCoords.Add(MapUv(min, max, bottomRight));


                    vd.barycentricCoords.Add(new Vector4(1.0f, 0.0f, 0.0f, i));
                    vd.barycentricCoords.Add(new Vector4(0.0f, 1.0f, 0.0f, i));
                    vd.barycentricCoords.Add(new Vector4(0.0f, 0.0f, 1.0f, i));
                    vd.barycentricCoords.Add(new Vector4(1.0f, 0.0f, 0.0f, i));
                    vd.barycentricCoords.Add(new Vector4(0.0f, 1.0f, 0.0f, i));
                    vd.barycentricCoords.Add(new Vector4(0.0f, 0.0f, 1.0f, i));
                }
                ++i;
            }



        
        }

        public void GenerateCollisionData(QuadMesh.VertexData vd, Texture2D texture)
        {
            float totalWidth = (mQuadSize) * mWidth;
            float totalLength = (mQuadSize) * mLength;

            float halfWidth = totalWidth * 0.5f;
            float halfLength = totalLength * 0.5f;

            Vector3 startPoint = mCenter ? new Vector3(-halfWidth, 0.0f, -halfLength) : Vector3.zero;
            Vector3 point = startPoint;

            //List<Vector3> positions = new List<Vector3>();
            // Calculate positions..
            for (int z = 0; z < mLength + 1; ++z)
            {
                for (int x = 0; x < mWidth + 1; ++x)
                {
                    Color pixelColor = texture.GetPixel(x, z);
                    point.y = Mathf.Lerp(0.0f, mHeight, pixelColor.r);
                    vd.positions.Add(point);
                    point.x += mQuadSize;
                }
                point.x = startPoint.x;
                point.z += mQuadSize;
            }

            int offset = mWidth + 1;
            // Triangulate
            for (int z = 0, i = 0; z < mLength; ++z)
            {
                for (int x = 0; x < mWidth; ++x, ++i)
                {
                    vd.triangles.Add(i + offset);
                    vd.triangles.Add(i + 1);
                    vd.triangles.Add(i);
                    vd.triangles.Add(i + offset);
                    vd.triangles.Add(i + offset + 1);
                    vd.triangles.Add(i + 1);
                }
                ++i;
            }
        }



        private int mWidth = 256;
        private int mLength = 256;
        private int mHeight = 64;
        private float mQuadSize = 1.0f;
        private bool mCenter = false;

        public int width { get { return mWidth; }set { mWidth = value; } }
        public int length { get { return mLength; } set { mLength = value; } }
        public int height { get { return mHeight; } set { mHeight = value; } }
        public float quadSize { get { return mQuadSize; }set { mQuadSize = value; } }
        public bool center { get { return mCenter; }set { mCenter = value; } }
    }

}
