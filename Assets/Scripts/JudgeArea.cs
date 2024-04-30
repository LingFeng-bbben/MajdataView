using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Sensor;

public class Area
{
    public bool On = false;
    public bool Off = false;
    public SensorType Type;
    public bool IsLast = false;
    public bool IsFinished
    {
        get
        {
            if (IsLast)
                return On;
            else
                return On && Off;
        }
    }
    public void Judge(SensorStatus status)
    {
        if (status == SensorStatus.Off)
        {
            if (On)
                Off = true;
        }
        else
            On = true;
    }
}
public class JudgeArea
{
    public bool On 
    {
        get
        {
            return areas.Any(a => a.On);
        }
    }
    public bool CanSkip = true;
    public bool IsFinished 
    {
        get
        {
            if (areas.Count == 0)
                return false;
            else
                return areas.Any(x => x.IsFinished);
        }
    }
    public int SlideIndex;
    List<Area> areas = new();

    public void Register(Action<Sensor, SensorStatus, SensorStatus> d)
    {
        foreach(var a in areas)
            GameObject.Find("Sensors")
                      .GetComponent<SensorManager>()
                      .GetSensor(a.Type).onSensorStatusChanged += d;
    }
    public void UnRegister(Action<Sensor, SensorStatus, SensorStatus> d)
    {
        foreach (var a in areas)
            GameObject.Find("Sensors")
                      .GetComponent<SensorManager>()
                      .GetSensor(a.Type).onSensorStatusChanged -= d;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Area[] GetAreas() => areas.ToArray();
    public SensorType[] GetSensorTypes() => areas.Select(x => x.Type).ToArray();
    public JudgeArea(Dictionary<SensorType,bool> types, int slideIndex)
    {
        SlideIndex = slideIndex;
        foreach (var item in types)
        {
            var type = item.Key;
            if (areas.Any(x => x.Type == type))
                continue;

            areas.Add(new Area()
            {
                Type = type,
                IsLast = item.Value
            });
        }
        SlideIndex = slideIndex;
    }
    public void Judge(SensorType type ,SensorStatus status)
    {
        var areaList = areas.Where(x => x.Type == type);

        if (areaList.Count() == 0)
            return;

        var area = areaList.First();
        area.Judge(status);
    }
    public void AddArea(SensorType type, bool isLast = false)
    {
        if (areas.Any(x => x.Type == type))
            return;
        areas.Add(new Area()
        {
            Type = type,
            IsLast = isLast
        });
    }
}
