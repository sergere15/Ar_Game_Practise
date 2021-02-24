using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataForGame : MonoBehaviour
{
    public int id;
    public int amount;
    public string data;
    public bool isCustomer;
    public string loginUser;
    public string loginStore;
    public string passwordUser;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
