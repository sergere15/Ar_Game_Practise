using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataForGame : MonoBehaviour
{
    public int id;
    public string data;


    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
