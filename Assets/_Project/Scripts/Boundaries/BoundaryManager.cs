using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    public Boundary[] Boundaries { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Boundaries = GetComponentsInChildren<Boundary>();
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
