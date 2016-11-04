using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CubePrototype : MonoBehaviour {
    public Vector3 position;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        position = transform.position;
	}

    public void Load(string savedData)
    {
        try
        {
            JsonUtility.FromJsonOverwrite(savedData, this);
            transform.position = position;
        }
        catch (Exception ex)
        {
            Debug.Log("Parsing error:"+ex.ToString());
        }
    }
}
