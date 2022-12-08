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
               Debug.Log( IsInsidePolygone(pts, convexHull)+" IsInside");
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
        List<Triangle> tris = new List<Triangle>();
        for (int i = 4; i < listePoints.Count; i++)
        {
            foreach (var face in hull.listFace)
            {
                //on calcule le volume du tétraedre
                Vector3 pts1 = face.point1;
                Vector3 pts2 = face.point2;
                Vector3 pts3 = face.point3;
                //calcul de l'aire du triangle de base via formule de heron
                float Length1 = Vector3.Distance(pts1, pts2);
                float Length2 = Vector3.Distance(pts2, pts3);
                float Length3 = Vector3.Distance(pts3, pts1);
                float perimeter = (Length1 + Length2 + Length3) / 2;
                float area = Mathf.Sqrt(perimeter * (perimeter - Length1) * (perimeter - Length2) *
                                        (perimeter - Length3));
                //Debug.Log(area);
                //centre triangle
                Vector3 center = (pts1 + pts2 + pts3) / 3;
                //Debug.LogWarningFormat(center +" center");
                float heigth = Vector3.Distance(center, listePoints[i]);
                float volume = (1 / 3) * (area) * heigth;
                //Debug.Log(heigth);
                Debug.Log(volume+" volume");
                Debug.Log(area+" area");
                Debug.Log(heigth +" heigth");
                if (volume < 0)
                {
                    tris.Add(face);
                }
            }
        }

        if (tris.Count > 0)
        {
            return true;
        }

        return false;
    }
    
}
