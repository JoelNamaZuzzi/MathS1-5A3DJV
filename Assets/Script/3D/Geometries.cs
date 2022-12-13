using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public enum color
{
    blanc,
    rouge,
    bleu,
    violet
};

[Serializable]
public class Triangle
{
    public Point point1;
    public Point point2;
    public Point point3;
    public Edges edges1;
    public Edges edges2;
    public Edges edges3;
    public Vector3 normal;
    public color couleur = color.blanc;
    [NonSerialized]public GameObject mesh;
    
    
    
    public Triangle()
    {
        point1 = new Point();
        point2 = new Point();
        point3 = new Point();
        edges1 = new Edges();
        edges1.triangleProprio.Add(this);
        edges2 = new Edges();
        edges2.triangleProprio.Add(this);
        edges3 = new Edges();
        edges3.triangleProprio.Add(this);
        normal = new Vector3();
    }
    
    public Triangle(Point pts1 , Point pts2 , Point pts3)
    {
        point1 = pts1;
        point2 = pts2;
        point3 = pts3;
        edges1 = new Edges(point1, point2);
        edges1.triangleProprio.Add(this);
        edges2 = new Edges(point2, point3);
        edges2.triangleProprio.Add(this);
        edges3 = new Edges(point3, point1);
        edges3.triangleProprio.Add(this);
        normal = Vector3.zero;
    }

    public Vector3 getNormale()
    {
        Vector3 n=Vector3.Cross(point1 - point2, point1- point3);

        Point externe = null;
        
        foreach (var e in point1.edgeProprio)
        {
            if (e.point1 != point1 && e.point1 != point2 && e.point1 != point3)
            {
                externe = e.point1;
                break;
            }
            if(e.point2 != point1 && e.point2 != point2 && e.point2 != point3)
            {
                externe = e.point2;
                break;
            }
        }

        if (externe == null)
        {
            Debug.Log("Impossible de verifier, il ny a qu'un triangle");
            return n;
        }

        if (Vector3.Dot(n, point1 - externe) > 0)
        {
            normal = -n;
            return -n;
        }

        normal = n;
        return n;
    }
    
    
    public void SetNormale()
    {
        Vector3 n=Vector3.Cross(point1 - point2, point1- point3);

        Point externe = null;
        
        foreach (var e in point1.edgeProprio)
        {
            if (e.point1 != point1 && e.point1 != point2 && e.point1 != point3)
            {
                externe = e.point1;
                break;
            }
            if(e.point2 != point1 && e.point2 != point2 && e.point2 != point3)
            {
                externe = e.point2;
                break;
            }
        }

        if (externe == null)
        {
            Debug.Log("Impossible de verifier, il ny a qu'un triangle");
            return ;
        }
        if (Vector3.Dot(n, point1 - externe) > 0) normal= -n;
        normal = n;
    }
    

    public void SetMesh(GameObject m)
    {
        this.mesh = m;
    }
    
    
}

[Serializable]
public class Edges
{
    [NonSerialized]public List<Triangle> triangleProprio;
    public Point point1;
    public Point point2;
    public color couleur = color.blanc;

    public Edges()
    {
        point1 = new Point();
        point1.edgeProprio.Add(this);
        point2 = new Point();
        point2.edgeProprio.Add(this);
        triangleProprio = new List<Triangle>();
    }
    public Edges(Point pts1, Point pts2)
    {
        point1 = pts1;
        point1.edgeProprio.Add(this);
        point2 = pts2;
        point2.edgeProprio.Add(this);
        triangleProprio = new List<Triangle>();
    }
}

[Serializable]
public class Point
{
    [NonSerialized]public List<Edges> edgeProprio;
    public Vector3 coordonées;
    public Vector3 normal;
    public color couleur = color.blanc;
    public GameObject mesh;
    public bool IsMovable;
    
    public Point()
    {
        this.coordonées = new Vector3();
        this.normal = Vector3.Normalize(coordonées);
        this.edgeProprio = new List<Edges>();
    }
   public Point(Vector3 coord)
   {
       this.coordonées = coord;
       this.normal = Vector3.Normalize(coordonées);
       this.edgeProprio = new List<Edges>();
   }
   
   public static Vector3 operator -(Point a, Point b)
   {
       return a.coordonées - b.coordonées;
   }

   public void SetMesh(GameObject m)
   {
       this.mesh = m;
   }
}

[Serializable]
public class ConvexHull
{
    public List<Point> listPoints;
    public List<Edges> listEdges;
    public List<Triangle> listFace;
    
    public ConvexHull()
    {
        this.listPoints = new List<Point>();

        this.listEdges = new List<Edges>();

        this.listFace = new List<Triangle>();
    }
    
    
}
