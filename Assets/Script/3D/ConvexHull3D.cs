using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConvexHull3D : MonoBehaviour
{
    public List<GameObject> listePoints = new List<GameObject>();
    
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
        
        hull.listFace.Add(triangle1);
        hull.listFace.Add(triangle2);
        hull.listFace.Add(triangle3);
    }

    bool IsInsidePolygone(GameObject point)
    {
        return true;
    }
    
}
