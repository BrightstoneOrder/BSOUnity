using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
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



