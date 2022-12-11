using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EdgesNTris
{
    private static Vector3 Vec0 = new Vector3(0f, 0f, 0f);
    public static void drawEdge(GameObject pts1, GameObject pts2, GameObject LndrObj)
    {
        LndrObj.transform.position = Vec0;
        Vector3 pos0 = new Vector3(0f, 0f, 0f);
        LineRenderer lnrdr = LndrObj.GetComponent<LineRenderer>();
        lnrdr.material = new Material(Shader.Find("Sprites/Default"));
        lnrdr.startColor = Color.red;
        lnrdr.endColor = Color.red;
        lnrdr.startWidth = 0.1f;
        lnrdr.endWidth = 0.1f;
        lnrdr.positionCount = 2;
        lnrdr.useWorldSpace = true;
        lnrdr.SetPosition(0, pts1.transform.position);
        lnrdr.SetPosition(1, pts2.transform.position);
    }

    public static void drawTri(Triangle triangle, GameObject meshObj)
    {
        int LayerName = LayerMask.NameToLayer("Face");
        meshObj.transform.position = Vec0;
        meshObj.layer = LayerName;
        MeshRenderer meshRenderer = meshObj.GetComponent<MeshRenderer>();
        
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Opaque"));

        MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();

        MeshCollider meshcollider = meshObj.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3]
        {
            triangle.point1.coordonées,
            triangle.point2.coordonées,
            triangle.point3.coordonées
        };
        mesh.vertices = vertices;

        int[] tris = new int[3]
        {
            // lower left triangle
            0, 2, 1,
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[3]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[3]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;

        meshcollider.sharedMesh= mesh;
        meshFilter.mesh = mesh;
        
    }
}
