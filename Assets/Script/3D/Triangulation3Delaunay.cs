using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulation3Delaunay : MonoBehaviour
{
    public ConvexHull3D convexhull3DScript;
    public ConvexHull hull;
    public ConvexHull hullDelauney = new ConvexHull();
    [SerializeField] private LineRenderer lr;
    [SerializeField] private GameObject line;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            hull = convexhull3DScript.convexHull;
            Triangulation3DelaunayFlipping();
        }
    }

    public void Triangulation3DelaunayFlipping()
    {
        
        // int isLooping = 0;
        //
        // int flippingEdges = 0;
        //
        // while (true)
        // {
        //     isLooping += 1;
        //
        //     if (isLooping > 10000)
        //     {
        //         Debug.Log("BOUCLE INFINIE ");
        //         break;
        //     }
        //     bool isFlip = false;
        //
        //     for (int i = 0; i < hull.listEdges.Count; i++)
        //     {
        //         Edges e = hull.listEdges[i];
        //         
        //     }
        // }

        DrawTriangulation3Delauney(hull);
    }

    public void DrawTriangulation3Delauney(ConvexHull h)
    {
        foreach (GameObject foundObj in GameObject.FindGameObjectsWithTag("Face"))
        {
            Destroy(foundObj);
        }
        for (int f = 0; f < h.listFace.Count; f++)
        {
            GameObject Meshobj = Instantiate(convexhull3DScript.meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Material mat = convexhull3DScript.mats[Random.Range(0,convexhull3DScript.mats.Count)];
            EdgesNTris.drawTri(h.listFace[f], Meshobj,mat);
        }
    }
}
