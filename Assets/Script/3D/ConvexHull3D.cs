using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;


public class ConvexHull3D : MonoBehaviour
{
    public List<Vector3> listePoints = new List<Vector3>();
    
    public GameObject lnrdr;
    public GameObject meshObj;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            DrawConvexHull3D();
        }
    }

    void DrawConvexHull3D()
    {
        ConvexHull convexHull = new ConvexHull();
        if (listePoints.Count > 3)
        {
            DrawTetrahedre(convexHull);
            foreach (Vector3 pts in listePoints)
            {
               Debug.Log( IsInsidePolygone(pts, convexHull));
            }
        }
        
        
    }

    void DrawTetrahedre(ConvexHull hull)
    {
        GameObject Meshobj = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle1 = new Triangle(listePoints[0],listePoints[1],listePoints[2]);
        EdgesNTris.drawTri(triangle1, Meshobj);
        
        GameObject Meshobj2 = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle2 = new Triangle(listePoints[0],listePoints[3],listePoints[1]);
        EdgesNTris.drawTri(triangle2, Meshobj2);
        
        GameObject Meshobj3 = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle3 = new Triangle(listePoints[0],listePoints[2],listePoints[3]);
        EdgesNTris.drawTri(triangle3, Meshobj3);
        
        GameObject Meshobj4 = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle4 = new Triangle(listePoints[1],listePoints[2],listePoints[3]);
        EdgesNTris.drawTri(triangle4, Meshobj4);
        
        
        hull.listFace.Add(triangle1);
        hull.listFace.Add(triangle2);
        hull.listFace.Add(triangle3);
        hull.listFace.Add(triangle4);
        
        hull.listEdges.Add(triangle1.edges1);
        hull.listEdges.Add(triangle1.edges2);
        hull.listEdges.Add(triangle1.edges3);
        hull.listEdges.Add(triangle2.edges1);
        hull.listEdges.Add(triangle2.edges2);
        hull.listEdges.Add(triangle2.edges3);
        hull.listEdges.Add(triangle3.edges1);
        hull.listEdges.Add(triangle3.edges2);
        hull.listEdges.Add(triangle3.edges3);
        hull.listEdges.Add(triangle4.edges1);
        hull.listEdges.Add(triangle4.edges2);
        hull.listEdges.Add(triangle4.edges3);
        
        hull.listPoints.Add(triangle1.point1);
        hull.listPoints.Add(triangle1.point2);
        hull.listPoints.Add(triangle1.point3);
        hull.listPoints.Add(triangle2.point1);
        hull.listPoints.Add(triangle2.point2);
        hull.listPoints.Add(triangle2.point3);
        hull.listPoints.Add(triangle3.point1);
        hull.listPoints.Add(triangle3.point2);
        hull.listPoints.Add(triangle3.point3);
        hull.listPoints.Add(triangle4.point1);
        hull.listPoints.Add(triangle4.point2);
        hull.listPoints.Add(triangle4.point3);
    }

    bool IsInsidePolygone(Vector3 point, ConvexHull hull)
    {
        bool isInside = true;
        float epsilon = Mathf.Epsilon;
        foreach (Triangle triangle in hull.listFace)
        {
            Vector3 p1 = triangle.point1;
            Vector3 p2 = triangle.point2;
            Vector3 p3 = triangle.point3;
            //creating plane
            Vector3 planePos = p1; 
            Vector3 planeNormal = Vector3.Cross(p3-p2, p1-p2);
            planeNormal = planeNormal.normalized;
            Debug.Log("Plane normal " + planeNormal);
            //getting a signed distance from point to plane
            float distance = Vector3.Dot(planeNormal, point - planePos);
            //Debug.Log(distance);
            if (distance > 0f+epsilon)
            {
                return false;
            }
        }
        return true;
    }
    
}
