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
    
    
    public Triangle()
    {
        point1 = new Point();
        point2 = new Point();
        point3 = new Point();
        edges1 = new Edges();
        edges2 = new Edges();
        edges3 = new Edges();
        normal = new Vector3();


    }
    
    public Triangle(Point pts1 , Point pts2 , Point pts3)
    {
        point1 = pts1;
        point2 = pts2;
        point3 = pts3;
        edges1 = new Edges(point1, point2);
        edges2 = new Edges(point2, point3);
        edges3 = new Edges(point3, point1);
        normal = Vector3.zero;

    }

    Vector3 getNormale(Point externe)
    {
        Vector3 n=Vector3.Cross(point1 - point2, point1- point3);

        if (Vector3.Dot(n, point1 - externe) > 0) return -n;
        return n;
    }
}

public class Edges
{
    public Point point1;
    public Point point2;
    public color couleur = color.blanc;

    public Edges()
    {
        point1 = new Point();
        point2 = new Point();
    }
    public Edges(Point pts1, Point pts2)
    {
        point1 = pts1;
        point2 = pts2;
    }
}

[Serializable]
public class Point
{
    public Vector3 coordonées;
    public Vector3 normal;
    public color couleur = color.blanc;

    
    public Point()
    {
        this.coordonées = new Vector3();
        this.normal = Vector3.Normalize(coordonées);
    }
   public Point(Vector3 coord)
   {
       this.coordonées = coord;
       this.normal = Vector3.Normalize(coordonées);
   }
   
   public static Vector3 operator -(Point a, Point b)
   {
       return a.coordonées - b.coordonées;
   }
}


public class ConvexHull
{
    public List<Point> listPoints;
    public List<Vector3> listPointsNormalized;
    public List<Edges> listEdges;
    public List<Triangle> listFace;
    
    public ConvexHull()
    {
        this.listPoints = new List<Point>();
        
        this.listPointsNormalized = new List<Vector3>();
        
        this.listEdges = new List<Edges>();

        this.listFace = new List<Triangle>();
    }
}
