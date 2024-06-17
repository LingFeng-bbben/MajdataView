using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sensor;
using UnityEngine;

namespace Assets.Scripts
{
    public class TouchBase : NoteDrop
    {
        public char areaPosition;
        public bool isFirework;

        public GameObject tapEffect;
        public GameObject judgeEffect;


        protected Sprite[] judgeText;

        protected Quaternion GetRoation()
        {
            if (sensor.Type == SensorType.C)
                return Quaternion.Euler(Vector3.zero);
            var d = Vector3.zero - transform.position;
            var deg = 180 + Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg;

            return Quaternion.Euler(new Vector3(0, 0, -deg));
        }
        public SensorType GetSensor()
        {
            switch (areaPosition)
            {
                case 'A':
                    return (SensorType)(startPosition - 1);
                case 'B':
                    return (SensorType)(startPosition + 7);
                case 'C':
                    return SensorType.C;
                case 'D':
                    return (SensorType)(startPosition + 16);
                case 'E':
                    return (SensorType)(startPosition + 24);
                default:
                    return SensorType.A1;
            }
        }
    }
}
