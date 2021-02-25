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

    [System.Serializable]
    public class Seller
    {
        public string login;
        public string description;
    }

    [System.Serializable]
    public class Item
    {
        public long id;
        public string name;
        public string description;
        public string seller;
        public int cost;
    }

    [System.Serializable]
    public class Order
    {
        public long id;
        public string item;
        public string seller;
        public string addres;
    }
}
