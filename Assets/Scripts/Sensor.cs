using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public event Action<Sensor, SensorStatus, SensorStatus> onSensorStatusChanged;
    public enum SensorStatus
    {
        On,
        Off
    }
    public enum SensorType
    {
        A1, 
        A2,
        A3,
        A4,
        A5,
        A6,
        A7,
        A8,
        B1,
        B2,
        B3,
        B4,
        B5,
        B6,
        B7,
        B8,
        C1,
        C2,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        E1,
        E2,
        E3,
        E4,
        E5,
        E6,
        E7,
        E8
    }
    public enum SensorGroup
    {
        A,
        B,
        C,
        D,
        E
    }
    public SensorStatus Status = SensorStatus.Off;
    public SensorType Type;
    public SensorGroup Group 
    { 
        get
        {
            var i = (int)Type;
            if (i <= 7)
                return SensorGroup.A;
            else if (i <= 15)
                return SensorGroup.B;
            else if (i <= 17)
                return SensorGroup.C;
            else if (i <= 25)
                return SensorGroup.D;
            else
                return SensorGroup.E;
        }
    }

    List<Guid> tasks = new();
    public void SetOn(Guid id)
    {
        var oStatus = Status;
        var nStatus = SensorStatus.On;

        Status = nStatus;
        if (onSensorStatusChanged is not null)
            onSensorStatusChanged(this, oStatus, nStatus);
        if(!tasks.Contains(id))
            tasks.Add(id);
        if (oStatus != nStatus)
            print($"Sensor:{Type} On");
    }
    public void SetOff(Guid id) 
    {
        var oStatus = Status;
        var nStatus = SensorStatus.Off;

        tasks.Remove(id);
        if(tasks.Count == 0)
        {
            Status = nStatus;
            if (onSensorStatusChanged is not null)
                onSensorStatusChanged(this, oStatus, nStatus);
            print($"Sensor:{Type} Off");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
