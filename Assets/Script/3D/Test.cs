using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<GameObject> go = new List<GameObject>();
    public GameObject pointTest;
    public GameObject lnrdr;
    public GameObject meshObj;
    public Vector2 AB;
    public Vector2 AC;
    public float productABC;
    
    public Vector2 PA;
    public Vector2 PB;
    public Vector2 PC;
    
    public float productPAC;
    public float productPBC;
    public float productPAB;

    public float productALL;
    void Start()
    {
        //GameObject newlndr = Instantiate(lnrdr, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        //EdgesNTris.drawEdge(go[0], go[1], newlndr);
        GameObject Meshobj = Instantiate(meshObj, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
        //EdgesNTris.drawTri(go, Meshobj);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InsideTriangle();
        }
    }

    void InsideTriangle()
    {
        AB = new Vector2((go[1].transform.position.x - go[0].transform.position.x), (go[1].transform.position.y - go[0].transform.position.y));
        AC = new Vector2((go[2].transform.position.x - go[0].transform.position.x), (go[2].transform.position.y - go[0].transform.position.y));
        productABC = ((AB.x * AC.y) - (AC.x * AB.y))/2;
        
        PA = new Vector2((go[0].transform.position.x - pointTest.transform.position.x), (go[0].transform.position.y - pointTest.transform.position.y));
        PB = new Vector2((go[1].transform.position.x - pointTest.transform.position.x), (go[1].transform.position.y - pointTest.transform.position.y));
        PC = new Vector2((go[2].transform.position.x - pointTest.transform.position.x), (go[2].transform.position.y - pointTest.transform.position.y));
        
        productPAB = ((PA.x * PB.y) - (PB.x * PA.y))/2;
        productPAC = ((PA.x * PC.y) - (PC.x * PA.y))/2;
        productPBC = ((PB.x * PC.y) - (PC.x * PB.y))/2;

        productALL = productPAB + productPAC + productPBC;
    }
}
