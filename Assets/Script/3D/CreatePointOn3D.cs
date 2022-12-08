using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatePointOn3D : MonoBehaviour
{
    
    [SerializeField]  private int nb_Point;
     public GameObject point;
     private float distance = 7;
     [SerializeField] private Vector3 Pos;
     [SerializeField] private Camera camera;
    public List<GameObject> points = new List<GameObject>();
    public ConvexHull3D convexhull3DScript;

    [SerializeField]  float speed = 20;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(points!= null) ResetPoint();
            GeneratePoint();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveCameraY(speed*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveCameraY(-speed*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            MoveCameraZ(speed*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveCameraZ(-speed*Time.deltaTime);
        }
    }

    void GeneratePoint()
    {
        Random.InitState((int)DateTime.Now.TimeOfDay.TotalMilliseconds);

        for (int i = 0; i < nb_Point; i++)
        {
            Pos.x = Random.Range(-distance,distance);
            Pos.y = Random.Range(-distance,distance);
            Pos.z = Random.Range(-distance,distance) ;

            GameObject newPoint = Instantiate(point, Pos, Quaternion.identity);
            newPoint.name = "Point_" + i;
            points.Add(newPoint);
        }
        
        points.Sort(SortList);
        convexhull3DScript.listePoints = points;
    }

    void ResetPoint()
    {
        foreach (var point in points)
        {
            Destroy(point);
        }
        points.Clear();
        convexhull3DScript.listePoints.Clear();
    }

    void MoveCameraY(float speed)
    {
        camera.gameObject.transform.RotateAround(new Vector3(0,0,0),Vector3.up,speed );
        camera.gameObject.transform.LookAt(new Vector3(0,0,0));
    }
    void MoveCameraZ(float speed)
    {
        camera.gameObject.transform.RotateAround(new Vector3(0,0,0),Vector3.forward,speed );
        camera.gameObject.transform.LookAt(new Vector3(0,0,0));
    }

    
        public int SortList(GameObject a, GameObject b)
        {
            if (a.transform.position.x == b.transform.position.x)
            {
                if (a.transform.position.y < b.transform.position.y)
                {
                    if (a.transform.position.z < b.transform.position.z)
                    {
                        return -1;
                    }
                    else if (a.transform.position.z > b.transform.position.z)
                    {
                        return 1;
                    }
                }
                else if (a.transform.position.y > b.transform.position.y)
                {
                    if (a.transform.position.z < b.transform.position.z)
                    {
                        return -1;
                    }
                    else if (a.transform.position.z > b.transform.position.z)
                    {
                        return 1;
                    }
                }
            }
            else if (a.transform.position.x < b.transform.position.x)
            {
                return -1;
            }
            else if (a.transform.position.x > b.transform.position.x)
            {
                return 1;
            }

            return 0;
        }
    
}
