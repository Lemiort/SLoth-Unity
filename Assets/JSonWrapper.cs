﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public abstract class Transaction
{
    static int globalCounter = 0;
}


[Serializable]
public class JSonWrapper : MonoBehaviour {
    public GameObject jsonContainerPrefab;
    public GameObject cubePrefab;

    static string servIp = "127.0.0.1";
    //static string servIp = "192.168.137.156";

    public CubePrototype cube;
    public CubePrototype tempCube;
    private UdpClient client;
    public JSonContainer jsonContainer;
    private int prevTransactionCount;
    private int currentTransactionCount;
    private Number number;
    private Dictionary<int, CubePrototype> cubesDictionary;
    private int ownCubeId;

    [Serializable]
    public class Number
    {
        public int number;
        public void Load(string savedData)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(savedData, this);
            }
            catch (Exception ex)
            {
                Debug.Log("Parsing error:" + ex.ToString());
            }
        }
    }

    [SerializeField]
    public class FakeCube
    {
        [SerializeField]
        public Transformation transformation;
        public int instanceID;

        public void Load(string savedData)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(savedData, this);
            }
            catch (Exception ex)
            {
                Debug.Log("Parsing error:" + ex.ToString());
            }
        }
    }

    FakeCube fakeCube;

    // Use this for initialization
    void Start () {
        number = new Number();
     
        cube = Instantiate(cubePrefab).GetComponent<CubePrototype>();
        cubesDictionary = new Dictionary<int, CubePrototype>();
        cubesDictionary.Add(cube.GetInstanceID(), cube);
        cube.instanceID = cube.GetInstanceID();
        ownCubeId = cube.GetInstanceID();

        //tempCube = cube;
        //tempCube = Instantiate(cubePrefab).GetComponent<CubePrototype>();
        //tempCube.transform.position = new Vector3(2, 2);
        fakeCube = new FakeCube();
        if (cube != null)
        {
            cube.transform.position = new Vector3(0, 0, 0);
            Debug.Log("Found cube successful");
        }

        jsonContainer = Instantiate(jsonContainerPrefab).GetComponent<JSonContainer>();
       /* jsonContainer.objectType = "setPosition";
        jsonContainer.objects.Add(JsonUtility.ToJson(cube));
        string str = JsonUtility.ToJson(jsonContainer).ToString();*/

        try
        {
            client = new UdpClient();

        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
	
	// Update is called once per frame
	void Update () {
        UpdateCurrentNum();
        UpdateTransactions();
        cube = cubesDictionary[ownCubeId];
        Vector3 newPos = cube.transform.position;
        if (Input.GetKey(KeyCode.W))
            newPos.z += 0.3f;
        if (Input.GetKey(KeyCode.S))
            newPos.z -= 0.3f;
        if (Input.GetKey(KeyCode.A))
            newPos.x -= 0.3f;
        if (Input.GetKey(KeyCode.D))
            newPos.x += 0.3f;
        //cube.transform.position = newPos;
        fakeCube.transformation.position = newPos;
        fakeCube.instanceID = cube.instanceID;
        //cube.transform.position = new Vector3(cube.transform.position.x, 2.0f + 2.0f * Mathf.Sin(Time.realtimeSinceStartup), cube.transform.position.z);
        try
        {
            //IPAddress ipAddr = IPAddress.Parse("192.168.137.134");
            IPAddress ipAddr = IPAddress.Parse(servIp);
            IPEndPoint serverEP = new IPEndPoint(ipAddr, 9876);

            //send cube
            jsonContainer.Clear();
            jsonContainer.objectType = "setPosition";
            jsonContainer.objects.Add(JsonUtility.ToJson(fakeCube));
            string jsonString = JsonUtility.ToJson(jsonContainer);
           

            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            client.Send(jsonBytes, jsonBytes.Length, serverEP);

            //Debug.Log(jsonString);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private void UpdateCurrentNum()
    {
        prevTransactionCount = currentTransactionCount;
        try
        {
            //IPAddress ipAddr = IPAddress.Parse("192.168.137.134");
            IPAddress ipAddr = IPAddress.Parse(servIp);
            IPEndPoint serverEP = new IPEndPoint(ipAddr, 9876);

            //Get current num
            jsonContainer.FormGetCurrentNumContainer();
            string jsonString = JsonUtility.ToJson(jsonContainer);


            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            client.Send(jsonBytes, jsonBytes.Length, serverEP);

            //Debug.Log(jsonString);

            IPEndPoint RemoteIPEndPoint = null;
            byte[] bytes = client.Receive(ref RemoteIPEndPoint);

            string results = Encoding.UTF8.GetString(bytes);

            //получение текущего номера
            jsonContainer.Load(results);
            if (jsonContainer.objectType == "currentNum")
            {
                number.Load(jsonContainer.objects[0]);
                currentTransactionCount = number.number;
            }

            //Debug.Log(results);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private void UpdateTransactions()
    {
        try
        {
            //IPAddress ipAddr = IPAddress.Parse("192.168.137.134");
            IPAddress ipAddr = IPAddress.Parse(servIp);
            IPEndPoint serverEP = new IPEndPoint(ipAddr, 9876);

            for (int i = prevTransactionCount; i < currentTransactionCount; i++)
            {
                //Get i transaction
                jsonContainer.FormGetTransaction(i);
                string jsonString = JsonUtility.ToJson(jsonContainer);


                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                client.Send(jsonBytes, jsonBytes.Length, serverEP);

                //Debug.Log(jsonString);

                IPEndPoint RemoteIPEndPoint = null;
                byte[] bytes = client.Receive(ref RemoteIPEndPoint);

                string results = Encoding.UTF8.GetString(bytes);

                //исполнение траназкций
                jsonContainer.Load(results);
                if (jsonContainer.objectType == "setPosition")
                {
                    //tempCube.Load(jsonContainer.objects[0]);
                    fakeCube.Load(jsonContainer.objects[0]);

                    if (cubesDictionary.ContainsKey(fakeCube.instanceID))
                    {
                        cubesDictionary[fakeCube.instanceID].Load(jsonContainer.objects[0]);
                    }
                    else
                    {
                        //CubePrototype temp = Instantiate(tempCube);
                        CubePrototype temp = Instantiate(cubePrefab).GetComponent<CubePrototype>();
                        temp.instanceID = fakeCube.instanceID;
                        cubesDictionary.Add(fakeCube.instanceID, temp);
                    }
                    //tempCube.transform.position = new Vector3(2, 2, 0);
                }

                //Debug.Log(results);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Parsing error??"+ex.ToString());
        }
    }

}
