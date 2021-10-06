using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YValue : MonoBehaviour
{
    public float yValue;
    public static YValue ins;

    void Awake()
    {
        ins = this;
    }

    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
