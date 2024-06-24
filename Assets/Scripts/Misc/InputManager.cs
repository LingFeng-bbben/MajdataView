using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Sensor;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public bool AutoPlay = false;
    public event Action<SensorType, SensorStatus, SensorStatus> OnSensorStatusChange;//oStatus nStatus

    Guid guid = Guid.NewGuid();
    public Dictionary<int,List<Sensor>> triggerSensors = new();
    List<GameObject> sensors = new();
    SensorManager sManager;

    Dictionary<KeyCode, SensorType> keyMap = new();
    // Start is called before the first frame update
    void Start()
    {
        var sManagerObj = GameObject.Find("Sensors");
        var count = sManagerObj.transform.childCount;
        for (int i = 0; i < count; i++)
            sensors.Add(sManagerObj.transform.GetChild(i).gameObject);
        sManager = sManagerObj.GetComponent<SensorManager>();
        keyMap.Add(KeyCode.LeftArrow, SensorType.A1);
        keyMap.Add(KeyCode.RightArrow, SensorType.A1);
    }

    // Update is called once per frame
    void Update()
    {
        var count = Input.touchCount;
        CheckKey(new KeyCode[] {KeyCode.LeftArrow , KeyCode.RightArrow});

        if (Input.GetKeyDown(KeyCode.Home))
            AutoPlay = !AutoPlay;
        if (Input.GetMouseButton(0))
            PositionHandle(-1, Input.mousePosition);
        else
            Untrigger(-1);
        if (count > 0)
        {
            foreach(var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        PositionHandle(touch.fingerId, touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        Untrigger(touch.fingerId);
                        break;
                }
            }
        }

    }
    void Untrigger(int id)
    {
        if (triggerSensors.Count() == 0 || !triggerSensors.ContainsKey(id))
            return;
        foreach (var s in triggerSensors[id])
            sManager.SetSensorOff(s.Type, guid);
        triggerSensors[id].Clear();
    }
    void PositionHandle(int id, Vector3 pos)
    {
        Vector3 sPosition = pos;
        sPosition.z = mainCamera.nearClipPlane;
        Vector3 wPosition = mainCamera.ScreenToWorldPoint(sPosition);
        wPosition.z = 0;
        if (!triggerSensors.ContainsKey(id))
            triggerSensors.Add(id, new());
        Running(id,wPosition);
    }
    void CheckKey(KeyCode[] keys)
    {
        var dict = keys.ToDictionary(x => x,x => Input.GetKeyDown(x));
        foreach(var key in dict.Keys)
        {
            if (keyMap.ContainsKey(key) && OnSensorStatusChange != null && dict[key])
            {
                OnSensorStatusChange(keyMap[key], SensorStatus.Off, SensorStatus.On);
                sManager.GetSensor(keyMap[key]).IsJudging = false;
            }
        }
    }
    void Running(int id,Vector3 pos)
    {
        var starRadius = 0.763736616f;
        var starPos = pos;
        var oldList = new List<Sensor>(triggerSensors[id]);
        triggerSensors[id].Clear();
        foreach (var s in sensors.Select(x => x.GetComponent<RectTransform>()))
        {
            var sensor = s.GetComponent<Sensor>();

            var rCenter = s.position;
            var rWidth = s.rect.width * s.lossyScale.x;
            var rHeight = s.rect.height * s.lossyScale.y;

            var radius = Math.Max(rWidth, rHeight) / 2;

            if ((starPos - rCenter).sqrMagnitude <= (radius * radius + starRadius * starRadius))
                triggerSensors[id].Add(sensor);
        }
        var untriggerSensors = oldList.Where(x => !triggerSensors[id].Contains(x));

        foreach (var s in untriggerSensors)
            sManager.SetSensorOff(s.Type, guid);
        foreach (var s in triggerSensors[id])
            sManager.SetSensorOn(s.Type, guid);
    }
}
