using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulation3Delaunay : MonoBehaviour
{
    public ConvexHull3D convexhull3DScript;
    public ConvexHull hull;
    public ConvexHull hullDelauney = new ConvexHull();
    [SerializeField] private LineRenderer lr;
    [SerializeField] private GameObject line;
    public GameObject sphereRayon;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            hull = convexhull3DScript.convexHull;
            Triangulation3DelaunayFlipping();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            hull = convexhull3DScript.convexHullTriangulationTest;
            Triangulation3DelaunayFlipping();
        }
    }

    public void Triangulation3DelaunayFlipping()
    {
        CentreCircon(hull);
        RayonTetra();

        DrawTriangulation3Delauney(hull);
    }

    public void DrawTriangulation3Delauney(ConvexHull h)
    {
        foreach (GameObject foundObj in GameObject.FindGameObjectsWithTag("Face"))
        {
            Destroy(foundObj);
        }
        Debug.LogWarningFormat(h.listFace.Count.ToString());
        for (int f = 0; f < h.listFace.Count; f++)
        {
            GameObject Meshobj = Instantiate(convexhull3DScript.meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Material mat = convexhull3DScript.mats[Random.Range(0,convexhull3DScript.mats.Count)];
            EdgesNTris.drawTri(h.listFace[f], Meshobj,mat);
        }
    }

    public void CentreCircon(ConvexHull h)
    {
        Vector3 center = new Vector3(0,0,0);

        Vector3 p1 = h.listPoints[0].coordonées;
        Vector3 p2 = h.listPoints[1].coordonées;
        Vector3 p3 = h.listPoints[3].coordonées;
        Vector3 p4 = h.listPoints[4].coordonées;
        
        float v1 = (Mathf.Pow(p2.x,2)+Mathf.Pow(p2.y,2)+Mathf.Pow(p2.z,2)-Mathf.Pow(p1.x,2)-Mathf.Pow(p1.y,2)-Mathf.Pow(p1.z,2))/2;
        float v2 = (Mathf.Pow(p3.x,2)+Mathf.Pow(p3.y,2)+Mathf.Pow(p3.z,2)-Mathf.Pow(p1.x,2)-Mathf.Pow(p1.y,2)-Mathf.Pow(p1.z,2))/2;
        float v3 = (Mathf.Pow(p4.x,2)+Mathf.Pow(p4.y,2)+Mathf.Pow(p4.z,2)-Mathf.Pow(p1.x,2)-Mathf.Pow(p1.y,2)-Mathf.Pow(p1.z,2))/2;
        
        Debug.LogWarningFormat("1: "+v1+"\n2: "+v2+"\n3: "+v3);
    }

    public void RayonTetra()
    {
        
    }
}
