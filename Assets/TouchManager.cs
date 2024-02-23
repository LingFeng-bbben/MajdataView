using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    private SerialPort ser;

    public bool[] sensorStates = new bool[35];
    public GameObject[] touchDisplays;
    //public Text debugtext;
    public bool display_Debug = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        //debugtext = GameObject.Find("SerialDebug").GetComponent<Text>();
        //debugtext.text = "OpenSerial";
        sensorStates = new bool[35];
        ser = new SerialPort("COM3",9600);
        ser.Open();
        ser.Write("{STAT}");
        Thread recvThread = new Thread(new ThreadStart(recvLoop));
        recvThread.Start();
    }

    void recvLoop()
    {
        while (true)
        {
            if(ser.IsOpen)
            {
                int count = ser.BytesToRead;
                var buf = new byte[count];
                ser.Read(buf, 0, count);
                if (buf.Length > 0)
                {
                    
                    if (buf[0] == '(')
                    {
                        int k = 0;
                        for (int i = 1; i < 8; i++)
                        {
                            print(buf[i].ToString("X2"));
                            for (int j = 0; j < 5; j++)
                            {
                                sensorStates[k] = (buf[i] & (0x01 << j)) > 0;
                                k++;
                            }
                        }
                    }
                    
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        ser.Close();
    }
    // Update is called once per frame
    void Update()
    {
        if (display_Debug)
        {
            //debugtext.text = "";
            for (int i = 0; i < 34; i++)
            {
                //debugtext.text += sensorStates[i] ? 1 : 0;
                touchDisplays[i].SetActive(sensorStates[i]);
            }
        }
    }
}
