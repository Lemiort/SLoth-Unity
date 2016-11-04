﻿using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

[Serializable]
public class JSonWrapper : MonoBehaviour {

    public GameObject cubePrefab;

    CubePrototype cube;
    UdpClient client;

    // Use this for initialization
    void Start () {

        cube = Instantiate(cubePrefab).GetComponent<CubePrototype>();
        if(cube != null)
        {
            cube.transform.position = new Vector3(0, 0, 0);
            Debug.Log("Found cube successful");
        }

        try
        {
            // IPAddress ipAddr = IPAddress.Parse("172.18.4.29");
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint serverEP = new IPEndPoint(ipAddr, 9876);
            client = new UdpClient();

            // Отправка простого сообщения
            string jsonString = JsonUtility.ToJson(cube);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            client.Send(jsonBytes, jsonBytes.Length, serverEP);

            IPEndPoint RemoteIPEndPoint = null;
            byte[] bytes = client.Receive(ref RemoteIPEndPoint);

            string results = Encoding.UTF8.GetString(bytes);

            Debug.Log(results);

            // Закрываем соединение
            //client.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
	
	// Update is called once per frame
	void Update () {
        cube.transform.position = new Vector3(cube.transform.position.x, 2.0f + 2.0f * Mathf.Sin(Time.realtimeSinceStartup), cube.transform.position.z);
        try
        {
            //IPAddress ipAddr = IPAddress.Parse("192.168.137.134");
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint serverEP = new IPEndPoint(ipAddr, 9876);

            // Отправка простого сообщения
            string jsonString = JsonUtility.ToJson(cube);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            client.Send(jsonBytes, jsonBytes.Length, serverEP);

            Debug.Log(jsonString);

            IPEndPoint RemoteIPEndPoint = null;
            byte[] bytes = client.Receive(ref RemoteIPEndPoint);

            string results = Encoding.UTF8.GetString(bytes);



            //JsonUtility.FromJsonOverwrite(results, cube);
            cube.Load(results);
            Debug.Log(results);
           // cube = JsonUtility.FromJson<CubePrototype>(results);
            // Debug.Log(Encoding.UTF8.GetBytes(JsonUtility.ToJson(cube2)));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}
