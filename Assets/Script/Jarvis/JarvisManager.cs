using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class JarvisManager : MonoBehaviour
{
    public List<GameObject> Points = new List<GameObject>();
    private GameObject CurPts;
    public List<GameObject> P = new List<GameObject>();
    public GameObject endPoint;
    private void Start()
    {
        JarvisWalk();
    }

    public GameObject FindLowestX()
    {
        GameObject FirstPts=null;
        float curlowX=10000.0f;
        float curlowY = 10000.0f;
        foreach (GameObject pts in Points)
        {
            if (pts.transform.position.x < curlowX)
            {
                FirstPts = pts;
                var position = FirstPts.transform.position;
                curlowX = position.x;
                curlowY = position.y;
                //Debug.Log(FirstPts.name);
            }
            else if (Math.Abs(pts.transform.position.x - curlowX) < 0.1f)
            {
                if (pts.transform.position.y < curlowY)
                {
                    FirstPts = pts;
                    var position = FirstPts.transform.position;
                    curlowX = position.x;
                    curlowY = position.y;
                    //Debug.Log(FirstPts.name);
                }
            }
        }

        return FirstPts;
    }

    public void JarvisWalk()
    {
        /*CurPts = FindLowestX();
        endPoint.transform.position = Vector3.zero;
        while (true)
        {
            P.Add(CurPts);
            endPoint = Points[0];
            
            for (var j = 1; j<Points.Count; j++)
            {
                if ((endPoint == CurPts) || (CounterClock(CurPts.transform.position, endPoint.transform.position, Points[j].transform.position)<0)) {
                    endPoint = Points[j];
                }
            }
            
            CurPts = endPoint;
            Debug.Log(CurPts.name);
            if (endPoint == P[0])
            {
                Debug.Log("End");
                Debug.Log(endPoint.name);
                break;
            }
        }*/
        Vector2 V = new Vector2(0, -1);
        GameObject I = FindLowestX();
        do
        {
            P.Add(I);
            Debug.Log(I.name);
            GameObject JV;
            GameObject Inew;
            int index = Points.IndexOf(I);
            if (index==0)
            {
                JV = Points[1];
            }
            else
            {
                JV = Points[0];
            }
            
            Vector3 IVT = I.transform.position;
            Vector3 JVT = JV.transform.position;
            
            Vector2 JVI = new Vector2(JVT.x-IVT.x, JVT.y-IVT.y);
            float Amin = VecAngle(V, JVI);
            float Lmax = VecNorm(JVI);
            Debug.Log(Amin);
            Inew = JV;
            
            for(int j = Points.IndexOf(Inew)+1; j<Points.Count; j++)
            {
                if (Points[j] != I)
                {
                    //Debug.Log(Points[j].name);
                    float A = VecAngle(V,new Vector2(I.transform.position.x-Points[j].transform.position.x, I.transform.position.y-Points[j].transform.position.y));
                    float L = VecNorm(new Vector2(I.transform.position.x - Points[j].transform.position.x,
                        I.transform.position.y - Points[j].transform.position.y));
                    if (Amin > A || (Amin==A && Lmax<L))
                    {
                        Amin = A;
                        Lmax = L;
                        Debug.Log("Point looked "+Points[j].name);
                        Debug.Log("Amin after change "+Amin);
                        Inew = Points[j];
                    }
                }
            }

            V = new Vector2(Inew.transform.position.x - I.transform.position.x,
                Inew.transform.position.y - I.transform.position.y);
            I = Inew;
            //Debug.Log(I.name);
            //break;

        } while (I != P[0]);
        
        Debug.Log("end");
    }

    public float VecAngle(Vector2 a, Vector2 b)
    {
        return Mathf.Acos(Vector2.Dot(a, b) / (VecNorm(a) * VecNorm(b)));
    }

    public float VecNorm(Vector2 a)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x,2)+Mathf.Pow(a.y, 2));
    }
    
    /*public static float CounterClock(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Mathf.Acos((p2.x - p1.x) * (p3.z - p1.z) - (p3.x - p1.x) * (p2.z - p1.z));
    }*/
}
