using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Triangle
{
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public Edges edges1;
    public Edges edges2;
    public Edges edges3;

    
    public Triangle()
    {
        point1 = new GameObject();
        point2 = new GameObject();
        point3 = new GameObject();
        edges1 = new Edges();
        edges2 = new Edges();
        edges3 = new Edges();
    }
    
    public Triangle(GameObject pts1 , GameObject pts2 , GameObject pts3)
    {
        point1 = pts1;
        point2 = pts2;
        point3 = pts3;
        edges1 = new Edges(point1, point2);
        edges2 = new Edges(point2, point3);
        edges3 = new Edges(point3, point1);
    }
    
}

public class Edges
{
    public GameObject point1;
    public GameObject point2;

    public Edges()
    {
        point1 = new GameObject();
        point2 = new GameObject();
    }
    public Edges(GameObject pts1, GameObject pts2)
    {
        point1 = pts1;
        point2 = pts2;
    }
}

public class ConvexHull
{
    public List<Vector3> listPoints;
    public List<Edges> listEdges;
    public List<Triangle> listFace;
    
    public ConvexHull()
    {
        this.listPoints = new List<Vector3>();

        this.listEdges = new List<Edges>();

        this.listFace = new List<Triangle>();
    }
}
