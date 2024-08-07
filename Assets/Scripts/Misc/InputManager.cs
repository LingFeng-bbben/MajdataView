using Assets.Scripts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#nullable enable
public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public static bool AutoPlay { get; set; } = true;
    public Dictionary<int,List<Sensor>> triggerSensors = new();

    //public event EventHandler<InputEventArgs> OnSensorStatusChanged;
    public event EventHandler<InputEventArgs>? OnButtonStatusChanged;

    Guid guid = Guid.NewGuid();
    
    List<GameObject> sensorObjs = new();
    List<Sensor> sensors = new();
    List<Button> buttons = new();
    SensorManager sManager;

    // Start is called before the first frame update
    void Start()
    {
        var sManagerObj = GameObject.Find("Sensors");
        var count = sManagerObj.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var obj = sManagerObj.transform.GetChild(i).gameObject;
            sensorObjs.Add(obj);
            sensors.Add(obj.GetComponent<Sensor>());
        }
        sManager = sManagerObj.GetComponent<SensorManager>();
        buttons = new(new Button[] 
        {
            new Button(KeyCode.W,SensorType.A1),
            new Button(KeyCode.E,SensorType.A2),
            new Button(KeyCode.D,SensorType.A3),
            new Button(KeyCode.C,SensorType.A4),
            new Button(KeyCode.X,SensorType.A5),
            new Button(KeyCode.Z,SensorType.A6),
            new Button(KeyCode.A,SensorType.A7),
            new Button(KeyCode.Q,SensorType.A8),
        });
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
    public void BindSensor(EventHandler<InputEventArgs> checker, SensorType sType)
    {
        var sensor = sensors.Find(x => x.Type == sType);
        if (sensor == null)
            throw new Exception($"{sType} Sensor not found.");
        sensor.OnStatusChanged += checker;
    }
    public void UnbindSensor(EventHandler<InputEventArgs> checker, SensorType sType)
    {
        var sensor = sensors.Find(x => x.Type == sType);
        if (sensor == null)
            throw new Exception($"{sType} Sensor not found.");
        sensor.OnStatusChanged -= checker;
    }
    public void BindArea(EventHandler<InputEventArgs> checker,SensorType sType)
    {
        var sensor = sensors.Find(x => x.Type == sType);
        var button = buttons.Find(x => x.Type == sType);
        if (sensor == null || button is null)
            throw new Exception($"{sType} Sensor or Button not found.");

        sensor.OnStatusChanged += checker;
        button.OnStatusChanged += checker;
    }
    public void UnbindArea(EventHandler<InputEventArgs> checker, SensorType sType)
    {
        var sensor = sensors.Find(x => x.Type == sType);
        var button = buttons.Find(x => x.Type == sType);
        if (sensor == null || button is null)
            throw new Exception($"{sType} Sensor or Button not found.");

        sensor.OnStatusChanged -= checker;
        button.OnStatusChanged -= checker;
    }
    public bool CheckAreaStatus(SensorType sType,SensorStatus targetStatus)
    {
        var sensor = sensors.Find(x => x.Type == sType);
        var button = buttons.Find(x => x.Type == sType);

        if (sensor == null || button is null)
            throw new Exception($"{sType} Sensor or Button not found.");

        return sensor.Status == targetStatus || button.Status == targetStatus; 
    }
    public bool CheckSensorStatus(SensorType target,SensorStatus targetStatus)
    {
        if (target > SensorType.A8)
            throw new InvalidOperationException("Button index cannot greater than A8");

        var sensor = sensorObjs[(int)target].GetComponent<Sensor>();
        return sensor.Status == targetStatus;
    }
    public bool CheckButtonStatus(SensorType target, SensorStatus targetStatus)
    {
        if (target > SensorType.A8)
            throw new InvalidOperationException("Button index cannot greater than A8");
        var button = buttons.Find(x => x.Type == target);

        return button.Status == targetStatus;
    }
    public void ClickSensor(SensorType target)
    {
        var sensor = GetSensor(target);
        if (sensor is null)
            throw new Exception($"{target} Sensor not found.");
        sensor.Click();
    }

    public void SetBusy(InputEventArgs args)
    {
        var type = args.Type;
        if(args.IsButton)
        {
            var button = GetButton(type);
            if(button is null)
                throw new Exception($"{type} Button not found.");

            button.IsJudging = true;
        }
        else
        {
            var sensor = GetSensor(type);
            if (sensor is null)
                throw new Exception($"{type} Sensor not found.");

            sensor.IsJudging = true;
        }
    }
    public void SetIdle(InputEventArgs args)
    {
        var type = args.Type;
        if (args.IsButton)
        {
            var button = GetButton(type);
            if (button is null)
                throw new Exception($"{type} Button not found.");

            button.IsJudging = false;
        }
        else
        {
            var sensor = GetSensor(type);
            if (sensor is null)
                throw new Exception($"{type} Sensor not found.");

            sensor.IsJudging = false;
        }
    }
    public bool IsIdle(InputEventArgs args)
    {
        bool isIdle = false;
        var type = args.Type;
        if (args.IsButton)
        {
            var button = GetButton(type);
            if (button is null)
                throw new Exception($"{type} Button not found.");

            isIdle = !button.IsJudging;
        }
        else
        {
            var sensor = GetSensor(type);
            if (sensor is null)
                throw new Exception($"{type} Sensor not found.");

            isIdle = !sensor.IsJudging;
        }
        return isIdle;
    }

    public Button? GetButton(SensorType type) => buttons.Find(x => x.Type == type);
    public Sensor? GetSensor(SensorType type) => sensors.Find(x => x.Type == type);

    public void SetBusying(SensorType target,bool isJudging)
    {
        if (target > SensorType.A8)
            throw new InvalidOperationException("Button index cannot greater than A8");
        var button = buttons.Find(x => x.Type == target);
        button.IsJudging = isJudging;
    }
    public bool IsBusying(SensorType target)
    {
        if (target > SensorType.A8)
            throw new InvalidOperationException("Button index cannot greater than A8");
        var button = buttons.Find(x => x.Type == target);
        return button.IsJudging;
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
        foreach(var button in buttons)
        {
            var nStatus = Input.GetKey(button.BindingKey) ? SensorStatus.On : SensorStatus.Off;
            var oStatus = button.Status;
            if(oStatus != nStatus)
            {
                print($"Key \"{button.BindingKey}\": {nStatus}");
                button.PushEvent(new InputEventArgs()
                {
                    Type = button.Type,
                    OldStatus = oStatus,
                    Status = nStatus,
                    IsButton = true
                });
                button.Status = nStatus;
                button.IsJudging = false;
            }
        }
    }
    void Running(int id,Vector3 pos)
    {
        var starRadius = 0.763736616f;
        var starPos = pos;
        var oldList = new List<Sensor>(triggerSensors[id]);
        triggerSensors[id].Clear();
        foreach (var s in sensorObjs.Select(x => x.GetComponent<RectTransform>()))
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
