using UnityEngine;
using Assets.Scripts.Notes;
using Assets.Scripts.IO;

namespace Assets.Scripts
{
    public class TouchBase : NoteDrop
    {
        public char areaPosition;
        public bool isFirework;

        public GameObject tapEffect;
        public GameObject judgeEffect;


        protected Sprite[] judgeText;
        public TouchGroup GroupInfo;

        protected Quaternion GetRoation()
        {
            if (sensor.Type == SensorType.C)
                return Quaternion.Euler(Vector3.zero);
            var d = Vector3.zero - transform.position;
            var deg = 180 + Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg;

            return Quaternion.Euler(new Vector3(0, 0, -deg));
        }
        public SensorType GetSensor() => GetSensor(areaPosition, startPosition);
        public static SensorType GetSensor(char areaPos, int startPos)
        {
            switch (areaPos)
            {
                case 'A':
                    return (SensorType)(startPos - 1);
                case 'B':
                    return (SensorType)(startPos + 7);
                case 'C':
                    return SensorType.C;
                case 'D':
                    return (SensorType)(startPos + 16);
                case 'E':
                    return (SensorType)(startPos + 24);
                default:
                    return SensorType.A1;
            }
        }
    }
}
