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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && dl.isIncre)
        {
            delaunayTriangle = TriangulationDelauney.TriangulationFlippingEdges(dl.triangles);
            voronoi = VoronoiDiagram(delaunayTriangle);
           
        }
    }

    

    public List<TScript.VoronoiRegion> VoronoiDiagram(List<TScript.Triangle> delaunayTriangulation)
    {
        List<TScript.VoronoiEdge> voronoiEdges = new List<TScript.VoronoiEdge>();

        // On genere les voronoi edge par rapport aux edge recuperer de la triangulation de delaunay
        for (int i = 0; i < delaunayTriangulation.Count; i++)
        {
            // On recupere les half edges du triangle 
            TScript.HalfEdge edge1 = delaunayTriangulation[i].halfEdge;
            TScript.HalfEdge edge2 = edge1.nextEdge;
            TScript.HalfEdge edge3 = edge2.nextEdge;

            //On recupere al position des 3 points pour le calcul du centre du cercle circonscrit

            Vector3 pos1 = edge1.v.position;
            Vector3 pos2 = edge2.v.position;
            Vector3 pos3 = edge3.v.position;

            Vector3 cercleCenter = CalculateCenterCircle(pos1, pos2, pos3);

            Vector3 voronoiSommet = new Vector3(cercleCenter.x, cercleCenter.y, cercleCenter.z);
            // Instantiate(voronoiPrefab, voronoiSommet, Quaternion.identity);

            //On verifie les 3 edges du triangle si il y  a d'autre triangles aux alentour pour determiner les liaisons

            CheckTriangle(edge1, voronoiSommet, voronoiEdges);
            CheckTriangle(edge2, voronoiSommet, voronoiEdges);
            CheckTriangle(edge3, voronoiSommet, voronoiEdges);
        }

        // on determine les régions d'incidence

        List<TScript.VoronoiRegion> ListeVoronoiRegions = new List<TScript.VoronoiRegion>();

        for (int i = 0; i < voronoiEdges.Count; i++)
        {
            //On verifie si il existe déja la région a laquel appartient l'edge

            int numeroRegion = checkIfRegion(voronoiEdges[i], ListeVoronoiRegions);

            if (numeroRegion == -1)
            {
                TScript.VoronoiRegion newRegion = new TScript.VoronoiRegion(voronoiEdges[i].noyau);
                ListeVoronoiRegions.Add(newRegion);
                newRegion.edges.Add(voronoiEdges[i]);
            }
            else
            {
                ListeVoronoiRegions[numeroRegion].edges.Add(voronoiEdges[i]);
            }
        }

        return ListeVoronoiRegions;
    }

    public Vector3 CalculateCenterCircle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 center = new Vector3();

        float segmentAB = (pointB.y - pointA.y) / (pointB.x - pointA.x);
        float segmentBC = (pointC.y - pointB.y) / (pointC.x - pointB.x);

        center.x = (segmentAB * segmentBC * (pointA.y - pointC.y) + segmentBC * (pointA.x + pointB.x) -
                    segmentAB * (pointB.x + pointC.x)) / (2 * (segmentBC - segmentAB));

        center.y = (-1 / segmentAB) * (center.x - (pointA.x + pointB.x) / 2) + (pointA.y + pointB.y) / 2;
        center.z = 20.3f;
        return center;
    }

    public void CheckTriangle(TScript.HalfEdge edge, Vector3 voronoiSommet,
        List<TScript.VoronoiEdge> ListAllVoronoiEdge)
    {
        //On verifie si il y a un triangle voisin de l'arete
        // Si : Non , il n'y a pas de voisin donc on ne peut pas créer de Voronoi Edge
        if (edge.oppositeEdge == null)
        {
            return;
        }
        //SI oui , on calcule le centre de son cercle 

        // on recup les coordonées des sommet du triangle voisin
        TScript.HalfEdge edgeVoisin = edge.oppositeEdge;
        Vector3 pos1 = edgeVoisin.v.position;
        Vector3 pos2 = edgeVoisin.nextEdge.v.position;
        Vector3 pos3 = edgeVoisin.nextEdge.nextEdge.v.position;

        Vector3 centreCercle = CalculateCenterCircle(pos1, pos2, pos3);

        Vector3 voronoiSommetVoisin = new Vector3(centreCercle.x, centreCercle.y, centreCercle.z);
        Instantiate(voronoiPrefab, voronoiSommetVoisin, Quaternion.identity);

        TScript.VoronoiEdge vorEdge =
            new TScript.VoronoiEdge(voronoiSommet, voronoiSommetVoisin, edge.prevEdge.v.position);
        ListAllVoronoiEdge.Add(vorEdge);

    }


    // On verifie si l'edge passé en parametre appartient a une région déjà enrtegistré 
    public int checkIfRegion(TScript.VoronoiEdge edge, List<TScript.VoronoiRegion> listeRegion)
    {
        for (int i = 0; i < listeRegion.Count; i++)
        {
            if (edge.noyau == listeRegion[i].noyau)
            {
                return i;
            }
        }

        return -1;
    }

   
}
