using Assets.Scripts.IO;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public List<GameObject> notes = new();
    public Dictionary<GameObject, int> noteOrder = new();
    public Dictionary<int, int> noteCount = new();

    public Dictionary<GameObject, int> touchOrder = new();
    public Dictionary<SensorType, int> touchCount = new();
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
    }
    public void Refresh()
    {
        var count = transform.childCount;
        ResetCounter();
        for (int i = 0; i < count; i++)
        {
            var child = transform.GetChild(i);
            var tap = child.GetComponent<TapDrop>();
            var hold = child.GetComponent<HoldDrop>();
            var star = child.GetComponent<StarDrop>();
            var touch = child.GetComponent<TouchDrop>();
            var touchHold = child.GetComponent<TouchHoldDrop>();

            if (tap != null)
                noteOrder.Add(tap.gameObject, noteCount[tap.startPosition]++);
            else if (hold != null)
                noteOrder.Add(hold.gameObject, noteCount[hold.startPosition]++);
            else if (star != null && !star.isNoHead)
                noteOrder.Add(star.gameObject, noteCount[star.startPosition]++);
            else if (touch != null)
                touchOrder.Add(touch.gameObject, touchCount[touch.GetSensor()]++);
            else if(touchHold != null)
                touchOrder.Add(touchHold.gameObject, touchCount[SensorType.C]++);

            notes.Add(child.gameObject);
        }
        ResetCounter();
    }
    void ResetCounter()
    {
        noteCount.Clear();
        touchCount.Clear();
        for (int i = 1; i < 9; i++)
            noteCount.Add(i, 0);
        var sensorParent = GameObject.Find("Sensors");
        var count = sensorParent.transform.childCount;
        for (int i = 0; i < count; i++)
            touchCount.Add(sensorParent.transform
                                       .GetChild(i)
                                       .GetComponent<Sensor>().Type, 0);
    }
    public int GetOrder(GameObject obj) => noteOrder[obj];
    public bool CanJudge(GameObject obj, int pos)
    {
        if (!noteOrder.ContainsKey(obj))
            return false;
        var index = noteOrder[obj];
        var nowIndex = noteCount[pos];

        return index <= nowIndex;
    }

    public bool CanJudge(GameObject obj,SensorType t)
    {
        if (!touchOrder.ContainsKey(obj))
            return false;
        var index = touchOrder[obj];
        var nowIndex = touchCount[t];

        return index <= nowIndex;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
