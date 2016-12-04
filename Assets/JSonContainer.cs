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

    public void Clear()
    {
        objects.Clear();
    }

    public void Load(string json)
    {
        objects.Clear();
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void FormGetCurrentNumContainer()
    {
        this.Clear();
        this.objectType = "getCurrentNum";
        //this.containerType = ContainerType.getCurrentNum;
        //this.objectTypeStr = this.containerType.name();
    }

    public void FormCurrentNumContainer(int num)
    {
        throw new NotImplementedException();
        //this.clear();
        //this.containerType = ContainerType.currentNum;
        //this.objectTypeStr = this.containerType.name();
        //this.jsonObject.put("number", num);
        //this.addObject(jsonObject, 0);
    }

    public void FormGetTransaction(int num)
    {
        this.Clear();
        this.objectType = "getTransaction";
        JSonWrapper.Number number = new JSonWrapper.Number();
        number.number = num;
        objects.Add(JsonUtility.ToJson(number));
        //this.containerType = ContainerType.getTransaction;
        //this.objectTypeStr = this.containerType.name();
        //this.jsonObject.put("number", num);
        //this.addObject(jsonObject, 0);
    }

    public void FormSetPositionContainer(CubePrototype cube)
    {
        throw new NotImplementedException();
        //this.clear();
        //this.containerType = ContainerType.setPosition;
        //this.objectTypeStr = this.containerType.name();
        //this.addObject(simpleCube.toJSONObject(), 0);
    }
}
