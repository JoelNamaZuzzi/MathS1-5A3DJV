using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;


public class ConvexHull3D : MonoBehaviour
{
    public List<Point> listePoints = new List<Point>();
    
    public GameObject lnrdr;
    public GameObject meshObj;
    public ConvexHull convexHull;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            DrawConvexHull3D();
        }

        foreach (Point point in listePoints)
        {
            if (point.IsMovable)
            {
                point.coordonées = point.mesh.transform.position;
            }
        }
        
    }

    public void DrawConvexHull3D()
    {

        //On créer un objet ConvexHull qui va contenir la liste des triangles(face), des arretes et des sommets

        // A modif car les ancienne Hull reste lors d'une nouvelle genération

        convexHull = new ConvexHull();
        if (listePoints.Count < 3)
        {
            Debug.Log("Il nous faut 4 points au min");
        }
        
        DrawTetrahedre(convexHull);
        foreach (Point pts in listePoints)
        {
            
            bool inside = TestInteriorite(pts);
            if (inside)
            {
                Debug.Log(pts.coordonées+"interieur");
            }
            else
            {
                CheckVisibilité(pts);
                Debug.Log(pts.coordonées+ "exterieur");
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
        
        // On stock notre tetrahedre dans notre convex hull
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
    
    //renvoie true si interieur, sinon renvoie false
    bool TestInteriorite(Point p)
    {

        RaycastHit[] hits;
        Ray ray = new Ray(p.coordonées, Vector3.right);
        hits = Physics.RaycastAll(ray,100);
        Debug.DrawRay(p.coordonées,Vector3.right*100,Color.red,duration:1000000f);
        
        // si il y a 1 seul hit on est a l'interieur sinon on est a l'exterieur 
        if (hits.Length == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Permet de verifier la visibilité d'un point.
    private void CheckVisibilité(Point pts)
    {
        foreach (var triangle in convexHull.listFace)
        {
            if (isVisible(triangle, pts))
            {
                triangle.couleur = color.bleu;
            }
            else
            {
                triangle.couleur = color.rouge;
            }
        }
        // Change la couleur des edges et points
        SetEdgeColor();
        SetPointColor();

    }

    private void SetEdgeColor()
    {
        foreach (var edges in convexHull.listEdges)
        {
            bool onlyRed = true;
            bool onlyBlue = true;
            foreach (var triangle in edges.triangleProprio)
            {
                if (triangle.couleur == color.rouge)
                {
                    onlyBlue = false;
                }

                if (triangle.couleur == color.bleu)
                {
                    onlyRed = false;
                }
            }

            if (onlyRed) edges.couleur = color.rouge;
            else if (onlyBlue) edges.couleur = color.bleu;
            else edges.couleur = color.violet;
        }
    }
    
    private void SetPointColor()
    {
        foreach (var point in convexHull.listPoints)
        {
            bool onlyRed = true;
            bool onlyBlue = true;
            foreach (var edge in point.edgeProprio)
            {
                if (edge.couleur == color.rouge)
                {
                    onlyBlue = false;
                }

                if (edge.couleur == color.bleu)
                {
                    onlyRed = false;
                }
            }

            if (onlyRed) point.couleur = color.rouge;
            else if (onlyBlue) point.couleur = color.bleu;
            else point.couleur = color.violet;
        }
    }

    public void ResetColor()
    {
        foreach (var t in convexHull.listFace)
        {
            t.couleur = color.blanc;
        }
        foreach (var e in convexHull.listEdges)
        {
            e.couleur = color.blanc;
        }
        foreach (var p in convexHull.listPoints)
        {
            p.couleur = color.blanc;
        }
    }
    

    [ContextMenu("Calcul Normale")]
    void CalculateNormale()
    {
        foreach (var point in listePoints)
        {
            point.normal = Vector3.Normalize(point.coordonées);
        }
    }

    bool isVisible(Triangle t, Point p)
    {

        float r = Vector3.Dot(t.point1 - p, t.getNormale());

        if (r > 0) return true;
        return false;
    }
    
    
}
