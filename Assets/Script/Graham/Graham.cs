using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graham : MonoBehaviour
{

    public GameObject conteneur;
    public GameObject baryfab;
    public LineRenderer lineRenderer;

    
    [Header("Running Data")]
    public GameObject baricentre;
    public List<GameObject> points;
    public List<GameObject> pointsDeLaGéométrie;
    
    [Header ("Debug Data")]
    public List<float> debugAngles;
    public List<float> debugDistances;
    

    // Start is called before the first frame update
    void Start()
    {
        //GetPoint();
        //GetBaricentre();
        //Scan();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            lineRenderer.positionCount = 0;
            ProcessGraham();
        }
    }

    void ProcessGraham()
    {
        points = GetComponent<DrawLine>().points;
        GetBaricentre();
        Scan();
    }
    
    
    public void GetPoint()
    {
        for (int i = 0; i < conteneur.transform.childCount; i++)
        {
            points.Add(conteneur.transform.GetChild(i).gameObject);
        }
    }
    
    public void GetBaricentre()
    {
        if (baricentre != null)
        {
            Destroy(baricentre);
            baricentre = null;
        }
        
        Vector3 bposition = new Vector3(0,0,0);
        
        foreach (var point in points)
        {
            bposition += point.transform.position;
        }

        bposition /= points.Count;

        baricentre = Instantiate(baryfab, bposition, Quaternion.identity);

    }

    public void Scan()
    {
        
        OrderFromBaricentre();
        
        pointsDeLaGéométrie = RetirerLesConcaves();
        
        if (points.Count > 0)
        {
            lineRenderer.positionCount = pointsDeLaGéométrie.Count + 1;
            for (int i = 0; i < pointsDeLaGéométrie.Count; i++)
                lineRenderer.SetPosition(i, pointsDeLaGéométrie[i].transform.position);
            lineRenderer.SetPosition(pointsDeLaGéométrie.Count, pointsDeLaGéométrie[0].transform.position);
        }
        
    }

    public void OrderFromBaricentre()
    {
        List<float> angles = new List<float>();
        List<float> distances = new List<float>();

        foreach (var point   in points)
        {
            angles.Add(GetAngleFromBaricentre(point));
            distances.Add(GetDistanceFromBaricentre(point));
        }
        
        int x, j;
        float actuala;
        float actuald;
        GameObject actualg;
 
        for (x = 1; x < angles.Count; x++) {
            actuala = angles[x];
            actuald = distances[x];
            actualg = points[x];
            for (j = x; j > 0 && (angles[j - 1] > actuala || (angles[j - 1] == actuala && distances[j-1]> actuald)); j--) {
                points[j] = points[j - 1];
                angles[j] = angles[j - 1];
                distances[j] = distances[j - 1];
            }
            points[j] = actualg;
            angles[j] = actuala;
            distances[j] = actuald;
        }

        debugAngles = angles;
        debugDistances = distances;

    }

    public float GetAngleFromBaricentre(GameObject obj)
    {
        Vector3 vec = obj.transform.position - baricentre.transform.position;
        float a = Mathf.Atan2(vec.x, vec.y);
        if (a <= 0)
        {
            a = 2 * Mathf.PI + a;
        }
        return a;
        
    }

    public float GetAngleRad(Vector3 pivot, Vector3 previous, Vector3 next)
    {
        return Vector2.SignedAngle(next - pivot, previous - pivot);
    }

    public float GetDistanceFromBaricentre(GameObject obj)
    {
        Vector3 vec = baricentre.transform.position - obj.transform.position;
        float d = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y);
        return d;
    }

    public bool isConvexe(GameObject pivot, GameObject previous, GameObject next)
    {
        float rad = GetAngleRad(pivot.transform.position, previous.transform.position,
            next.transform.position);

        return (rad > 180 || rad < 0);
    }

    public List<GameObject> RetirerLesConcaves ()
    {
        LinkedList<GameObject> lc = new LinkedList<GameObject>(points);
        LinkedListNode<GameObject> pInit = lc.First;
        LinkedListNode<GameObject> pivot = pInit;
        
        bool avance = true;
        
        do
        {
            
            if (isConvexe(pivot.Value,GetPrevious(pivot).Value,GetNext(pivot).Value))
            {
                pivot = GetNext(pivot);
                avance = true;
            }
            else
            {
                pInit = GetPrevious(pivot);
                lc.Remove(pivot);
                pivot = pInit;
                avance = false;
            }
        } while ((pivot.Value != pInit.Value || avance == false));

        points = new List<GameObject>(lc);

        return points;
    }
    
    private static LinkedListNode<T> GetNext<T>(LinkedListNode<T> current)
    {
        return current.Next ?? current.List.First;
    }

    private static LinkedListNode<T> GetPrevious<T>(LinkedListNode<T> current)
    {
        return current.Previous ?? current.List.Last;
    }

}
