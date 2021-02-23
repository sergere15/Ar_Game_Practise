using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONTemplate
{
    [System.Serializable]
    public class Response
    {
        public int value;
        public int code;
        public string description;
    }
}
