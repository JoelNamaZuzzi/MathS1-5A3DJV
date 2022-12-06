using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CreatePointOn3D : MonoBehaviour
{
    
    [SerializeField]  private int nb_Point;
     public GameObject point;
     private float distance = 7;
     [SerializeField] private Vector3 Pos;
     [SerializeField] private Camera camera;
    public List<GameObject> points = new List<GameObject>();

    private float speed = 10;



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
            MoveCameraY(speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveCameraY(-speed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            MoveCameraZ(speed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveCameraZ(-speed);
        }
    }

    void GeneratePoint()
    {
       
            
        for (int i = 0; i < nb_Point; i++)
        {
            Pos.x = Random.Range(-distance,distance);
            Pos.y = Random.Range(-distance,distance);
            Pos.z = Random.Range(-distance,distance) ;

            GameObject newPoint = Instantiate(point, Pos, Quaternion.identity);
            newPoint.name = "Point_" + i;
            points.Add(newPoint);
        }
    }

    void ResetPoint()
    {
        foreach (var point in points)
        {
            Destroy(point);
        }
        points.Clear();
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
}
