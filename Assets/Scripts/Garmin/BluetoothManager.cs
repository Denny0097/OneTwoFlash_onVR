using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BluetoothManager : MonoBehaviour
{
    private AndroidJavaClass unityClass;
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject _pluginInstance;

    public LabGarminData labGarminData = new LabGarminData();
    public bool connectionStatus = false;

    public static BluetoothManager instance;

    public void IntializePlugin(string pluginName)
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        _pluginInstance = new AndroidJavaObject(pluginName);
        if (_pluginInstance == null)
            Debug.Log("Plugin Error");
        _pluginInstance.CallStatic("receiveUnityActivity", unityActivity);
        _pluginInstance.Call("Initialize");
    }

    /*public List<string> GetDevices()
    {
        // Get all bluetooth paired devices.
        string list = _pluginInstance.Call<string>("bluetoothPairedDevices");
        
        return 
    }*/

    public void ConnectToDevice_Send(string message)
    {
        if (_pluginInstance != null)
        {
            // Set Andriod device name.
            string deviceName = "Galaxy S23 Ultra";

            if (message.Equals("connect"))
            {
                bool _receiveConnection = false;
                while (!_receiveConnection)
                {
                    _receiveConnection = ConnectToDevice_Receive(deviceName);
                }
                connectionStatus = true;
            }
            else if (message.Equals("stop"))
            {
                CancelInvoke("ReceiveData");
            }
            else if (message.Equals("disconnect"))
            {
                DisconnectFromDevice_Receive();
            }

            var _sendConnection = _pluginInstance.Call<bool>("connectToDevice_Send", deviceName);
            if (_sendConnection)
            {
                Debug.Log("Connect to send device.");
                bool _messageSend = false;
                while (!_messageSend)
                {
                    _messageSend = _pluginInstance.Call<bool>("sendBluetoothMessage", message);
                }
                Debug.Log("Message send.");
                if (CheckStartString(message))
                {
                    /*                          *
                     * Set data receiving rate. *
                     *                          */
                    InvokeRepeating("ReceiveData", 0, 1f);
                }
            }
            else
            {
                Debug.Log("Send connection failed.");
            }
        }
    }

    public bool ConnectToDevice_Receive(string deviceName)
    {
        if (_pluginInstance != null)
        {
            // Establish connection to get data from Android.
            var _receiveConnection = _pluginInstance.Call<bool>("connectToDevice_Receive", deviceName);
            if (_receiveConnection)
            {
                Debug.Log("Start receive connection.");
                return true;
            }
            else
            {
                Debug.Log("Receive conntection failed.");
                return false;
            }
        }
        Debug.Log("Plug-in instance is null.");
        return false;
    }

    public void DisconnectFromDevice_Receive()
    {
        var _disconnection = _pluginInstance.Call<bool>("disconnectFromDevice_Receive");
        if (_disconnection)
        {
            Debug.Log("Disconnect from receive device.");
        }
        else
        {
            Debug.Log("Disconnect failed");
        }
    }

    void Start()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        connectionStatus = false;
        IntializePlugin("com.garmin.bluetoothconnection.BluetoothManager");
    }

    public void ReceiveData()
    {
        StartCoroutine(CallReceiveData());
    }

    public IEnumerator CallReceiveData()
    {
        yield return null;
        var dataList = _pluginInstance.Call<string>("receiveMessage");
        //if (!dataList.Equals("error"))
        //    HandleReceivedData(dataList);
    }

    public void HandleReceivedData(string dataList)
    {
        //Debug.Log(dataList);
        List<LabGarminData> _dataList = DeserializeLabGarminDataList(dataList);
    }

    List<LabGarminData> DeserializeLabGarminDataList(string jsonString)
    {
        List<LabGarminData> dataList = new List<LabGarminData>();
        jsonString = jsonString.Replace("}{", "},{");
        string[] jsonStrings = jsonString.Split(new[] { "},{" }, System.StringSplitOptions.None);

        foreach (var jsonStr in jsonStrings)
        {
            string json = jsonStr;
            if (!jsonStr.StartsWith("{"))
            {
                json = "{" + json;
            }
            if (!jsonStr.EndsWith("}"))
            {
                json = json + "}";
            }
            LabGarminData data = JsonUtility.FromJson<LabGarminData>(json);
            labGarminData = data;

            Debug.Log($"HeartRate: {data.heartRate}, " +
                $"HeartVariability: {data.heartRateVariability}, " +
                $"StressLevel: {data.stressLevel}, " +
                $"SPO2: {data.SPO2}, " +
                $"Respiration: {data.respiration}, tag: {data.tag}, time: {data.time}");

            /*Debug.Log($"HeartRate: {data.heartRate}, RestingHeartRate: {data.restingHeartRate} " +
                $"HeartVariability: {data.heartRateVariability}, Accelerometer: {data.accelerometer_x}, " +
                $"{data.accelerometer_y}, {data.accelerometer_z}, Steps: {data.steps} " +
                $"Calories: {data.calories_Total}, {data.calories_Active}, Floors: {data.floors_Climb}, " +
                $"{data.floors_Descend}, IntensityMinutes: {data.intensityMinutes_Moderate}, " +
                $"{data.intensityMinutes_Vigorous}, StressLevel: {data.stressLevel}, " +
                $"SPO2: {data.SPO2}, BodyBattery: {data.bodyBattery}, " +
                $"Respiration: {data.respiration}, tag: {data.tag}, time: {data.time}");*/
            dataList.Add(data);
        }

        return dataList;
    }

    bool CheckStartString(string str)
    {
        if (!string.IsNullOrEmpty(str) && str.Length >= 5)
        {
            return str.Substring(0, 5).Equals("start");
        }
        return false;
    }
}
