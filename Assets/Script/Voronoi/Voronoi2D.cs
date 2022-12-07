using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Voronoi2D : MonoBehaviour
{
    public DrawLine dl;
    private List<TScript.Triangle> delaunayTriangle;
    private List<TScript.VoronoiRegion> voronoi;
    [SerializeField] private GameObject voronoiPrefab;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private List<GameObject> cells;
    [SerializeField] private List<GameObject> voronoidPoints;
    [SerializeField] private List<Vector3> centerList = new List<Vector3>();
    public GameObject pref;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            delaunayTriangle = TriangulationDelauney.TriangulationFlippingEdges(dl.trianglesDelauney);
            VoronoiDiagram(delaunayTriangle);
        }
    }


    public void VoronoiDiagram(List<TScript.Triangle> delaunayTriangulation)
    {
        List<TScript.VoronoiEdge> voronoiEdges = new List<TScript.VoronoiEdge>();

        Debug.Log("nb triangle :" + delaunayTriangulation.Count);

        if (voronoidPoints.Count > 0)
        {
            for (int p_nb = 0; p_nb < voronoidPoints.Count; p_nb++)
            {
                Destroy(voronoidPoints[p_nb]);
            }
        }

        if (cells.Count > 0)
        {
            for (int c_nb = 0; c_nb < cells.Count; c_nb++)
            {
                Destroy(cells[c_nb]);
            }
        }

        // On genere les point du voronoi par rapport aux edge recuperer de la triangulation de delaunay
        for (int i = 0; i < delaunayTriangulation.Count; i++)
        {
            // On recupere les half edges du triangle 
            TScript.HalfEdge edge1 = delaunayTriangulation[i].halfEdge;
            TScript.HalfEdge edge2 = edge1.nextEdge;
            TScript.HalfEdge edge3 = edge2.nextEdge;
            //On recupere la position des 3 points pour le calcul du centre du cercle circonscrit

            Vector3 pos1 = edge1.v.position;
            Vector3 pos2 = edge2.v.position;
            Vector3 pos3 = edge3.v.position;

            bool Colineaire1 = IsColineaire(pos1, pos2);
            bool Colineaire2 = IsColineaire(pos2, pos3);
            bool Colineaire3 = IsColineaire(pos3, pos1);

            /*  if (Colineaire1 && Colineaire2 && Colineaire3)
              {
                  Vector3 AB = new Vector3((pos2.x - pos1.x) , (pos2.y - pos1.y),20.3f);
                  Vector3 BC = new Vector3((pos3.x - pos2.x) , (pos3.y - pos2.y),20.3f);
                  Vector3 perp = Vector3.Cross(AB, BC);
                  Instantiate(pref, perp, Quaternion.identity);
              } */
            float segmentAB = (pos2.y - pos1.y) / (pos2.x - pos1.x);
            float segmentBC = (pos3.y - pos2.y) / (pos3.x - pos2.x);

            if (segmentBC - segmentAB == 0)
            {
                Debug.Log("point parrallele");
            }
            else
            {
                Vector3 cercleCenter = CalculateCenterCircle(segmentAB, segmentBC, pos1, pos2, pos3);
                Vector3 voronoiSommet = new Vector3(cercleCenter.x, cercleCenter.y, cercleCenter.z);
                centerList.Add(voronoiSommet);

                GameObject centerCircle = Instantiate(voronoiPrefab, voronoiSommet, Quaternion.identity);
                voronoidPoints.Add(centerCircle);
                centerCircle.name = "voroinoi Vertice N°" + i;
            }

            // On calcul les mediatrice du triangle actuel
            Vector3 mediatrice1 = CalculMediatrice(pos1, pos2);
            Vector3 mediatrice2 = CalculMediatrice(pos2, pos3);
            Vector3 mediatrice3 = CalculMediatrice(pos3, pos1);

            //On verifie les 3 edges du triangle si il y a d'autre triangles aux alentour pour determiner les liaisons
            Debug.Log("edge 1" + edge1.v.position + edge1.nextEdge.v.position);
            CheckTriangle(edge1, centerList[i],  mediatrice1);
            Debug.Log("edge 2" + edge2.v.position + edge2.nextEdge.v.position);
            CheckTriangle(edge2, centerList[i],  mediatrice2);
            Debug.Log("edge 3" + edge3.v.position + edge3.nextEdge.v.position);
            CheckTriangle(edge3, centerList[i],  mediatrice3);
        }
    }


    public Vector3 CalculateCenterCircle(float segmentAB, float segmentBC, Vector3 pointA, Vector3 pointB,
        Vector3 pointC)
    {
        Vector3 center = new Vector3();
        center.x = (segmentAB * segmentBC * (pointA.y - pointC.y) + segmentBC * (pointA.x + pointB.x) -
                    segmentAB * (pointB.x + pointC.x)) / (2 * (segmentBC - segmentAB));

        center.y = (-1 / segmentAB) * (center.x - (pointA.x + pointB.x) / 2) + (pointA.y + pointB.y) / 2;

        center.z = 20.3f;

        return center;
    }

    public void CheckTriangle(TScript.HalfEdge edge, Vector3 voronoiSommet, Vector3 mediatrice)
    {
        //On verifie si il y a un triangle voisin de l'arete
        // Si : Non , il n'y a pas de voisin donc on relie a l'intersection entre l'edge et la mediatrice
        if (edge.oppositeEdge == null)
        {
           
            Debug.Log("pas de voisin");
            return;
        }
        DrawLineVoronoi(voronoiSommet, mediatrice);
        //SI oui , on calcule le centre de son cercle 
        // on recup les coordonées des sommet du triangle voisin

        TScript.HalfEdge edgeVoisin = edge.oppositeEdge;
        Vector3 pos1 = edgeVoisin.v.position;
        Vector3 pos2 = edgeVoisin.nextEdge.v.position;
        Vector3 pos3 = edgeVoisin.nextEdge.nextEdge.v.position;

        float segmentAB = (pos2.y - pos1.y) / (pos2.x - pos1.x);
        float segmentBC = (pos3.y - pos2.y) / (pos3.x - pos2.x);

        if (segmentBC - segmentAB == 0)
        {
            Debug.Log("points parallèle , impossible de créer un cercle circonscrit");
        }
        else
        {
            Vector3 voronoiSommetVoisin = CalculateCenterCircle(segmentAB, segmentBC, pos1, pos2, pos3);

            Debug.Log("triangle d'a coté : " + pos1 + " " + pos2 + " " + pos3);
            Debug.Log("Centre cercle vosiin :" + voronoiSommetVoisin);

            
            DrawLineVoronoi(voronoiSommet, voronoiSommetVoisin);
        
        }
    }

    private Vector3 CalculMediatrice(Vector3 point1, Vector3 point2)
    {
        Vector3 med = new Vector3();
        med.x = (point1.x + point2.x) / 2;
        med.y = (point1.y + point2.y) / 2;
        med.z = 20.3f;
        return med;
    }

    private void DrawLineVoronoi(Vector3 centre, Vector3 mediatrice)
    {
        GameObject cellRenderer = Instantiate(cellPrefab, centre, Quaternion.identity);
        cells.Add(cellRenderer);
        LineRenderer lr = cellRenderer.GetComponent<LineRenderer>();
        lr.SetPosition(0, centre);
        lr.SetPosition(1, mediatrice);
    }

    public bool IsColineaire(Vector3 a, Vector3 b)
    {
        return (a - b).magnitude < 0.000001f;
    }
}