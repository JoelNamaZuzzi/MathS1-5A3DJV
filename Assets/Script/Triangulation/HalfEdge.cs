using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfEdge : MonoBehaviour
{
    public static List<TScript.HalfEdge> TransformTriangleToHeHalfEdges(List<TScript.Triangle> triangles)
    {
        OrientTrianglesClockwise(triangles);

        List < TScript.HalfEdge > halfEdges = new List<TScript.HalfEdge>(triangles.Count * 3);


        for (int i = 0; i < triangles.Count; i++)
        {
            TScript.Triangle t = triangles[i];
            TScript.HalfEdge h1 = new TScript.HalfEdge(t.v1);
            TScript.HalfEdge h2 = new TScript.HalfEdge(t.v2);
            TScript.HalfEdge h3 = new TScript.HalfEdge(t.v3);

            h1.nextEdge = h2;
            h2.nextEdge = h3;
            h3.nextEdge = h1;
            
            h1.prevEdge = h3;
            h2.prevEdge = h1;
            h3.prevEdge = h2;
            
            h1.v.halfEdge = h2;
            h2.v.halfEdge = h3;
            h3.v.halfEdge = h1;
            
            t.halfEdge = h1;

            h1.t = t;
            h2.t = t;
            h3.t = t;

            //Add the half-edges to the list
            halfEdges.Add(h1);
            halfEdges.Add(h2);
            halfEdges.Add(h3);
        }

        for (int i = 0; i < halfEdges.Count; i++)
        {
            TScript.HalfEdge h = halfEdges[i];

            TScript.Vertex ToVertex = h.v;
            TScript.Vertex FromVertex = h.prevEdge.v;

            for (int j = 0; j < halfEdges.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                TScript.HalfEdge hOpposite = halfEdges[j];

                if (FromVertex.position == hOpposite.v.position && ToVertex.position == hOpposite.prevEdge.v.position)
                {
                    h.oppositeEdge = hOpposite;
                    break;
                }
            }
        }

        return halfEdges;
    }
    
    public static void OrientTrianglesClockwise(List<TScript.Triangle> triangles)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            TScript.Triangle tri = triangles[i];

            Vector2 v1 = new Vector2(tri.v1.position.x, tri.v1.position.z);
            Vector2 v2 = new Vector2(tri.v2.position.x, tri.v2.position.z);
            Vector2 v3 = new Vector2(tri.v3.position.x, tri.v3.position.z);

            if (!IsTriangleOrientedClockwise(v1, v2, v3))
            {
                tri.ChangeOrientation();
            }
        }
    }
    
    public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool isClockWise = true;

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        if (determinant > 0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }
}
