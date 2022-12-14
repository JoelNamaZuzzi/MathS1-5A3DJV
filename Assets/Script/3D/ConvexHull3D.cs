using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;


public class ConvexHull3D : MonoBehaviour
{
    public List<Point> listePoints = new List<Point>();
    
    public GameObject lnrdr;
    public GameObject meshObj;
    public ConvexHull convexHull;

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
        if (listePoints.Count < 3)
        {
            Debug.Log("Il nous faut 4 points au min");
        }
        
        DrawTetrahedre();
        foreach (Point pts in listePoints)
        {
            
            bool inside = TestInteriorite(pts);
            if (inside)
            {
                Debug.Log(pts.coordonées+"interieur");
            }
            else
            {
                
                Debug.Log(pts.coordonées+ "exterieur");
                CheckVisibilité(pts);
               // UpdateHull(pts,convexHull);
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
        List<Edges> visibleEdges = new List<Edges>();
        foreach (Triangle face in hull.listFace)
        {
            if (face.couleur == color.bleu)
            {
                visibleEdges.Add(face.edges1);
                visibleEdges.Add(face.edges2);
                visibleEdges.Add(face.edges3);
            }
        }
        foreach (Triangle face in hull.listFace)
        {
            if (face.couleur == color.bleu)
            {
                int count = visibleEdges.Where(x => x.Equals(face.edges1)).Count();
                if (count > 1)
                {
                    face.edges1 =null;
                }
                count = visibleEdges.Where(x => x.Equals(face.edges2)).Count();
                if (count > 1)
                {
                    face.edges2 =null;
                }
                count = visibleEdges.Where(x => x.Equals(face.edges3)).Count();
                if (count > 1)
                {
                    face.edges3 =null;
                }
            }
        }
        foreach (Triangle face in hull.listFace)
        {
            if (face.couleur == color.bleu)
            {
                
                
                //we have to test independently each edges
                if (face.edges1 != null && face.edges1.couleur == color.violet)
                {
                    //Create triangle
                    GameObject Meshobj = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
                    Triangle newTriangle = new Triangle(pts,face.edges1.point1,face.edges1.point2);
                    Material mat = mats[Random.Range(0,mats.Count)];
                    EdgesNTris.drawTri(newTriangle, Meshobj, mat);
                    
                    hull.listFace.Add(newTriangle);
                    hull.listEdges.Add(newTriangle.edges1);
                    hull.listEdges.Add(newTriangle.edges2);
                    hull.listEdges.Add(newTriangle.edges3);

                    hull.listPoints.Add(newTriangle.point1);
                    hull.listPoints.Add(newTriangle.point2);
                    hull.listPoints.Add(newTriangle.point3);
                }
                else
                {
                    Debug.Log("Edge is Red");
                }
                if (face.edges2 != null && face.edges2.couleur == color.violet)
                {
                    //Create triangle
                    GameObject Meshobj = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
                    Triangle newTriangle = new Triangle(pts,face.edges2.point1,face.edges2.point2);
                    Material mat = mats[Random.Range(0,mats.Count)];
                    EdgesNTris.drawTri(newTriangle, Meshobj, mat);
                    
                    hull.listFace.Add(newTriangle);
                    hull.listEdges.Add(newTriangle.edges1);
                    hull.listEdges.Add(newTriangle.edges2);
                    hull.listEdges.Add(newTriangle.edges3);

                    hull.listPoints.Add(newTriangle.point1);
                    hull.listPoints.Add(newTriangle.point2);
                    hull.listPoints.Add(newTriangle.point3);
                }
                else
                {
                    Debug.Log("Edge is Red");
                }
                if (face.edges3 != null && face.edges3.couleur == color.violet)
                {
                    //Create triangle
                    GameObject Meshobj = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
                    Triangle newTriangle = new Triangle(pts,face.edges3.point1,face.edges3.point2);
                    Material mat = mats[Random.Range(0,mats.Count)];
                    EdgesNTris.drawTri(newTriangle, Meshobj, mat);
                    
                    hull.listFace.Add(newTriangle);
                    hull.listEdges.Add(newTriangle.edges1);
                    hull.listEdges.Add(newTriangle.edges2);
                    hull.listEdges.Add(newTriangle.edges3);

                    hull.listPoints.Add(newTriangle.point1);
                    hull.listPoints.Add(newTriangle.point2);
                    hull.listPoints.Add(newTriangle.point3);
                }
                else
                {
                    Debug.Log("Edge is Red");
                }
                int index = hull.listFace.IndexOf(face);
                EndFace(face);
                hull.listFace.RemoveAt(index);
            }
            else
            {
                Debug.Log("face is red");
            }
          
        }
    }
    
    void EndFace(Triangle t)
    {
        int index = t.edges1.triangleProprio.IndexOf(t);
        t.edges1.triangleProprio.RemoveAt(index); 
            
        index = t.edges2.triangleProprio.IndexOf(t);
        t.edges2.triangleProprio.RemoveAt(index);
            
        index = t.edges3.triangleProprio.IndexOf(t);
        t.edges3.triangleProprio.RemoveAt(index);
        
        Destroy(t.mesh);
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
        if ( index == -1)
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
