using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CreatePointOn3D : MonoBehaviour
{
    
    [SerializeField] private int nb_Point;
    [SerializeField] public GameObject point;
    [SerializeField] public float maxX;
    [SerializeField] public float maxY;
    [SerializeField] public float maxZ;
    [SerializeField] private Vector3 Pos;
    [SerializeField] private Vector3 worldPos;
    [SerializeField] private Camera camera;
    public List<GameObject> points = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("test");
            
            for (int i = 0; i < nb_Point; i++)
            {
                Pos.x = Random.Range(-maxX,maxX);
                Pos.y = Random.Range(-maxY,maxY);
                Pos.z = Random.Range(0,maxZ) + (camera.nearClipPlane + 15);
                
                worldPos = camera.ScreenToWorldPoint(Pos);
                
                GameObject newPoint = Instantiate(point, Pos, Quaternion.identity);
                newPoint.name = "Point_" + i;
                points.Add(newPoint);
            }
        }
        //Debug.Log(Input.mousePosition);
    }
}
