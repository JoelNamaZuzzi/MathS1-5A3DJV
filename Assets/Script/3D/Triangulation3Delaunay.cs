using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Triangulation3Delaunay : MonoBehaviour
{
    public ConvexHull3D convexhull3DScript;
    public ConvexHull hull;
    public ConvexHull hullDelauney = new ConvexHull();
    [SerializeField] private LineRenderer lr;
    [SerializeField] private GameObject line;
    public GameObject sphereRayon;

    public GameObject goTest;
    
    Matrix4x4 matrix = new Matrix4x4();
    private Matrix4x4 invMatrix = new Matrix4x4();

    public Vector3 center;
    
    public Vector3 centerDelaunay =  new Vector3();
    public float rayon;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            hull = convexhull3DScript.convexHull;
            Triangulation3DelaunayFlipping();
        }
    }

    public void Triangulation3DelaunayFlipping()
    {
        CentreCircon(hull.listPoints[0],hull.listPoints[1],hull.listPoints[2],hull.listPoints[3], hull);

        DrawTriangulation3Delauney(hull);

        Debug.LogWarningFormat(DelaunayCheck(centerDelaunay,goTest.transform.position,rayon).ToString());
    }

    public void DrawTriangulation3Delauney(ConvexHull h)
    {
        foreach (GameObject foundObj in GameObject.FindGameObjectsWithTag("Face"))
        {
            Destroy(foundObj);
        }
        for (int f = 0; f < h.listFace.Count; f++)
        {
            GameObject Meshobj = Instantiate(convexhull3DScript.meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Material mat = convexhull3DScript.mats[Random.Range(0,convexhull3DScript.mats.Count)];
            EdgesNTris.drawTri(h.listFace[f], Meshobj,mat);
        }
    }

    public void CentreCircon(Point p1, Point p2, Point p3, Point p4, ConvexHull h)
    {
        Vector3 center = new Vector3(0,0,0);

        Vector3 pts1 = p1.coordonées;
        Vector3 pts2 = p2.coordonées;
        Vector3 pts3 = p3.coordonées;
        Vector3 pts4 = p4.coordonées;
        
        float v1 = (Mathf.Pow(pts2.x,2)+Mathf.Pow(pts2.y,2)+Mathf.Pow(pts2.z,2)-Mathf.Pow(pts1.x,2)-Mathf.Pow(pts1.y,2)-Mathf.Pow(pts1.z,2))/2;
        float v2 = (Mathf.Pow(pts3.x,2)+Mathf.Pow(pts3.y,2)+Mathf.Pow(pts3.z,2)-Mathf.Pow(pts1.x,2)-Mathf.Pow(pts1.y,2)-Mathf.Pow(pts1.z,2))/2;
        float v3 = (Mathf.Pow(pts4.x,2)+Mathf.Pow(pts4.y,2)+Mathf.Pow(pts4.z,2)-Mathf.Pow(pts1.x,2)-Mathf.Pow(pts1.y,2)-Mathf.Pow(pts1.z,2))/2;

        Vector4 column1 = new Vector4(pts2.x-pts1.x,pts2.y-pts1.y,pts2.z-pts1.z,0);
        Vector4 column2 = new Vector4(pts3.x-pts1.x,pts3.y-pts1.y,pts3.z-pts1.z,0);
        Vector4 column3 = new Vector4(pts4.x-pts1.x,pts4.y-pts1.y,pts4.z-pts1.z,0);
        Vector4 column4 = new Vector4(0,0,0,1);
        
        matrix.SetRow(0,column1);
        matrix.SetRow(1,column2);
        matrix.SetRow(2,column3);
        matrix.SetRow(3,column4);

        invMatrix = matrix.inverse;
        
        // for (int i = 0; i < 4; i++)
        // {
        //     Debug.LogWarningFormat("column : " + i + " " + invMatrix.GetRow(i).ToString());
        //     //Debug.LogWarningFormat("x:"+invMatrix.GetColumn(i).x.ToString());
        //     //Debug.LogWarning(matrix.GetRow(j));
        // }
        

        float x = (invMatrix.GetRow(0).x*v1) + (invMatrix.GetRow(0).y*v2) + (invMatrix.GetRow(0).z*v3);
        float y = (invMatrix.GetRow(1).x*v1) + (invMatrix.GetRow(1).y*v2) + (invMatrix.GetRow(1).z*v3);;
        float z = (invMatrix.GetRow(2).x*v1) + (invMatrix.GetRow(2).y*v2) + (invMatrix.GetRow(2).z*v3);;
        
        center = new Vector3(x, y, z);
        
        float w1 = (matrix.GetRow(0).x*x) + (matrix.GetRow(0).y*y) + (matrix.GetRow(0).z*z);
        float w2 = (matrix.GetRow(1).x*x) + (matrix.GetRow(1).y*y) + (matrix.GetRow(1).z*z);
        float w3 = (matrix.GetRow(2).x*x) + (matrix.GetRow(2).y*y) + (matrix.GetRow(2).z*z);

        centerDelaunay = center;
        RayonTetra(center,pts1);
    }

    public void RayonTetra(Vector3 c, Vector3 p)
    {
        float distance = Mathf.Sqrt(Mathf.Pow((c.x - p.x), 2) + Mathf.Pow((c.y - p.y), 2) + Mathf.Pow((c.z - p.z), 2));
        
        GameObject go = Instantiate(sphereRayon, c, Quaternion.identity);
        go.transform.localScale *= (distance * 2);

        rayon = distance;
    }

    public bool DelaunayCheck(Vector3 c, Vector3 p, float r)
    {
        float distance = Mathf.Sqrt(Mathf.Pow((c.x - p.x), 2) + Mathf.Pow((c.y - p.y), 2) + Mathf.Pow((c.z - p.z), 2));
        
        Debug.LogWarningFormat("Distance : "+distance+" rayon : "+r);
        
        if (distance < r)
        {
            return true;
        }
        
        return false;
    }
}
