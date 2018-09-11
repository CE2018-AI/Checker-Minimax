using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    List<Vector2> possibilities = new List<Vector2>();
    Vector2 x;
    // Use this for initialization
    void Start () {
        possibilities.Add(x);
        possibilities.Add(x);
        x = Vector2.zero;
        Debug.Log(x.x+"******"+x.y);

    }
	
	// Update is called once per frame
	void Update () {
       
    }
}
