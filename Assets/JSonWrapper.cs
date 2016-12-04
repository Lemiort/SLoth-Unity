using UnityEngine;
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

    // Use this for initialization
    void Start () {
        number = new Number();
     
        cube = Instantiate(cubePrefab).GetComponent<CubePrototype>();
        cubesDictionary = new Dictionary<int, CubePrototype>();
        cubesDictionary.Add(cube.GetInstanceID(), cube);
        cube.instanceID = cube.GetInstanceID();
        ownCubeId = cube.GetInstanceID();

        //tempCube = cube;
        tempCube = Instantiate(cubePrefab).GetComponent<CubePrototype>();
        tempCube.transform.position = new Vector3(2, 2);
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
           //  IPAddress ipAddr = IPAddress.Parse(servIp);
           //// IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
           // IPEndPoint serverEP = new IPEndPoint(ipAddr, 9876);
            client = new UdpClient();

            //// Отправка простого сообщения
            ////string jsonString = JsonUtility.ToJson(cube);
            //string jsonString = JsonUtility.ToJson(jsonContainer);

            //byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            //client.Send(jsonBytes, jsonBytes.Length, serverEP);
            //Debug.Log("Sent"+ jsonString);

            //IPEndPoint RemoteIPEndPoint = null;
            //byte[] bytes = client.Receive(ref RemoteIPEndPoint);

            //string results = Encoding.UTF8.GetString(bytes);

            //Debug.Log(results);

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
        UpdateCurrentNum();
        UpdateTransactions();
        cube = cubesDictionary[ownCubeId];
        cube.transform.position = new Vector3(cube.transform.position.x, 2.0f + 2.0f * Mathf.Sin(Time.realtimeSinceStartup), cube.transform.position.z);
        try
        {
            //IPAddress ipAddr = IPAddress.Parse("192.168.137.134");
            IPAddress ipAddr = IPAddress.Parse(servIp);
            IPEndPoint serverEP = new IPEndPoint(ipAddr, 9876);

            //send cube
            jsonContainer.Clear();
            jsonContainer.objectType = "setPosition";
            jsonContainer.objects.Add(JsonUtility.ToJson(cube));
            string jsonString = JsonUtility.ToJson(jsonContainer);
           

            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            client.Send(jsonBytes, jsonBytes.Length, serverEP);

            Debug.Log(jsonString);
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

            Debug.Log(jsonString);

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

            Debug.Log(results);
            // cube = JsonUtility.FromJson<CubePrototype>(results);
            // Debug.Log(Encoding.UTF8.GetBytes(JsonUtility.ToJson(cube2)));
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

                Debug.Log(jsonString);

                IPEndPoint RemoteIPEndPoint = null;
                byte[] bytes = client.Receive(ref RemoteIPEndPoint);

                string results = Encoding.UTF8.GetString(bytes);

                //исполнение траназкций
                jsonContainer.Load(results);
                if (jsonContainer.objectType == "setPosition")
                {
                    tempCube.Load(jsonContainer.objects[0]);

                    if(cubesDictionary.ContainsKey(tempCube.instanceID))
                    {
                        cubesDictionary[tempCube.instanceID].Load(jsonContainer.objects[0]);
                    }
                    else
                    {
                        CubePrototype temp = Instantiate(tempCube);
                        temp.instanceID = tempCube.instanceID;
                        //CubePrototype temp = Instantiate(cubePrefab).GetComponent<CubePrototype>();
                        //temp.Load(jsonContainer.objects[0]);
                        cubesDictionary.Add(tempCube.instanceID, temp);
                    }
                    tempCube.transform.position = new Vector3(2, 2, 0);
                }

                Debug.Log(results);
                // cube = JsonUtility.FromJson<CubePrototype>(results);
                // Debug.Log(Encoding.UTF8.GetBytes(JsonUtility.ToJson(cube2)));
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Parsing error??"+ex.ToString());
        }
    }

}
