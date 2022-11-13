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
                Debug.Log(FirstPts.name);
            }
            else if (Math.Abs(pts.transform.position.x - curlowX) < 0.1f)
            {
                if (pts.transform.position.y < curlowY)
                {
                    FirstPts = pts;
                    var position = FirstPts.transform.position;
                    curlowX = position.x;
                    curlowY = position.y;
                    Debug.Log(FirstPts.name);
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
            
            Vector2 JVI = new Vector2(JVT.x-IVT.x, JVT.z-IVT.z);
            float Amin = VecAngle(V, JVI);
            Debug.Log(Amin);
            Inew = JV;
            
            for(int j = Points.IndexOf(Inew)+1; j<Points.Count; j++)
            {
                if (Points[j] != I)
                {
                    Debug.Log(Points[j].name);
                    float A = VecAngle(V,new Vector2(I.transform.position.x-Points[j].transform.position.x, I.transform.position.z-Points[j].transform.position.z));
                    if (Amin > A)
                    {
                        Amin = A;
                        Inew = Points[j];
                    }
                }
            }

            V = new Vector3(Inew.transform.position.x - I.transform.position.x,
                Inew.transform.position.z - I.transform.position.z);
            I = Inew;
            Debug.Log(I.name);
            //break;

        } while (I != P[0]);
        
        Debug.Log("end");
    }

    public float VecAngle(Vector2 a, Vector2 b)
    {
        float res;
        res = Mathf.Acos((Vector2.Dot(a, b) / (a.magnitude * b.magnitude)));
        return res;
    }
    
    /*public static float CounterClock(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Mathf.Acos((p2.x - p1.x) * (p3.z - p1.z) - (p3.x - p1.x) * (p2.z - p1.z));
    }*/
    

}
