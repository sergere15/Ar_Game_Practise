using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTarget : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject capsule;
    void Start()
    {
        var o = Instantiate(capsule, new Vector3(0, 0, 0), Quaternion.identity);
        o.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
