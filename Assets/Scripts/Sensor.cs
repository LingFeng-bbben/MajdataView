using Assets.Scripts.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool IsJudging { get; set; } = false;
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
            else if (i <= 16)
                return SensorGroup.C;
            else if (i <= 24)
                return SensorGroup.D;
            else
                return SensorGroup.E;
        }
    }

    public event EventHandler<InputEventArgs> OnStatusChanged;//oStatus nStatus

    List<Guid> tasks = new();
    public void SetOn(Guid id)
    {
        if (tasks.Contains(id))
            return;
        var oStatus = Status;
        var nStatus = SensorStatus.On;

        Status = nStatus;
        
        if(!tasks.Contains(id))
            tasks.Add(id);
        if (oStatus != nStatus)
        {
            if (OnStatusChanged != null)
            {
                OnStatusChanged(this,new InputEventArgs()
                {
                    IsButton = false,
                    Type = Type,
                    OldStatus = oStatus,
                    Status = nStatus
                });
                IsJudging = false;
            }
            print($"Sensor:{Type} On");
        }
    }
    public void SetOff(Guid id) 
    {
        if (!tasks.Contains(id))
            return;
        var nStatus = SensorStatus.Off;

        tasks.Remove(id);
        if(tasks.Count == 0)
        {
            var oStatus = Status;
            if (OnStatusChanged != null)
            {
                OnStatusChanged(this,new InputEventArgs()
                {
                    IsButton = false,
                    Type = Type,
                    OldStatus = oStatus,
                    Status = nStatus
                });
            }
            Status = nStatus;
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
