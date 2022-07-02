using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultTouchHandler : MonoBehaviour
{
    private List<TouchDrop>[] touchSlots = new List<TouchDrop>[33]; // C,A1-8,B1-8,D1-8,E1-8

    // Start is called before the first frame update
    void Start()
    {
        for(var i = 0; i < 33; i++)
        {
            touchSlots[i] = new List<TouchDrop>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int getAreaIndex(char area, int pos)
    {
        if (area == 'C')
        {
            return 0;
        }
        switch(area)
        {
            case 'A':
                return 0 + pos;
            case 'B':
                return 8 + pos;
            case 'D':
                return 16 + pos;
            case 'E':
                return 24 + pos;
        }
        return 0;
    }

    public void clearSlots ()
    {
        for (var i = 0; i < 33; i++)
        {
            touchSlots[i].Clear();
        }
    }

    public void registerTouch(TouchDrop obj)
    {
        var areaIndex = getAreaIndex(obj.areaPosition, obj.startPosition);
        obj.setLayer(touchSlots[areaIndex].Count);
        touchSlots[areaIndex].Add(obj);
    }

    public void cancelTouch(TouchDrop obj)
    {
        var areaIndex = getAreaIndex(obj.areaPosition, obj.startPosition);
        touchSlots[areaIndex].RemoveAt(0);

        foreach (TouchDrop each in touchSlots[areaIndex])
        {
            each.layerDown();
        }
    }
}
