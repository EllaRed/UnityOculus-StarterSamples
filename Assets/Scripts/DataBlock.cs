using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class DataBlock : MonoBehaviour 
{
    public enum DataBlockType
    {
        Text,
        Image,
        Video
    }

    public List<Attribute> Attributes = new List<Attribute>(); 

    public class Attribute
    {
        public string Name;
        public string Value;
    }
    void Start() {
        //gameObject.tag = "DataBlock";
    }
}