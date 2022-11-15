using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulationDelauney : MonoBehaviour
{
    public static List<TScript.Triangle> TriangulationFlippingEdges(List<TScript.Triangle> tri)
    {
        
        List<TScript.Triangle> triDelauney = new List<TScript.Triangle>(tri);

        List<TScript.HalfEdge> halfEdges = HalfEdge.TransformTriangleToHeHalfEdges(triDelauney);

        int isLooping = 0;

        int flippingEdges = 0;

        while (true)
        {
            isLooping += 1;

            if (isLooping > 10000)
            {
                Debug.Log("BOUCLE INFINIE ");
                break;
            }

            bool isFlip = false;

            for (int i = 0; i < halfEdges.Count; i++)
            {
                TScript.HalfEdge thisEdge = halfEdges[i];

                if (thisEdge.oppositeEdge == null)
                {
                    continue;
                }

                TScript.Vertex a = thisEdge.v;
                TScript.Vertex b = thisEdge.nextEdge.v;
                TScript.Vertex c = thisEdge.prevEdge.v;
                TScript.Vertex d = thisEdge.oppositeEdge.nextEdge.v;

                Vector2 aPos = a.position;
                Vector2 bPos = b.position;
                Vector2 cPos = c.position;
                Vector2 dPos = d.position;

                if (IsPointInsideOutsideOrOnCircle(aPos, bPos, cPos, dPos) < 0f)
                {
                    if (IsQuadrilateralConvex(aPos, bPos, cPos, dPos))
                    {
                        if (IsPointInsideOutsideOrOnCircle(bPos, cPos, dPos, aPos) < 0f)
                        {
                            continue;
                        }

                        flippingEdges += 1;

                        isFlip = true;

                        FlipEdge(thisEdge);
                    }
                }
            }

            if (!isFlip)
            {
                break;
            }
        }

        return triDelauney;
    }
    
    public static float IsPointInsideOutsideOrOnCircle(Vector2 aVec, Vector2 bVec, Vector2 cVec, Vector2 dVec)
    {
        float a = aVec.x - dVec.x;
        float d = bVec.x - dVec.x;
        float g = cVec.x - dVec.x;

        float b = aVec.y - dVec.y;
        float e = bVec.y - dVec.y;
        float h = cVec.y - dVec.y;

        float c = a * a + b * b;
        float f = d * d + e * e;
        float i = g * g + h * h;

        float determinant = (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);

        return determinant;
    }
    
    public static bool IsQuadrilateralConvex(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        bool isConvex = false;

        bool abc = HalfEdge.IsTriangleOrientedClockwise(a, b, c);
        bool abd = HalfEdge.IsTriangleOrientedClockwise(a, b, d);
        bool bcd = HalfEdge.IsTriangleOrientedClockwise(b, c, d);
        bool cad = HalfEdge.IsTriangleOrientedClockwise(c, a, d);

        if (abc && abd && bcd & !cad)
        {
            isConvex = true;
        }
        else if (abc && abd && !bcd & cad)
        {
            isConvex = true;
        }
        else if (abc && !abd && bcd & cad)
        {
            isConvex = true;
        }
        //The opposite sign, which makes everything inverted
        else if (!abc && !abd && !bcd & cad)
        {
            isConvex = true;
        }
        else if (!abc && !abd && bcd & !cad)
        {
            isConvex = true;
        }
        else if (!abc && abd && !bcd & !cad)
        {
            isConvex = true;
        }


        return isConvex;
    }
    
    private static void FlipEdge(TScript.HalfEdge one)
    {
        //The data we need
        //This edge's triangle
        TScript.HalfEdge two = one.nextEdge;
        TScript.HalfEdge three = one.prevEdge;
        //The opposite edge's triangle
        TScript.HalfEdge four = one.oppositeEdge;
        TScript.HalfEdge five = one.oppositeEdge.nextEdge;
        TScript.HalfEdge six = one.oppositeEdge.prevEdge;
        //The vertices
        TScript.Vertex a = one.v;
        TScript.Vertex b = one.nextEdge.v;
        TScript.Vertex c = one.prevEdge.v;
        TScript.Vertex d = one.oppositeEdge.nextEdge.v;



        //Flip

        //Change vertex
        a.halfEdge = one.nextEdge;
        c.halfEdge = one.oppositeEdge.nextEdge;

        //Change half-edge
        //Half-edge - half-edge connections
        one.nextEdge = three;
        one.prevEdge = five;

        two.nextEdge = four;
        two.prevEdge = six;

        three.nextEdge = five;
        three.prevEdge = one;

        four.nextEdge = six;
        four.prevEdge = two;

        five.nextEdge = one;
        five.prevEdge = three;

        six.nextEdge = two;
        six.prevEdge = four;

        //Half-edge - vertex connection
        one.v = b;
        two.v = b;
        three.v = c;
        four.v = d;
        five.v = d;
        six.v = a;

        //Half-edge - triangle connection
        TScript.Triangle t1 = one.t;
        TScript.Triangle t2 = four.t;

        one.t = t1;
        three.t = t1;
        five.t = t1;

        two.t = t2;
        four.t = t2;
        six.t = t2;

        //Opposite-edges are not changing!

        //Triangle connection
        t1.v1 = b;
        t1.v2 = c;
        t1.v3 = d;

        t2.v1 = b;
        t2.v2 = d;
        t2.v3 = a;

        t1.halfEdge = three;
        t2.halfEdge = four;
    }
    
}
