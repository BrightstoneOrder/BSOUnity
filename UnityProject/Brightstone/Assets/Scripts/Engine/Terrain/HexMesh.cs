using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class HexMesh
    {
        public class  VertexData
        {
            public List<Vector3> positions = new List<Vector3>();
            public List<int> triangles = new List<int>();
            public List<Vector2> texCoords = new List<Vector2>();
            public List<Vector4> barycentricCoords = new List<Vector4>(); // Debug wireframe
        }

        public void Clear()
        {
            if(mMesh != null)
            {
                mMesh.Clear();
            }
            mVertexData.positions.Clear();
            mVertexData.triangles.Clear();
            mVertexData.texCoords.Clear();
            mVertexData.barycentricCoords.Clear();
        }

        public void Commit()
        {
            mMesh.SetVertices(mVertexData.positions);
            mMesh.SetTriangles(mVertexData.triangles, 0);
            mMesh.SetUVs(0, mVertexData.texCoords);
            mMesh.SetUVs(1, mVertexData.barycentricCoords);
            mMesh.RecalculateNormals();

            mVertexData.positions.Clear();
            mVertexData.triangles.Clear();
            mVertexData.texCoords.Clear();
            mVertexData.barycentricCoords.Clear();
        }

        public void Render(Matrix4x4 transform, Camera gameCamera)
        {
            if (mMesh == null || mMaterial == null || gameCamera == null)
            {
                return;
            }
            // Dumb Hack to force bounds in view!
            Vector3 camPos = gameCamera.transform.position;
            Vector3 camForward = Vector3.Normalize(gameCamera.transform.forward);
            float boundsDistance = (gameCamera.farClipPlane - gameCamera.nearClipPlane) / 2.0f + gameCamera.nearClipPlane;
            Vector3 boundsTarget = camPos + (camForward * boundsDistance);
            Vector3 relativeBoundsTarget = transform.inverse * boundsTarget;
            mMesh.bounds = new Bounds(relativeBoundsTarget, Vector3.one);

            Graphics.DrawMesh(mMesh, transform, mMaterial, 0);
        }

        private Material mMaterial = null;
        private Mesh mMesh = null;
        private VertexData mVertexData = new VertexData();

        public Material material { get { return mMaterial; } set { mMaterial = value; } }
        public Mesh mesh { get { return mMesh; }set { mMesh = value; } }
        public VertexData vertexData { get { return mVertexData; } }

    }
}