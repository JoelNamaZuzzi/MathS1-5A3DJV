using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public TScript ts;
    public TriangulationDelauney td;
    [SerializeField] private GameObject point;
    [SerializeField] private List<GameObject> points = new List<GameObject>();
    [SerializeField] private List<GameObject> pointsTriangulationIncre = new List<GameObject>();

    [SerializeField] private Camera camera;
    [SerializeField] private LineRenderer lr;
    
    [SerializeField] private Vector3 mousePos;
    [SerializeField] private Vector3 worldPos;
    public List<TScript.Triangle> triangles = new List<TScript.Triangle>();
    private int counter = 1;
    
    public bool isIncre = false;
    
    // Start is called before the first frame update
    void Start()
    {
        ts = GetComponent<TScript>();
        td = GetComponent<TriangulationDelauney>();
        camera = Camera.main;
    }

    //Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            mousePos.z = camera.nearClipPlane + 30;

            worldPos = camera.ScreenToWorldPoint(mousePos);

            //mousePos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.nearClipPlane));

            GameObject newPoint = Instantiate(point, worldPos, Quaternion.identity);
            newPoint.name = "Point_" + counter; 
            //newPoint.AddComponent<LineRenderer>();
            points.Add(newPoint);
            pointsTriangulationIncre.Add(newPoint);
            
            ts.Sommets.Add(newPoint);
            
            counter++;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DrawTriangleLinesTriangulationIncre();
            isIncre = true;
        }
        
        if (Input.GetKeyDown(KeyCode.D) && isIncre == true)
        {
            DrawTriangleLinesTriangulationDelauney();
        }
        if (Input.GetKeyDown(KeyCode.D) && isIncre == false)
        {
            Debug.LogErrorFormat("Faire la triangulation dabord");
        }
    }

    public void DrawTriangleLinesTriangulationDelauney()
    {
        Debug.Log("delau");
        List<TScript.Triangle> trianglesDelauney = new List<TScript.Triangle>();
        trianglesDelauney = TriangulationDelauney.TriangulationFlippingEdges(triangles);
        DrawLineTriangle(trianglesDelauney);
        
      /*  for (int i = 0; i < trianglesDelauney.Count - 1; i++)
        {
            Debug.LogErrorFormat(trianglesDelauney[i].v1.position+" "+trianglesDelauney[i].v2.position+" "+trianglesDelauney[i].v3.position+"\n");
        }*/
    }

    public void DrawTriangleLinesTriangulationIncre()
    {
        pointsTriangulationIncre.Sort(SortList);
        triangles = TriangulationIncremental(pointsTriangulationIncre);

        // Debug.Log(triangles.Count);
        DrawLineTriangle(triangles);
     /*   for (int i = 0; i < triangles.Count - 1; i++)
        {
            Debug.LogWarningFormat(triangles[i].v1.position+" "+triangles[i].v2.position+" "+triangles[i].v3.position+"\n");
        }*/
    }

    public void DrawLineTriangle(List<TScript.Triangle> tri)
    {
        lr.positionCount = 0;
        for (int j = 0; j < tri.Count; j++)
        {
            //points[i].GetComponent<LineRenderer>().positionCount = 4;
            lr.positionCount += 2;
            lr.SetPosition(lr.positionCount -2, tri[j].v1.position);
          //  Debug.LogWarningFormat((lr.positionCount -2).ToString());
            lr.SetPosition(lr.positionCount -1, tri[j].v2.position);
          //  Debug.LogWarningFormat((lr.positionCount -1).ToString());
            lr.positionCount += 2;
            
            lr.SetPosition(lr.positionCount -2, tri[j].v2.position);
          //  Debug.LogWarningFormat((lr.positionCount -2).ToString());
            lr.SetPosition(lr.positionCount -1, tri[j].v3.position);
          //  Debug.LogWarningFormat((lr.positionCount -1).ToString());
            lr.positionCount += 2;
            
            lr.SetPosition(lr.positionCount -2, tri[j].v3.position);
          //  Debug.LogWarningFormat((lr.positionCount -2).ToString());
            lr.SetPosition(lr.positionCount -1, tri[j].v1.position);
          //  Debug.LogWarningFormat((lr.positionCount -1).ToString());
            //lr.positionCount += 2;
        }
    }

    public int SortList(GameObject a, GameObject b)
    {
        if (a.transform.position.x == b.transform.position.x)
        {
            if (a.transform.position.y < b.transform.position.y)
            {
                return -1;
            }
            else if (a.transform.position.y > b.transform.position.y)
            {
                return 1;
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

    public static List<TScript.Triangle> TriangulationIncremental(List<GameObject> _points)
    {
        List<TScript.Triangle> triangles = new List<TScript.Triangle>();

        TScript.Triangle newTriangle = new TScript.Triangle(_points[0].transform.position, _points[1].transform.position, _points[2].transform.position);
        
        triangles.Add(newTriangle);

        List<TScript.Edge> edges = new List<TScript.Edge>();
        
        edges.Add(new TScript.Edge(newTriangle.v1, newTriangle.v2));
        edges.Add(new TScript.Edge(newTriangle.v2, newTriangle.v3));
        edges.Add(new TScript.Edge(newTriangle.v3, newTriangle.v1));

        for (int i = 3; i < _points.Count; i++)
        {
            Vector3 currentPoint = _points[i].transform.position;

            List<TScript.Edge> newEdges = new List<TScript.Edge>();

            for (int j = 0; j < edges.Count; j++)
            {
                TScript.Edge currentEdge = edges[j];
                Vector3 midPoint = (currentEdge.v1.position + currentEdge.v2.position) / 2f;
                TScript.Edge edgeToMidPoint = new TScript.Edge(currentPoint, midPoint);

                bool SeeEdge = true;

                for (int k = 0; k < edges.Count; k++)
                {
                    if (k == j)
                    {
                        continue;
                    }
                 //   Debug.LogError(AreEdgesIntersecting(edgeToMidPoint, edges[k]));
                    if (AreEdgesIntersecting(edgeToMidPoint, edges[k]))
                    {
                        SeeEdge = false;
                        
                        break;
                    }
                }

                if (SeeEdge)
                {
                    TScript.Edge edgePoint1 = new TScript.Edge(currentEdge.v1, new TScript.Vertex(currentPoint));
                    TScript.Edge edgePoint2 = new TScript.Edge(currentEdge.v2, new TScript.Vertex(currentPoint));
                    
                    newEdges.Add(edgePoint1);
                    newEdges.Add(edgePoint2);

                    TScript.Triangle newtriangle = new TScript.Triangle(edgePoint1.v1, edgePoint1.v2, edgePoint2.v1);
                    
                    triangles.Add(newtriangle);
                }
            }

            for (int j = 0; j < newEdges.Count; j++)
            {
                edges.Add(newEdges[j]);
            }
        }

        return triangles;
    }
    
    private static bool AreEdgesIntersecting(TScript.Edge edge1, TScript.Edge edge2)
    {
        Vector2 l1_p1 = new Vector2(edge1.v1.position.x, edge1.v1.position.y);
        Vector2 l1_p2 = new Vector2(edge1.v2.position.x, edge1.v2.position.y);
        
        Vector2 l2_p1 = new Vector2(edge2.v1.position.x, edge2.v1.position.y);
        Vector2 l2_p2 = new Vector2(edge2.v2.position.x, edge2.v2.position.y);

        bool isIntersecting = AreLinesIntersecting(l1_p1, l1_p2, l2_p1, l2_p2, true);

        return isIntersecting;
    }
    
    public static bool AreLinesIntersecting(Vector2 l1_p1, Vector2 l1_p2, Vector2 l2_p1, Vector2 l2_p2, bool shouldIncludeEndPoints)
    {
        bool isIntersecting = false;

        float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

        //Make sure the denominator is > 0, if not the lines are parallel
        if (denominator != 0f)
        {
            float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
            float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

            //Are the line segments intersecting if the end points are the same
            if (shouldIncludeEndPoints)
            {
                //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                if (u_a >= 0f && u_a <= 1f && u_b >= 0f && u_b <= 1f)
                {
                    isIntersecting = true;
                }
            }
            else
            {
                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a > 0f && u_a < 1f && u_b > 0f && u_b < 1f)
                {
                    isIntersecting = true;
                }
            }
		
        }

        return isIntersecting;
    }
    
}
