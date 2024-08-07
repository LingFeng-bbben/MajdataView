#nullable enable
using System;
using UnityEngine;

namespace Assets.Scripts.Types
{
    public struct InputEventArgs
    {
        public SensorType Type {  get; set; }
        public SensorStatus OldStatus { get; set; }
        public SensorStatus Status { get; set; }
        public bool IsButton { get; set; }
        public bool IsClick => OldStatus == SensorStatus.Off && Status == SensorStatus.On;

    }
    public class Button
    {
        public KeyCode BindingKey { get; set; }
        public SensorType Type { get; set; }
        public bool IsJudging { get; set; }
        public SensorStatus Status { get; set; }
        public event EventHandler<InputEventArgs>? OnStatusChanged;
        public Button(KeyCode bindingKey,SensorType type)
        {
            BindingKey = bindingKey;
            Type = type;
            IsJudging = false;
            Status = SensorStatus.Off;
            OnStatusChanged = null;
        }
        public void PushEvent(InputEventArgs args)
        {
            if (OnStatusChanged is not null)
                OnStatusChanged(this,args);
        }
    }
}
