using Assets.Scripts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public bool AutoPlay = false;
    public Dictionary<int,List<Sensor>> triggerSensors = new();

    //public event EventHandler<InputEventArgs> OnSensorStatusChanged;
    public event EventHandler<InputEventArgs> OnButtonStatusChanged;

    Guid guid = Guid.NewGuid();
    
    List<GameObject> sensors = new();
    SensorManager sManager;
    Dictionary<KeyCode, SensorType> keyMap = new()
    {
        { KeyCode.W,SensorType.A1 },
        { KeyCode.E,SensorType.A2 },
        { KeyCode.D,SensorType.A3 },
        { KeyCode.C,SensorType.A4 },
        { KeyCode.X,SensorType.A5 },
        { KeyCode.Z,SensorType.A6 },
        { KeyCode.A,SensorType.A7 },
        { KeyCode.Q,SensorType.A8 },
    };
    Dictionary<KeyCode, (SensorStatus,bool)> keyStatus = new()
    {
        { KeyCode.W,(SensorStatus.Off,false) },
        { KeyCode.E,(SensorStatus.Off,false) },
        { KeyCode.D,(SensorStatus.Off,false) },
        { KeyCode.C,(SensorStatus.Off,false) },
        { KeyCode.X,(SensorStatus.Off,false) },
        { KeyCode.Z,(SensorStatus.Off,false) },
        { KeyCode.A,(SensorStatus.Off,false) },
        { KeyCode.Q,(SensorStatus.Off,false) },
    };
    // Start is called before the first frame update
    void Start()
    {
        var sManagerObj = GameObject.Find("Sensors");
        var count = sManagerObj.transform.childCount;
        for (int i = 0; i < count; i++)
            sensors.Add(sManagerObj.transform.GetChild(i).gameObject);
        sManager = sManagerObj.GetComponent<SensorManager>();

    }

    // Update is called once per frame
    void Update()
    {
        var count = Input.touchCount;
        CheckButton();

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
    public bool CheckSensorStatus(SensorType target,SensorStatus targetStatus)
    {
        var sensor = sensors[(int)target].GetComponent<Sensor>();
        return sensor.Status == targetStatus;
    }
    public bool CheckButtonStatus(SensorType target, SensorStatus targetStatus)
    {
        if (target > SensorType.A8)
            throw new InvalidOperationException("Button index cannot greater than A8");
        var _ = keyMap.ToDictionary(x => x.Value,y => y.Key);
        var (buttonStatus, _) = keyStatus[_[target]];
        return buttonStatus == targetStatus;
    }
    public void SetBusying(SensorType target,bool isJudging)
    {
        if (target > SensorType.A8)
            throw new InvalidOperationException("Button index cannot greater than A8");
        var _ = keyMap.ToDictionary(x => x.Value, y => y.Key);
        var (x, y) = keyStatus[_[target]];
        keyStatus[_[target]] = (x, isJudging);
    }
    public bool IsBusying(SensorType target)
    {
        if (target > SensorType.A8)
            throw new InvalidOperationException("Button index cannot greater than A8");
        var _ = keyMap.ToDictionary(x => x.Value, y => y.Key);
        var (x, y) = keyStatus[_[target]];
        return y;
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
    void CheckButton()
    {
        var keys = keyMap.ToDictionary(x => x.Key, y => Input.GetKey(y.Key) ? SensorStatus.On: SensorStatus.Off);
        foreach(var keyPair in keys)
        {
            var (oldStatus, busying) = keyStatus[keyPair.Key];
            if(oldStatus != keyPair.Value)
            {
                print($"Key \"{keyPair.Key}\": {keyPair.Value}");
                if(OnButtonStatusChanged is not null)
                {
                    OnButtonStatusChanged(this, new InputEventArgs()
                    {
                        Type = keyMap[keyPair.Key],
                        OldStatus = oldStatus,
                        Status = keyPair.Value,
                        IsButton = true
                    });
                }
                keyStatus[keyPair.Key] = (keyPair.Value, false);
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
