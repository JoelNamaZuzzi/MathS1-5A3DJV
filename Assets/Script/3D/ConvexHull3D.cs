using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;


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
        Material mat = mats[Random.Range(0,mats.Count)];
        EdgesNTris.drawTri(triangle1, Meshobj, mat);
        
        GameObject Meshobj2 = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle2 = new Triangle(listePoints[0],listePoints[3],listePoints[1]);
        mat = mats[Random.Range(0,mats.Count)];
        EdgesNTris.drawTri(triangle2, Meshobj2, mat);
        
        GameObject Meshobj3 = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle3 = new Triangle(listePoints[0],listePoints[2],listePoints[3]);
        mat = mats[Random.Range(0,mats.Count)];
        EdgesNTris.drawTri(triangle3, Meshobj3, mat);
        
        GameObject Meshobj4 = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Triangle triangle4 = new Triangle(listePoints[1],listePoints[2],listePoints[3]);
        mat = mats[Random.Range(0,mats.Count)];
        EdgesNTris.drawTri(triangle4, Meshobj4, mat);
        
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

    public void UpdateHull(Point pts, ConvexHull hull)
    {
        //List<Edges> visibleEdges = new List<Edges>();
        /*foreach (Triangle face in hull.listFace)
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
                hull.listFace.RemoveAt(hull.listFace.IndexOf(face));
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
            }
            else
            {
                Debug.Log("face is red");
            }
            ResetColor();
        }*/
        for (int i = 0; i < hull.listFace.Count(); i++)
        {
            Triangle curFace = hull.listFace[i];
            //We only interact with blue ones
            if (curFace.couleur == color.bleu)
            {
                //Delete Face
            }
        }
        for (int i = 0; i < hull.listPoints.Count(); i++)
        {
            Point curPts = hull.listPoints[i];
            //We only look at blue pts, red & purple one aren't useful here
            if (curPts.couleur == color.bleu)
            {
                //Delete Pts
            }
        }
        for (int i = 0; i<hull.listEdges.Count(); i++)
        {
            Edges curEdge = hull.listEdges[i];
            //We don't look at red ones here
            if (curEdge.couleur == color.bleu)
            {
                //Delete Edge
            }else if (curEdge.couleur == color.violet)
            {
                //We draw a new triangle
                GameObject mesh = Instantiate(meshObj, new Vector3(0f, 0f, 0f),Quaternion.Euler(0f,0f,0f));
                Triangle triangle = new Triangle(curEdge.point1, curEdge.point2, pts);
                Material mat = mats[Random.Range(0,mats.Count)];
                EdgesNTris.drawTri(triangle, meshObj, mat);

                hull.listFace.Add(triangle);
                hull.listEdges.Add(triangle.edges1);
                hull.listEdges.Add(triangle.edges2);
                hull.listEdges.Add(triangle.edges3);

                hull.listPoints.Add(triangle.point1);
                hull.listPoints.Add(triangle.point2);
                hull.listPoints.Add(triangle.point3);
            }
        }
    }
    
}
