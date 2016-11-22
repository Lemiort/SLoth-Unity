using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class JSonContainer : MonoBehaviour {
    public string objectType;

    public List<string> objects = new List<string>();

    public void AddObject(string jsonString)
    {
        objects.Add(jsonString.Substring(1, jsonString.Length - 2));
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Load(string json)
    {
        objects.Clear();
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
