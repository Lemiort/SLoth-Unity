using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Transformation
{
    public Vector3 position;
}

[Serializable]
public class CubePrototype : MonoBehaviour {

    [SerializeField]
    public Transformation transformation;
    public int instanceID;
    //public Vector3 position;

    // Use this for initialization
    void Start () {
        //instanceID = this.GetInstanceID();
        this.transformation.position = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //transformation.position = transform.position;
        transformation.position = transform.position;
    }

    public void Load(string savedData)
    {
        try
        {
            JsonUtility.FromJsonOverwrite(savedData, this);
            this.transform.position = transformation.position;
        }
        catch (Exception ex)
        {
            Debug.Log("Parsing error:"+ex.ToString());
        }
    }
}
