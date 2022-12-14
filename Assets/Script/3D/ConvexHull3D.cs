using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;


public class ConvexHull3D : MonoBehaviour
{
    public List<Point> listePoints = new List<Point>();
    
    public GameObject lnrdr;
    public GameObject meshObj;
    public ConvexHull convexHull;
    public ConvexHull convexHullTriangulationTest;

    public List<Material> mats = new List<Material>();
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
        if(convexHull!=null) EraseFace();

        convexHull = new ConvexHull();
        convexHullTriangulationTest = new ConvexHull();
        if (listePoints.Count < 3)
        {
            Debug.Log("Il nous faut 4 points au min");
        }
        
        DrawTetrahedre();
        for (int i = 4; i < listePoints.Count; i++)
        {
            if (Alinterieur(listePoints[i]))
            {
                Debug.Log(listePoints[i].coordonées+"interieur");
            }
            else
            {
                Debug.Log(listePoints[i].coordonées+ "exterieur");
                CheckVisibilité(listePoints[i]);
                UpdateHull(listePoints[i],convexHull);
            }
        }
    }
    
    void DrawTetrahedre()
    {
        AddTriangle(listePoints[0],listePoints[1],listePoints[2]);
        AddTriangle(listePoints[0],listePoints[3],listePoints[1]);
        AddTriangle(listePoints[0],listePoints[2],listePoints[3]);
        AddTriangle(listePoints[1],listePoints[2],listePoints[3]);
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

    bool Alinterieur(Point p)
    {
        for (int i = 0; i < convexHull.listFace.Count; i++)
        {
            if ( isVisible(convexHull.listFace[i], p)) return false;
        }
        return true;
    }
    
    public void EraseFace()
    {
        for (int i = 0; i < convexHull.listFace.Count; i++)
        {
            Destroy(convexHull.listFace[i].mesh);
        }
        
        convexHull.listEdges.Clear();
        convexHull.listFace.Clear();
        convexHull.listPoints.Clear();
    }


    public void UpdateHull(Point pts, ConvexHull hull)
    {
       
        for (int i = 0; i < hull.listFace.Count(); i++)
        {
            Triangle curFace = hull.listFace[i];
            //We only interact with blue ones
            if (curFace.couleur == color.bleu)
            {
                //Delete Face
                EndFace(curFace);
                hull.listFace.RemoveAt(i);
                i -= 1;
            }
        }
        for (int i = 0; i < hull.listPoints.Count(); i++)
        {
            Point curPts = hull.listPoints[i];
            //We only look at blue pts, red & purple one aren't useful here
            if (curPts.couleur == color.bleu)
            {
                //Delete Pts
                GameObject.Destroy(curPts.mesh);
                hull.listPoints.RemoveAt(i);
                i -= 1;
            }
        }
        for (int i = 0; i<hull.listEdges.Count(); i++)
        {
            Edges curEdge = hull.listEdges[i];
            //We don't look at red ones here
            if (curEdge.couleur == color.bleu)
            {
                //Delete Edge
                EndEdge(curEdge);
                hull.listEdges.RemoveAt(i);
                i -= 1;
            }
            else if (curEdge.couleur == color.violet)
            {
                //We draw a new triangle
                AddTriangle(curEdge.point1, curEdge.point2, pts);
            }
            else
            {
                Debug.Log("Ma bite");
            }
        }
        ResetColor();
    }
    
    void EndFace(Triangle t)
    {
        int index = t.edges1.triangleProprio.IndexOf(t);
        if (index != -1)
        {
            t.edges1.triangleProprio.RemoveAt(index);
        }

        index = t.edges2.triangleProprio.IndexOf(t);
        if (index != -1)
        {
            t.edges2.triangleProprio.RemoveAt(index);
        }

        index = t.edges3.triangleProprio.IndexOf(t);
        if (index != -1)
        {
            t.edges3.triangleProprio.RemoveAt(index);
        }
        GameObject.Destroy(t.mesh);
    }
    

    void EndEdge(Edges e)
    {
        int index = e.point1.edgeProprio.IndexOf(e);
        if (index != -1)
        {
            e.point1.edgeProprio.RemoveAt(index);
        }
        index = e.point2.edgeProprio.IndexOf(e);
        if (index != -1)
        {
            e.point2.edgeProprio.RemoveAt(index);
        }
    }

    int AlreadyInEdges(Edges e)
    {
        int index = -1;
        for (int i = 0; i < convexHull.listEdges.Count; i++)
        {
            if ((e.point1 == convexHull.listEdges[i].point1 ||
              e.point1 == convexHull.listEdges[i].point2 )&&(e.point2 == convexHull.listEdges[i].point1 || e.point2 ==
                convexHull.listEdges[i].point2))
            {
                index = i;
                return index;
            }   
        }
        return index;
    }
    
    int AlreadyInPoint(Point e)
    {
        
        int index = -1;
        for (int i = 0; i < convexHull.listPoints.Count; i++)
        {
            if (e.coordonées == convexHull.listPoints[i].coordonées)
            {
                index = i;
                return index;
            }   
        }
        return index;
    }

    void AddTriangle(Point a ,Point b, Point c)
    {
        GameObject Meshobj = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle = new Triangle(a,b,c);
        Material mat = mats[Random.Range(0,mats.Count)];
        EdgesNTris.drawTri(triangle, Meshobj, mat);
        
        // On verifie les doublons d'edges
        convexHull.listFace.Add(triangle);
        int index = AlreadyInEdges(triangle.edges1);
        if ( index == -1)
        {
            convexHull.listEdges.Add(triangle.edges1);
        }
        else
        {
            triangle.edges1 = convexHull.listEdges[index];
            convexHull.listEdges[index].triangleProprio.Add(triangle);
        }
        
        index = AlreadyInEdges(triangle.edges2);
        if ( index == -1)
        {
            convexHull.listEdges.Add(triangle.edges2);
        }
        else
        {
            triangle.edges2 = convexHull.listEdges[index];
            convexHull.listEdges[index].triangleProprio.Add(triangle);
        }
        index = AlreadyInEdges(triangle.edges3);
        if ( index == -1)
        {
            convexHull.listEdges.Add(triangle.edges3);
        }
        else
        {
            triangle.edges3 = convexHull.listEdges[index];
            convexHull.listEdges[index].triangleProprio.Add(triangle);
        }
        
        //Verification des doublons pour les points
        
        int indexPoint = AlreadyInPoint(triangle.point1);
        if ( indexPoint == -1)
        {
            convexHull.listPoints.Add(triangle.point1);
        }
        else
        {
            triangle.point1 = convexHull.listPoints[indexPoint];
            convexHull.listPoints[indexPoint].edgeProprio.Add(triangle.edges1);
            convexHull.listPoints[indexPoint].edgeProprio.Add(triangle.edges3);
        }
        
        indexPoint = AlreadyInPoint(triangle.point2);
        if ( indexPoint == -1)
        {
            convexHull.listPoints.Add(triangle.point2);
        }
        else
        {
            triangle.point2 = convexHull.listPoints[indexPoint];
            convexHull.listPoints[indexPoint].edgeProprio.Add(triangle.edges1);
            convexHull.listPoints[indexPoint].edgeProprio.Add(triangle.edges2);
        }
        
        indexPoint = AlreadyInPoint(triangle.point3);
        if ( indexPoint == -1)
        {
            convexHull.listPoints.Add(triangle.point3);
        }
        else
        {
            triangle.point3 = convexHull.listPoints[indexPoint];
            convexHull.listPoints[indexPoint].edgeProprio.Add(triangle.edges2);
            convexHull.listPoints[indexPoint].edgeProprio.Add(triangle.edges3);
        }
    }
}
