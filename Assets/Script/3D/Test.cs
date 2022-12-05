using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<GameObject> go = new List<GameObject>();
    public GameObject lnrdr;
    public GameObject meshObj;
    void Start()
    {
        //GameObject newlndr = Instantiate(lnrdr, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        //EdgesNTris.drawEdge(go[0], go[1], newlndr);
        GameObject Meshobj = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        EdgesNTris.drawTri(go, Meshobj);
    }
    
}
