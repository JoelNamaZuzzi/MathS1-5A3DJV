using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarvisManager : MonoBehaviour
{
    public List<GameObject> Points = new List<GameObject>();
    private GameObject CurPts;

    private void Start()
    {
        FindLowestX();
    }

    public void FindLowestX()
    {
        float curlowX=10000.0f;
        float curlowY = 10000.0f;
        foreach (GameObject pts in Points)
        {
            if (pts.transform.position.x < curlowX)
            {
                CurPts = pts;
                var position = CurPts.transform.position;
                curlowX = position.x;
                curlowY = position.y;
                Debug.Log(CurPts.name);
            }
            else if (Math.Abs(pts.transform.position.x - curlowX) < 0.1f)
            {
                if (pts.transform.position.y < curlowY)
                {
                    CurPts = pts;
                    var position = CurPts.transform.position;
                    curlowX = position.x;
                    curlowY = position.y;
                    Debug.Log(CurPts.name);
                }
            }
        }
    }
}
