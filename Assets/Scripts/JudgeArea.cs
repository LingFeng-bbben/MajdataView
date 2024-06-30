using Assets.Scripts.IO;
using System.Collections.Generic;
using System.Linq;

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
    public void Reset()
    {
        On = false;
        Off = false;
    }
}
public class JudgeAreaGroup
{
    List<JudgeArea> areas = new();
    public bool IsFinished
    {
        get => areas.All(x => x.IsFinished);
    }
    public bool On
    {
        get
        {
            return areas.All(a => a.On);
        }
    }
    public int SlideIndex { get; set; }

    public JudgeAreaGroup(List<JudgeArea> areas, int slideIndex)
    {
        this.areas = areas;
        SlideIndex = slideIndex;
        foreach (JudgeArea area in areas)
            area.Reset();
    }
    public void Judge(SensorType type, SensorStatus status)
    {
        foreach (var area in areas)
            area.Judge(type, status);
    }
    public SensorType[] GetSensorTypes() => areas.SelectMany(x => x.GetSensorTypes()).ToArray();
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
    public int SlideIndex { get; set; }
    List<Area> areas = new();
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
    public void SetIsLast()
    {
        areas.ForEach(x => x.IsLast = true);
    }
    public void SetNonLast()
    {
        areas.ForEach(x => x.IsLast = false);
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
    public void Reset()
    {
        foreach(var area in areas)
            area.Reset();
    }
}
