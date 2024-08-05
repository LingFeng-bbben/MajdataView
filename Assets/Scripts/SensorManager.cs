using Assets.Scripts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    //public event Action<Sensor,SensorStatus,SensorStatus> onSensorStatusChanged;
    List<Sensor> sensors = new();
    //Dictionary<SensorType, List<Guid>> sensorTask = new();

    // Start is called before the first frame update
    void Start()
    {
        //sensorTask.Clear();
        var count = transform.childCount;
        for (int i = 0; i < count; i++)
            sensors.Add(transform.GetChild(i).GetComponent<Sensor>());
        //foreach (var t in sensors.Select(x => x.Type))
        //    sensorTask.Add(t, new List<Guid>());
    }
    private void FixedUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetSensorOn(SensorType t,Guid id)
    {
        GetSensor(t).SetOn(id);
    }
    public void SetSensorOff(SensorType t, Guid id)
    {
        GetSensor(t).SetOff(id);
    }
    public SensorStatus GetSensorStatus(SensorType t) => sensors[(int)t].Status;
    public Sensor[] GetSensors(SensorGroup g) => sensors.Where(x => x.Group == g).ToArray();
    public Sensor GetSensor(SensorType t) => sensors[(int)t];
}
