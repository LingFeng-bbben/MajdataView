
namespace Assets.Scripts.IO
{
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
        C,
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
    public interface IIOComponent
    {

    }
    public struct InputEventArgs
    {
        public SensorType Type {  get; set; }
        public SensorStatus OldStatus { get; set; }
        public SensorStatus Status { get; set; }
        public bool IsButton { get; set; }
        public bool IsClick => OldStatus == SensorStatus.Off && Status == SensorStatus.On;

    }
}
