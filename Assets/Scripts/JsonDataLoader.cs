using Assets.Scripts.IO;
using Assets.Scripts.Notes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JsonDataLoader : MonoBehaviour
{
    public float noteSpeed = 7f;
    public float touchSpeed = 7.5f;
    public bool smoothSlideAnime = false;
    public Sprite starEach;
    public GameObject tapPrefab;
    public GameObject holdPrefab;
    public GameObject starPrefab;
    public GameObject touchHoldPrefab;
    public GameObject touchPrefab;
    public GameObject eachLine;
    public GameObject starLine;
    public GameObject notes;
    public GameObject star_slidePrefab;
    public GameObject[] slidePrefab;
    public Material breakMaterial;
    public RuntimeAnimatorController BreakShine;
    public RuntimeAnimatorController JudgeBreakShine;
    public RuntimeAnimatorController HoldShine;

    public Text diffText;
    public Text levelText;
    public Text titleText;
    public Text artistText;
    public Text designText;
    public RawImage cardImage;
    public Color[] diffColors = new Color[7];
    private CustomSkin customSkin;

    private ObjectCounter ObjectCounter;

    private int slideLayer = -1;
    private int noteSortOrder = 0;

    private static readonly Dictionary<SimaiNoteType, int> NOTE_LAYER_COUNT = new Dictionary<SimaiNoteType, int>()
    {
        {SimaiNoteType.Tap, 2 },
        {SimaiNoteType.Hold, 3 },
        {SimaiNoteType.Slide, 2 },
        {SimaiNoteType.Touch, 7 },
        {SimaiNoteType.TouchHold, 6 },
    };
    private static readonly Dictionary<string, int> SLIDE_PREFAB_MAP = new Dictionary<string, int>()
    {
        {"line3", 0 },
        {"line4", 1 },
        {"line5", 2 },
        {"line6", 3 },
        {"line7", 4 },
        {"circle1", 5 },
        {"circle2", 6 },
        {"circle3", 7 },
        {"circle4", 8 },
        {"circle5", 9 },
        {"circle6", 10 },
        {"circle7", 11 },
        {"circle8", 12 },
        {"v1", 41 },
        {"v2", 13 },
        {"v3", 14 },
        {"v4", 15 },
        {"v6", 16 },
        {"v7", 17 },
        {"v8", 18 },
        {"ppqq1", 19 },
        {"ppqq2", 20 },
        {"ppqq3", 21 },
        {"ppqq4", 22 },
        {"ppqq5", 23 },
        {"ppqq6", 24 },
        {"ppqq7", 25 },
        {"ppqq8", 26 },
        {"pq1", 27 },
        {"pq2", 28 },
        {"pq3", 29 },
        {"pq4", 30 },
        {"pq5", 31 },
        {"pq6", 32 },
        {"pq7", 33 },
        {"pq8", 34 },
        {"s", 35 },
        {"wifi", 36 },
        {"L2", 37 },
        {"L3", 38 },
        {"L4", 39 },
        {"L5", 40 },
    };

    static readonly Dictionary<SensorType, SensorType[]> TOUCH_GROUPS = new()
    {
        { SensorType.A1, new SensorType[]{ SensorType.D1, SensorType.D2, SensorType.E1, SensorType.E2 } },
        { SensorType.A2, new SensorType[]{ SensorType.D2, SensorType.D3, SensorType.E2, SensorType.E3 } },
        { SensorType.A3, new SensorType[]{ SensorType.D3, SensorType.D4, SensorType.E3, SensorType.E4 } },
        { SensorType.A4, new SensorType[]{ SensorType.D4, SensorType.D5, SensorType.E4, SensorType.E5 } },
        { SensorType.A5, new SensorType[]{ SensorType.D5, SensorType.D6, SensorType.E5, SensorType.E6 } },
        { SensorType.A6, new SensorType[]{ SensorType.D6, SensorType.D7, SensorType.E6, SensorType.E7 } },
        { SensorType.A7, new SensorType[]{ SensorType.D7, SensorType.D8, SensorType.E7, SensorType.E8 } },
        { SensorType.A8, new SensorType[]{ SensorType.D8, SensorType.D1, SensorType.E8, SensorType.E1 } },

        { SensorType.D1, new SensorType[]{ SensorType.A1, SensorType.A8, SensorType.E1 } },
        { SensorType.D2, new SensorType[]{ SensorType.A2, SensorType.A1, SensorType.E2 } },
        { SensorType.D3, new SensorType[]{ SensorType.A3, SensorType.A2, SensorType.E3 } },
        { SensorType.D4, new SensorType[]{ SensorType.A4, SensorType.A3, SensorType.E4 } },
        { SensorType.D5, new SensorType[]{ SensorType.A5, SensorType.A4, SensorType.E5 } },
        { SensorType.D6, new SensorType[]{ SensorType.A6, SensorType.A5, SensorType.E6 } },
        { SensorType.D7, new SensorType[]{ SensorType.A7, SensorType.A6, SensorType.E7 } },
        { SensorType.D8, new SensorType[]{ SensorType.A8, SensorType.A7, SensorType.E8 } },

        { SensorType.E1, new SensorType[]{ SensorType.D1, SensorType.A1, SensorType.A8, SensorType.B1, SensorType.B8 } },
        { SensorType.E2, new SensorType[]{ SensorType.D2, SensorType.A2, SensorType.A1, SensorType.B2, SensorType.B1 } },
        { SensorType.E3, new SensorType[]{ SensorType.D3, SensorType.A3, SensorType.A2, SensorType.B3, SensorType.B2 } },
        { SensorType.E4, new SensorType[]{ SensorType.D4, SensorType.A4, SensorType.A3, SensorType.B4, SensorType.B3 } },
        { SensorType.E5, new SensorType[]{ SensorType.D5, SensorType.A5, SensorType.A4, SensorType.B5, SensorType.B4 } },
        { SensorType.E6, new SensorType[]{ SensorType.D6, SensorType.A6, SensorType.A5, SensorType.B6, SensorType.B5 } },
        { SensorType.E7, new SensorType[]{ SensorType.D7, SensorType.A7, SensorType.A6, SensorType.B7, SensorType.B6 } },
        { SensorType.E8, new SensorType[]{ SensorType.D8, SensorType.A8, SensorType.A7, SensorType.B8, SensorType.B7 } },

        { SensorType.B1, new SensorType[]{ SensorType.E1, SensorType.E2, SensorType.B8, SensorType.B2, SensorType.A1, SensorType.C } },
        { SensorType.B2, new SensorType[]{ SensorType.E2, SensorType.E3, SensorType.B1, SensorType.B3, SensorType.A2, SensorType.C } },
        { SensorType.B3, new SensorType[]{ SensorType.E3, SensorType.E4, SensorType.B2, SensorType.B4, SensorType.A3, SensorType.C } },
        { SensorType.B4, new SensorType[]{ SensorType.E4, SensorType.E5, SensorType.B3, SensorType.B5, SensorType.A4, SensorType.C } },
        { SensorType.B5, new SensorType[]{ SensorType.E5, SensorType.E6, SensorType.B4, SensorType.B6, SensorType.A5, SensorType.C } },
        { SensorType.B6, new SensorType[]{ SensorType.E6, SensorType.E7, SensorType.B5, SensorType.B7, SensorType.A6, SensorType.C } },
        { SensorType.B7, new SensorType[]{ SensorType.E7, SensorType.E8, SensorType.B6, SensorType.B8, SensorType.A7, SensorType.C } },
        { SensorType.B8, new SensorType[]{ SensorType.E8, SensorType.E1, SensorType.B7, SensorType.B1, SensorType.A8, SensorType.C } },

        { SensorType.C, new SensorType[]{ SensorType.B1, SensorType.B2, SensorType.B3, SensorType.B4, SensorType.B5, SensorType.B6, SensorType.B7, SensorType.B8} },
    };

    static Dictionary<string, float> SLIDE_AREA_CONST = new()
    {
        { "line3", 0.1919f},
        { "line4", 0.1793f},
        { "line5", 0.1629f},
        { "line6", 0.1793f},
        { "line7", 0.1919f},
        { "circle1", 0.7892f},
        { "circle2", 0.2326f},
        { "circle3", 0.1550f},
        { "circle4", 0.1163f},
        { "circle5", 0.0930f},
        { "circle6", 0.0775f},
        { "circle7", 0.0664f},
        { "circle8", 0.0490f},
        { "v1", 0.1629f},
        { "v2", 0.1629f},
        { "v3", 0.1629f},
        { "v4", 0.1629f},
        { "v5", 0.1629f},
        { "v6", 0.1629f},
        { "v7", 0.1629f},
        { "v8", 0.1629f},
        { "ppqq1", 0.1014f},
        { "ppqq2", 0.1204f},
        { "ppqq3", 0.1434f},
        { "ppqq4", 0.0697f},
        { "ppqq5", 0.0867f},
        { "ppqq6", 0.1026f},
        { "ppqq7", 0.1266f},
        { "ppqq8", 0.1413f},
        { "pq1", 0.1021f},
        { "pq2", 0.1144f},
        { "pq3", 0.1247f},
        { "pq4", 0.1436f},
        { "pq5", 0.1627f},
        { "pq6", 0.0752f},
        { "pq7", 0.0984f},
        { "pq8", 0.1126f},
        { "s", 0.1054f},
        { "wifi", 0.1829f},
        { "L2", 0.0948f},
        { "L3", 0.0711f},
        { "L4", 0.0948f},
        { "L5", 0.1186f},
    };

    private static readonly Dictionary<string, List<int>> SLIDE_AREA_STEP_MAP = new Dictionary<string, List<int>>()
    {
        {"line3", new List<int>(){ 0, 2, 8, 13 } },
        {"line4", new List<int>(){ 0, 3, 8, 12, 18 } },
        {"line5", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"line6", new List<int>(){ 0, 3, 8, 12, 18 } },
        {"line7", new List<int>(){ 0, 2, 8, 13 } },
        {"circle1", new List<int>(){ 0, 3, 11, 19, 27, 35, 43, 50, 58, 63 } },
        {"circle2", new List<int>(){ 0, 3, 7 } },
        {"circle3", new List<int>(){ 0, 3, 11, 15 } },
        {"circle4", new List<int>(){ 0, 3, 11, 19, 23 } },
        {"circle5", new List<int>(){ 0, 3, 11, 19, 27, 31 } },
        {"circle6", new List<int>(){ 0, 3, 11, 19, 27, 35, 39 } },
        {"circle7", new List<int>(){ 0, 3, 11, 19, 27, 35, 43, 47 } },
        {"circle8", new List<int>(){ 0, 3, 11, 19, 27, 35, 43, 50, 55 } },
        {"v1", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v2", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v3", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v4", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v6", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v7", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v8", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"ppqq1", new List<int>(){ 0, 3, 7, 13, 17, 26, 32, 35 } },
        {"ppqq2", new List<int>(){ 0, 3, 7, 12, 16, 25, 28 } },
        {"ppqq3", new List<int>(){ 0, 3, 6, 12, 15, 22 } },
        {"ppqq4", new List<int>(){ 0, 3, 7, 12, 16, 25, 29, 35, 40, 44, 49 } },
        {"ppqq5", new List<int>(){ 0, 3, 7, 12, 16, 25, 29, 35, 40, 44, 49 } },
        {"ppqq6", new List<int>(){ 0, 3, 7, 12, 16, 25, 28, 34, 38, 41, 48 } },
        {"ppqq7", new List<int>(){ 0, 3, 7, 13, 17, 27, 31, 37, 41, 46 } },
        {"ppqq8", new List<int>(){ 0, 3, 7, 12, 16, 25, 29, 35, 41 } },
        {"pq1", new List<int>(){ 0, 3, 8, 11, 14, 17, 21, 24, 27, 33 } },
        {"pq2", new List<int>(){ 0, 3, 8, 11, 14, 18, 21, 24, 30 } },
        {"pq3", new List<int>(){ 0, 3, 9, 12, 16, 19, 23, 27 } },
        {"pq4", new List<int>(){ 0, 3, 9, 13, 16, 20, 24 } },
        {"pq5", new List<int>(){ 0, 3, 9, 13, 17, 21 } },
        {"pq6", new List<int>(){ 0, 3, 8, 11, 15, 18, 21, 25, 28, 31, 35, 38, 42 } },
        {"pq7", new List<int>(){ 0, 3, 8, 12, 15, 18, 22, 25, 28, 32, 35, 39 } },
        {"pq8", new List<int>(){ 0, 3, 8, 11, 14, 17, 21, 24, 27, 30, 36 } },
        {"s", new List<int>(){ 0, 3, 8, 11, 17, 21, 24, 30 } },
        {"wifi", new List<int>(){ 0, 1, 4, 6, 11 } },
        {"L2", new List<int>(){ 0, 2, 7, 15, 21, 26, 32 } },
        {"L3", new List<int>(){ 0, 2, 8, 17, 20, 26, 29, 34 } },
        {"L4", new List<int>(){ 0, 2, 8, 17, 22, 26, 32 } },
        {"L5", new List<int>(){ 0, 2, 8, 16, 22, 28 } },
    };
    private static readonly Dictionary<int, List<List<JudgeArea>>> WIFISLIDE_JUDGE_QUEUE = new Dictionary<int, List<List<JudgeArea>>>()
    {
        { 1,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, true },{SensorType.D6, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, true },{SensorType.B5, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, true },{SensorType.D5, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 2,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, true },{SensorType.D7, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, true },{SensorType.B6, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, true },{SensorType.D6, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 3,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, true },{SensorType.D8, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, true },{SensorType.B7, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, true },{SensorType.D7, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 4,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, true },{SensorType.D1, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, true },{SensorType.B8, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, true },{SensorType.D8, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 5,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, true },{SensorType.D2, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, true },{SensorType.B1, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, true },{SensorType.D1, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 6,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, true },{SensorType.D3, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, true },{SensorType.B2, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, true },{SensorType.D2, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 7,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, true },{SensorType.D4, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, true },{SensorType.B3, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, true },{SensorType.D3, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 8,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, true },{SensorType.D5, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, true },{SensorType.B4, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, true },{SensorType.D4, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        }
    };
    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 120;
        ObjectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void LoadJson(string json, float ignoreOffset)
    {
        var loadedData = JsonConvert.DeserializeObject<Majson>(json);

        diffText.text = loadedData.difficulty;
        levelText.text = loadedData.level;
        titleText.text = loadedData.title;
        artistText.text = loadedData.artist;
        designText.text = loadedData.designer;
        cardImage.color = diffColors[loadedData.diffNum];

        CountNoteSum(loadedData);

        var lastNoteTime = loadedData.timingList.Last().time;

        foreach (var timing in loadedData.timingList)
        {
            try
            {
                if (timing.time < ignoreOffset)
                {
                    CountNoteCount(timing.noteList);
                    continue;
                }
                List<TouchDrop> members = new();
                for (var i = 0; i < timing.noteList.Count; i++)
                {
                    var note = timing.noteList[i];
                    if (note.noteType == SimaiNoteType.Tap)
                    {
                        GameObject GOnote = null;
                        TapBase NDCompo = null;

                        if (note.isForceStar)
                        {
                            GOnote = Instantiate(starPrefab, notes.transform);
                            var _NDCompo = GOnote.GetComponent<StarDrop>();
                            _NDCompo.tapSpr = customSkin.Star;
                            _NDCompo.eachSpr = customSkin.Star_Each;
                            _NDCompo.breakSpr = customSkin.Star_Break;
                            _NDCompo.exSpr = customSkin.Star_Ex;
                            _NDCompo.tapLine = starLine;
                            _NDCompo.isFakeStarRotate = note.isFakeRotate;
                            _NDCompo.isFakeStar = true;
                            NDCompo = _NDCompo;
                        }
                        else
                        {
                            GOnote = Instantiate(tapPrefab, notes.transform);
                            NDCompo = GOnote.GetComponent<TapDrop>();
                            //自定义note样式
                            NDCompo.tapSpr = customSkin.Tap;
                            NDCompo.breakSpr = customSkin.Tap_Break;
                            NDCompo.eachSpr = customSkin.Tap_Each;
                            NDCompo.exSpr = customSkin.Tap_Ex;
                        }

                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.noteType];

                        NDCompo.BreakShine = BreakShine;

                        if (timing.noteList.Count > 1) NDCompo.isEach = true;
                        NDCompo.isBreak = note.isBreak;
                        NDCompo.isEX = note.isEx;
                        NDCompo.time = (float)timing.time;
                        NDCompo.startPosition = note.startPosition;
                        NDCompo.speed = noteSpeed * timing.HSpeed;
                    }
                    else if (note.noteType == SimaiNoteType.Hold)
                    {
                        var GOnote = Instantiate(holdPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<HoldDrop>();

                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.noteType];

                        NDCompo.tapSpr = customSkin.Hold;
                        NDCompo.holdOnSpr = customSkin.Hold_On;
                        NDCompo.holdOffSpr = customSkin.Hold_Off;
                        NDCompo.eachSpr = customSkin.Hold_Each;
                        NDCompo.eachHoldOnSpr = customSkin.Hold_Each_On;
                        NDCompo.exSpr = customSkin.Hold_Ex;
                        NDCompo.breakSpr = customSkin.Hold_Break;
                        NDCompo.breakHoldOnSpr = customSkin.Hold_Break_On;

                        NDCompo.HoldShine = HoldShine;
                        NDCompo.BreakShine = BreakShine;

                        if (timing.noteList.Count > 1) NDCompo.isEach = true;
                        NDCompo.time = (float)timing.time;
                        NDCompo.LastFor = (float)note.holdTime;
                        NDCompo.startPosition = note.startPosition;
                        NDCompo.speed = noteSpeed * timing.HSpeed;
                        NDCompo.isEX = note.isEx;
                        NDCompo.isBreak = note.isBreak;
                    }
                    else if (note.noteType == SimaiNoteType.TouchHold)
                    {
                        var GOnote = Instantiate(touchHoldPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<TouchHoldDrop>();

                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.noteType];

                        NDCompo.time = (float)timing.time;
                        NDCompo.LastFor = (float)note.holdTime;
                        NDCompo.speed = touchSpeed * timing.HSpeed;
                        NDCompo.isFirework = note.isHanabi;

                        Array.Copy(customSkin.TouchHold, NDCompo.TouchHoldSprite, 5);
                        NDCompo.TouchPointSprite = customSkin.TouchPoint;
                    }
                    else if (note.noteType == SimaiNoteType.Touch)
                    {
                        var GOnote = Instantiate(touchPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<TouchDrop>();

                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.noteType];

                        NDCompo.time = (float)timing.time;
                        NDCompo.areaPosition = note.touchArea;
                        NDCompo.startPosition = note.startPosition;

                        NDCompo.fanNormalSprite = customSkin.Touch;
                        NDCompo.fanEachSprite = customSkin.Touch_Each;
                        NDCompo.pointNormalSprite = customSkin.TouchPoint;
                        NDCompo.pointEachSprite = customSkin.TouchPoint_Each;
                        NDCompo.justSprite = customSkin.TouchJust;
                        Array.Copy(customSkin.TouchBorder, NDCompo.multTouchNormalSprite, 2);
                        Array.Copy(customSkin.TouchBorder_Each, NDCompo.multTouchEachSprite, 2);

                        if (timing.noteList.Count > 1)
                        {
                            NDCompo.isEach = true;
                            members.Add(NDCompo);
                        }
                        NDCompo.speed = touchSpeed * timing.HSpeed;
                        NDCompo.isFirework = note.isHanabi;
                        NDCompo.GroupInfo = null;
                    }

                    else if (note.noteType == SimaiNoteType.Slide)
                        InstantiateStarGroup(timing, note, i, lastNoteTime); // 星星组
                }


                if (members.Count != 0)
                {
                    var sensorTypes = members.GroupBy(x => x.GetSensor())
                                             .Select(x => x.Key)
                                             .ToList();
                    List<List<SensorType>> sensorGroups = new();

                    while (sensorTypes.Count > 0)
                    {
                        var sensorType = sensorTypes[0];
                        var existsGroup = sensorGroups.FindAll(x => x.Contains(sensorType));
                        var groupMap = TOUCH_GROUPS[sensorType];
                        existsGroup.AddRange(sensorGroups.FindAll(x => x.Any(y => groupMap.Contains(y))));

                        var groupMembers = existsGroup.SelectMany(x => x)
                                                      .ToList();
                        var newMembers = sensorTypes.FindAll(x => groupMap.Contains(x));

                        groupMembers.AddRange(newMembers);
                        groupMembers.Add(sensorType);
                        var newGroup = groupMembers.GroupBy(x => x)
                                                   .Select(x => x.Key)
                                                   .ToList();

                        foreach (var newMember in newGroup)
                            sensorTypes.Remove(newMember);
                        foreach (var oldGroup in existsGroup)
                            sensorGroups.Remove(oldGroup);

                        sensorGroups.Add(newGroup);
                    }
                    List<TouchGroup> touchGroups = new();
                    var memberMapping = members.ToDictionary(x => x.GetSensor());
                    foreach (var group in sensorGroups)
                    {
                        touchGroups.Add(new TouchGroup()
                        {
                            Members = group.Select(x => memberMapping[x]).ToArray()
                        });
                    }
                    foreach (var member in members)
                        member.GroupInfo = touchGroups.Find(x => x.Members.Any(y => y == member));
                }

                var eachNotes = timing.noteList.FindAll(o =>
                    o.noteType != SimaiNoteType.Touch && o.noteType != SimaiNoteType.TouchHold);
                if (eachNotes.Count > 1) //有多个非touchnote
                {
                    var startPos = eachNotes[0].startPosition;
                    var endPos = eachNotes[1].startPosition;
                    endPos = endPos - startPos;
                    if (endPos == 0) continue;

                    var line = Instantiate(eachLine, notes.transform);
                    var lineDrop = line.GetComponent<EachLineDrop>();

                    lineDrop.time = (float)timing.time;
                    lineDrop.speed = noteSpeed * timing.HSpeed;

                    endPos = endPos < 0 ? endPos + 8 : endPos;
                    endPos = endPos > 8 ? endPos - 8 : endPos;
                    endPos++;

                    if (endPos > 4)
                    {
                        startPos = eachNotes[1].startPosition;
                        endPos = eachNotes[0].startPosition;
                        endPos = endPos - startPos;
                        endPos = endPos < 0 ? endPos + 8 : endPos;
                        endPos = endPos > 8 ? endPos - 8 : endPos;
                        endPos++;
                    }

                    lineDrop.startPosition = startPos;
                    lineDrop.curvLength = endPos - 1;
                }
            }
            catch (Exception e)
            {
                GameObject.Find("ErrText").GetComponent<Text>().text =
                    "在第" + (timing.rawTextPositionY + 1) + "行发现问题：\n" + e.Message;
            }
        }
    }


    private void CountNoteSum(Majson json)
    {
        foreach (var timing in json.timingList)
            foreach (var note in timing.noteList)
                if (!note.isBreak)
                {
                    if (note.noteType == SimaiNoteType.Tap) ObjectCounter.tapSum++;
                    if (note.noteType == SimaiNoteType.Hold) ObjectCounter.holdSum++;
                    if (note.noteType == SimaiNoteType.TouchHold) ObjectCounter.holdSum++;
                    if (note.noteType == SimaiNoteType.Touch) ObjectCounter.touchSum++;
                    if (note.noteType == SimaiNoteType.Slide)
                    {
                        if (!note.isSlideNoHead) ObjectCounter.tapSum++;
                        if (note.isSlideBreak)
                            ObjectCounter.breakSum++;
                        else
                            ObjectCounter.slideSum++;
                    }
                }
                else
                {
                    if (note.noteType == SimaiNoteType.Slide)
                    {
                        if (!note.isSlideNoHead) ObjectCounter.breakSum++;
                        if (note.isSlideBreak)
                            ObjectCounter.breakSum++;
                        else
                            ObjectCounter.slideSum++;
                    }
                    else
                    {
                        ObjectCounter.breakSum++;
                    }
                }
    }

    private void CountNoteCount(List<SimaiNote> timing)
    {
        foreach (var note in timing)
            if (!note.isBreak)
            {
                if (note.noteType == SimaiNoteType.Tap) ObjectCounter.tapCount++;
                if (note.noteType == SimaiNoteType.Hold) ObjectCounter.holdCount++;
                if (note.noteType == SimaiNoteType.TouchHold) ObjectCounter.holdCount++;
                if (note.noteType == SimaiNoteType.Touch) ObjectCounter.touchCount++;
                if (note.noteType == SimaiNoteType.Slide)
                {
                    if (!note.isSlideNoHead) ObjectCounter.tapCount++;
                    if (note.isSlideBreak)
                        ObjectCounter.breakCount++;
                    else
                        ObjectCounter.slideCount++;
                }
            }
            else
            {
                if (note.noteType == SimaiNoteType.Slide)
                {
                    if (!note.isSlideNoHead) ObjectCounter.breakCount++;
                    if (note.isSlideBreak)
                        ObjectCounter.breakCount++;
                    else
                        ObjectCounter.slideCount++;
                }
                else
                {
                    ObjectCounter.breakCount++;
                }
            }
    }

    private void InstantiateStarGroup(SimaiTimingPoint timing, SimaiNote note, int sort, double lastNoteTime)
    {
        int charIntParse(char c)
        {
            return c - '0';
        }

        var subSlide = new List<SimaiNote>();
        var subBarCount = new List<int>();
        var sumBarCount = 0;

        var noteContent = note.noteContent;
        var latestStartIndex = charIntParse(noteContent[0]); // 存储上一个Slide的结尾 也就是下一个Slide的起点
        var ptr = 1; // 指向目前处理的字符

        var specTimeFlag = 0; // 表示此组合slide是指定总时长 还是指定每一段的时长
        // 0-目前还没有读取 1-读取到了一个未指定时长的段落 2-读取到了一个指定时长的段落 3-（期望）读取到了最后一个时长指定

        while (ptr < noteContent.Length)
            if (!char.IsNumber(noteContent[ptr]))
            {
                // 读取到字符
                var slideTypeChar = noteContent[ptr++].ToString();

                var slidePart = new SimaiNote();
                slidePart.noteType = SimaiNoteType.Slide;
                slidePart.startPosition = latestStartIndex;
                if (slideTypeChar == "V")
                {
                    // 转折星星
                    var middlePos = noteContent[ptr++];
                    var endPos = noteContent[ptr++];

                    slidePart.noteContent = latestStartIndex + slideTypeChar + middlePos + endPos;
                    latestStartIndex = charIntParse(endPos);
                }
                else
                {
                    // 其他普通星星
                    // 额外检查pp和qq
                    if (noteContent[ptr] == slideTypeChar[0]) slideTypeChar += noteContent[ptr++];
                    var endPos = noteContent[ptr++];

                    slidePart.noteContent = latestStartIndex + slideTypeChar + endPos;
                    latestStartIndex = charIntParse(endPos);
                }

                if (noteContent[ptr] == '[')
                {
                    // 如果指定了速度
                    if (specTimeFlag == 0)
                        // 之前未读取过
                        specTimeFlag = 2;
                    else if (specTimeFlag == 1)
                        // 之前读取到的都是未指定时长的段落 那么将flag设为3 如果之后又读取到时长 则报错
                        specTimeFlag = 3;
                    else if (specTimeFlag == 3)
                        // 之前读取到了指定时长 并期待那个时长就是最终时长 但是又读取到一个新的时长 则报错
                        throw new Exception("组合星星有错误\nSLIDE CHAIN ERROR");

                    while (ptr < noteContent.Length && noteContent[ptr] != ']')
                        slidePart.noteContent += noteContent[ptr++];
                    slidePart.noteContent += noteContent[ptr++];
                }
                else
                {
                    // 没有指定速度
                    if (specTimeFlag == 0)
                        // 之前未读取过
                        specTimeFlag = 1;
                    else if (specTimeFlag == 2 || specTimeFlag == 3)
                        // 之前读取到指定时长的段落了 说明这一条组合星星有的指定时长 有的没指定 则需要报错
                        throw new Exception("组合星星有错误\nSLIDE CHAIN ERROR");
                }

                string slideShape = detectShapeFromText(slidePart.noteContent);
                if (slideShape.StartsWith("-"))
                {
                    slideShape = slideShape.Substring(1);
                }
                int slideIndex = SLIDE_PREFAB_MAP[slideShape];
                if (slideIndex < 0) slideIndex = -slideIndex;

                var barCount = slidePrefab[slideIndex].transform.childCount;
                subBarCount.Add(barCount);
                sumBarCount += barCount;

                subSlide.Add(slidePart);
            }
            else
            {
                // 理论上来说 不应该读取到数字 因此如果读取到了 说明有语法错误
                throw new Exception("组合星星有错误\nwSLIDE CHAIN ERROR");
            }

        subSlide.ForEach(o =>
        {
            o.isBreak = note.isBreak;
            o.isEx = note.isEx;
            o.isSlideBreak = note.isSlideBreak;
            o.isSlideNoHead = true;
        });
        subSlide[0].isSlideNoHead = note.isSlideNoHead;

        if (specTimeFlag == 1 || specTimeFlag == 0)
            // 如果到结束还是1 那说明没有一个指定了时长 报错
            throw new Exception("组合星星有错误\nwSLIDE CHAIN ERROR");
        // 此时 flag为2表示每条指定语法 为3表示整体指定语法

        if (specTimeFlag == 3)
        {
            // 整体指定语法 使用slideTime来计算
            var tempBarCount = 0;
            for (var i = 0; i < subSlide.Count; i++)
            {
                subSlide[i].slideStartTime = note.slideStartTime + (double)tempBarCount / sumBarCount * note.slideTime;
                subSlide[i].slideTime = (double)subBarCount[i] / sumBarCount * note.slideTime;
                tempBarCount += subBarCount[i];
            }
        }
        else
        {
            // 每条指定语法

            // 获取时长的子函数
            double getTimeFromBeats(string noteText, float currentBpm)
            {
                var startIndex = noteText.IndexOf('[');
                var overIndex = noteText.IndexOf(']');
                var innerString = noteText.Substring(startIndex + 1, overIndex - startIndex - 1);
                var timeOneBeat = 1d / (currentBpm / 60d);
                if (innerString.Count(o => o == '#') == 1)
                {
                    var times = innerString.Split('#');
                    if (times[1].Contains(':'))
                    {
                        innerString = times[1];
                        timeOneBeat = 1d / (double.Parse(times[0]) / 60d);
                    }
                    else
                    {
                        return double.Parse(times[1]);
                    }
                }

                if (innerString.Count(o => o == '#') == 2)
                {
                    var times = innerString.Split('#');
                    return double.Parse(times[2]);
                }

                var numbers = innerString.Split(':');
                var divide = int.Parse(numbers[0]);
                var count = int.Parse(numbers[1]);


                return timeOneBeat * 4d / divide * count;
            }

            double tempSlideTime = 0;
            for (var i = 0; i < subSlide.Count; i++)
            {
                subSlide[i].slideStartTime = note.slideStartTime + tempSlideTime;
                subSlide[i].slideTime = getTimeFromBeats(subSlide[i].noteContent, timing.currentBpm);
                tempSlideTime += subSlide[i].slideTime;
            }
        }

        GameObject parent = null;
        List<SlideDrop> subSlides = new();
        float totalLen = (float)subSlide.Select(x => x.slideTime).Sum();
        float totalSlideLen = 0;
        for (var i = 0; i <= subSlide.Count - 1; i++)
        {
            bool isConn = subSlide.Count != 1;
            bool isGroupHead = i == 0;
            bool isGroupEnd = i == subSlide.Count - 1;
            if (note.noteContent.Contains('w')) //wifi
            {
                if (isConn)
                    throw new InvalidOperationException("不允许Wifi Slide作为Connection Slide的一部分");
                InstantiateWifi(timing, subSlide[i]);
            }
            else
            {
                ConnSlideInfo info = new ConnSlideInfo()
                {
                    TotalLength = totalLen,
                    IsGroupPart = isConn,
                    IsGroupPartHead = isGroupHead,
                    IsGroupPartEnd = isGroupEnd,
                    Parent = parent
                };
                parent = InstantiateStar(timing, subSlide[i], info);
                subSlides.Add(parent.GetComponent<SlideDrop>());
            }
        }
        subSlides.ForEach(s =>
        {
            s.Initialize();
            totalSlideLen += s.GetSlideLength();
        });
        subSlides.ForEach(s => s.ConnectInfo.TotalSlideLen = totalSlideLen);
    }

    private GameObject InstantiateWifi(SimaiTimingPoint timing, SimaiNote note)
    {
        var str = note.noteContent.Substring(0, 3);
        var digits = str.Split('w');
        var startPos = int.Parse(digits[0]);
        var endPos = int.Parse(digits[1]);
        endPos = endPos - startPos;
        endPos = endPos < 0 ? endPos + 8 : endPos;
        endPos = endPos > 8 ? endPos - 8 : endPos;
        endPos++;

        var GOnote = Instantiate(starPrefab, notes.transform);
        var NDCompo = GOnote.GetComponent<StarDrop>();



        // note的图层顺序
        NDCompo.noteSortOrder = noteSortOrder;
        noteSortOrder -= NOTE_LAYER_COUNT[note.noteType];

        NDCompo.tapSpr = customSkin.Star;
        NDCompo.eachSpr = customSkin.Star_Each;
        NDCompo.breakSpr = customSkin.Star_Break;
        NDCompo.exSpr = customSkin.Star_Ex;

        NDCompo.tapSpr_Double = customSkin.Star_Double;
        NDCompo.eachSpr_Double = customSkin.Star_Each_Double;
        NDCompo.breakSpr_Double = customSkin.Star_Break_Double;
        NDCompo.exSpr_Double = customSkin.Star_Ex_Double;

        NDCompo.BreakShine = BreakShine;

        NDCompo.rotateSpeed = (float)note.slideTime;
        NDCompo.isEX = note.isEx;
        NDCompo.isBreak = note.isBreak;

        var slideWifi = Instantiate(slidePrefab[SLIDE_PREFAB_MAP["wifi"]], notes.transform);
        slideWifi.SetActive(false);
        NDCompo.slide = slideWifi;
        var WifiCompo = slideWifi.GetComponent<WifiDrop>();

        WifiCompo.normalStar = customSkin.Star;
        WifiCompo.eachStar = customSkin.Star_Each;
        WifiCompo.breakStar = customSkin.Star_Break;
        WifiCompo.judgeBreakShine = JudgeBreakShine;
        WifiCompo.breakMaterial = breakMaterial;
        WifiCompo.slideShine = BreakShine;
        WifiCompo.areaStep = new List<int>(SLIDE_AREA_STEP_MAP["wifi"]);
        WifiCompo.judgeQueues = new(WIFISLIDE_JUDGE_QUEUE[startPos]);
        WifiCompo.slideConst = SLIDE_AREA_CONST["wifi"];
        WifiCompo.smoothSlideAnime = smoothSlideAnime;

        Array.Copy(customSkin.Wifi, WifiCompo.normalSlide, 11);
        Array.Copy(customSkin.Wifi_Each, WifiCompo.eachSlide, 11);
        Array.Copy(customSkin.Wifi_Break, WifiCompo.breakSlide, 11);

        if (timing.noteList.Count > 1)
        {
            NDCompo.isEach = true;
            NDCompo.isDouble = false;
            if (timing.noteList.FindAll(
                    o => o.noteType == SimaiNoteType.Slide).Count
                > 1)
                WifiCompo.isEach = true;
            var count = timing.noteList.FindAll(
                o => o.noteType == SimaiNoteType.Slide &&
                     o.startPosition == note.startPosition).Count;
            if (count > 1) //有同起点
            {
                NDCompo.isDouble = true;
                if (count == timing.noteList.Count)
                    NDCompo.isEach = false;
                else
                    NDCompo.isEach = true;
            }
        }

        WifiCompo.isBreak = note.isSlideBreak;

        NDCompo.isNoHead = note.isSlideNoHead;
        NDCompo.time = (float)timing.time;
        NDCompo.startPosition = note.startPosition;
        NDCompo.speed = noteSpeed * timing.HSpeed;

        WifiCompo.isJustR = detectJustType(note.noteContent, out endPos);
        WifiCompo.endPosition = endPos;
        WifiCompo.speed = noteSpeed * timing.HSpeed;
        WifiCompo.timeStart = (float)timing.time;
        WifiCompo.startPosition = note.startPosition;
        WifiCompo.time = (float)note.slideStartTime;
        WifiCompo.LastFor = (float)note.slideTime;
        WifiCompo.sortIndex = slideLayer;
        slideLayer -= SLIDE_AREA_STEP_MAP["wifi"].Last();
        //slideLayer += 5;

        return slideWifi;
    }

    private GameObject InstantiateStar(SimaiTimingPoint timing, SimaiNote note, ConnSlideInfo info)
    {
        var GOnote = Instantiate(starPrefab, notes.transform);
        var NDCompo = GOnote.GetComponent<StarDrop>();

        // note的图层顺序
        NDCompo.noteSortOrder = noteSortOrder;
        noteSortOrder -= NOTE_LAYER_COUNT[note.noteType];

        NDCompo.tapSpr = customSkin.Star;
        NDCompo.eachSpr = customSkin.Star_Each;
        NDCompo.breakSpr = customSkin.Star_Break;
        NDCompo.exSpr = customSkin.Star_Ex;

        NDCompo.tapSpr_Double = customSkin.Star_Double;
        NDCompo.eachSpr_Double = customSkin.Star_Each_Double;
        NDCompo.breakSpr_Double = customSkin.Star_Break_Double;
        NDCompo.exSpr_Double = customSkin.Star_Ex_Double;

        NDCompo.BreakShine = BreakShine;

        NDCompo.rotateSpeed = (float)note.slideTime;
        NDCompo.isEX = note.isEx;
        NDCompo.isBreak = note.isBreak;

        string slideShape = detectShapeFromText(note.noteContent);
        var isMirror = false;
        if (slideShape.StartsWith("-"))
        {
            isMirror = true;
            slideShape = slideShape.Substring(1);
        }
        int slideIndex = SLIDE_PREFAB_MAP[slideShape];

        var slide = Instantiate(slidePrefab[slideIndex], notes.transform);
        var slide_star = Instantiate(star_slidePrefab, notes.transform);
        slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star;
        slide_star.SetActive(false);
        slide.SetActive(false);
        NDCompo.slide = slide;
        var SliCompo = slide.AddComponent<SlideDrop>();

        SliCompo.slideType = slideShape;
        SliCompo.spriteNormal = customSkin.Slide;
        SliCompo.spriteEach = customSkin.Slide_Each;
        SliCompo.spriteBreak = customSkin.Slide_Break;
        SliCompo.slideShine = BreakShine;
        SliCompo.breakMaterial = breakMaterial;
        SliCompo.judgeBreakShine = JudgeBreakShine;
        SliCompo.areaStep = new List<int>(SLIDE_AREA_STEP_MAP[slideShape]);
        SliCompo.slideConst = SLIDE_AREA_CONST[slideShape];
        SliCompo.smoothSlideAnime = smoothSlideAnime;

        if (timing.noteList.Count > 1)
        {
            NDCompo.isEach = true;
            if (timing.noteList.FindAll(
                    o => o.noteType == SimaiNoteType.Slide).Count
                > 1)
            {
                SliCompo.isEach = true;
                slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star_Each;
            }

            var count = timing.noteList.FindAll(
                o => o.noteType == SimaiNoteType.Slide &&
                     o.startPosition == note.startPosition).Count;
            if (count > 1)
            {
                NDCompo.isDouble = true;
                if (count == timing.noteList.Count)
                    NDCompo.isEach = false;
                else
                    NDCompo.isEach = true;
            }
        }

        SliCompo.ConnectInfo = info;
        SliCompo.isBreak = note.isSlideBreak;
        if (note.isSlideBreak) slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star_Break;

        NDCompo.isNoHead = note.isSlideNoHead;
        NDCompo.time = (float)timing.time;
        NDCompo.startPosition = note.startPosition;
        NDCompo.speed = noteSpeed * timing.HSpeed;


        SliCompo.isMirror = isMirror;
        SliCompo.isJustR = detectJustType(note.noteContent, out int endPos);
        SliCompo.endPosition = endPos;
        if (slideIndex - 26 > 0 && slideIndex - 26 <= 8)
        {
            // known slide sprite issue
            //    1 2 3 4 5 6 7 8
            // p  X X X X X X O O
            // q  X O O X X X X X
            var pqEndPos = slideIndex - 26;
            SliCompo.isSpecialFlip = isMirror == (pqEndPos == 7 || pqEndPos == 8);
        }
        else
        {
            SliCompo.isSpecialFlip = isMirror;
        }
        SliCompo.speed = noteSpeed * timing.HSpeed;
        SliCompo.timeStart = (float)timing.time;
        SliCompo.startPosition = note.startPosition;
        SliCompo.star_slide = slide_star;
        SliCompo.time = (float)note.slideStartTime;
        SliCompo.LastFor = (float)note.slideTime;
        //SliCompo.sortIndex = -7000 + (int)((lastNoteTime - timing.time) * -100) + sort * 5;
        SliCompo.sortIndex = slideLayer;
        slideLayer -= SLIDE_AREA_STEP_MAP[slideShape].Last();
        //slideLayer += 5;
        return slide;
    }

    private bool detectJustType(string content, out int endPos)
    {
        // > < ^ V w
        if (content.Contains('>'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('>');
            var startPos = int.Parse(digits[0]);
            endPos = int.Parse(digits[1]);
            if (isUpperHalf(startPos))
                return true;
            return false;
        }

        if (content.Contains('<'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('<');
            var startPos = int.Parse(digits[0]);
            endPos = int.Parse(digits[1]);
            if (!isUpperHalf(startPos))
                return true;
            return false;
        }

        if (content.Contains('^'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('^');
            var startPos = int.Parse(digits[0]);
            endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;

            if (endPos < 4)
            {
                endPos = int.Parse(digits[1]);
                return true;
            }
            if (endPos > 4)
            {
                endPos = int.Parse(digits[1]);
                return false;
            }
        }
        else if (content.Contains('V'))
        {
            var str = content.Substring(0, 4);
            var digits = str.Split('V');
            endPos = int.Parse(digits[1][1].ToString());

            if (isRightHalf(endPos))
                return true;
            return false;
        }
        else if (content.Contains('w'))
        {
            var str = content.Substring(0, 3);
            endPos = int.Parse(str.Substring(2, 1));
            if (isUpperHalf(endPos))
                return true;
            return false;
        }
        else
        {
            //int endPos;
            if (content.Contains("qq") || content.Contains("pp"))
                endPos = int.Parse(content.Substring(3, 1));
            else
                endPos = int.Parse(content.Substring(2, 1));
            if (isRightHalf(endPos))
                return true;
            return false;
        }
        return true;
    }

    private string detectShapeFromText(string content)
    {
        int getRelativeEndPos(int startPos, int endPos)
        {
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            return endPos + 1;
        }

        //print(content);
        if (content.Contains('-'))
        {
            // line
            var str = content.Substring(0, 3); //something like "8-6"
            var digits = str.Split('-');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos < 3 || endPos > 7) throw new Exception("-星星至少隔开一键\n-スライドエラー");
            return "line" + endPos;
        }

        if (content.Contains('>'))
        {
            // circle 默认顺时针
            var str = content.Substring(0, 3);
            var digits = str.Split('>');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (isUpperHalf(startPos))
            {
                return "circle" + endPos;
            }

            endPos = MirrorKeys(endPos);
            return "-circle" + endPos; //Mirror
        }

        if (content.Contains('<'))
        {
            // circle 默认顺时针
            var str = content.Substring(0, 3);
            var digits = str.Split('<');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (!isUpperHalf(startPos))
            {
                return "circle" + endPos;
            }

            endPos = MirrorKeys(endPos);
            return "-circle" + endPos; //Mirror
        }

        if (content.Contains('^'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('^');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);

            if (endPos == 1 || endPos == 5)
            {
                throw new Exception("^星星不合法\n^スライドエラー");
            }

            if (endPos < 5)
            {
                return "circle" + endPos;
            }
            if (endPos > 5)
            {
                return "-circle" + MirrorKeys(endPos);
            }
        }

        if (content.Contains('v'))
        {
            // v
            var str = content.Substring(0, 3);
            var digits = str.Split('v');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos == 5) throw new Exception("v星星不合法\nvスライドエラー");
            return "v" + endPos;
        }

        if (content.Contains("pp"))
        {
            // ppqq 默认为pp
            var str = content.Substring(0, 4);
            var digits = str.Split('p');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[2]);
            endPos = getRelativeEndPos(startPos, endPos);
            return "ppqq" + endPos;
        }

        if (content.Contains("qq"))
        {
            // ppqq 默认为pp
            var str = content.Substring(0, 4);
            var digits = str.Split('q');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[2]);
            endPos = getRelativeEndPos(startPos, endPos);
            endPos = MirrorKeys(endPos);
            return "-ppqq" + endPos;
        }

        if (content.Contains('p'))
        {
            // pq 默认为p
            var str = content.Substring(0, 3);
            var digits = str.Split('p');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            return "pq" + endPos;
        }

        if (content.Contains('q'))
        {
            // pq 默认为p
            var str = content.Substring(0, 3);
            var digits = str.Split('q');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            endPos = MirrorKeys(endPos);
            return "-pq" + endPos;
        }

        if (content.Contains('s'))
        {
            // s
            var str = content.Substring(0, 3);
            var digits = str.Split('s');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos != 5) throw new Exception("s星星尾部错误\nsスライドエラー");
            return "s";
        }

        if (content.Contains('z'))
        {
            // s镜像
            var str = content.Substring(0, 3);
            var digits = str.Split('z');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos != 5) throw new Exception("z星星尾部错误\nzスライドエラー");
            return "-s";
        }

        if (content.Contains('V'))
        {
            // L
            var str = content.Substring(0, 4);
            var digits = str.Split('V');
            var startPos = int.Parse(digits[0]);
            var turnPos = int.Parse(digits[1][0].ToString());
            var endPos = int.Parse(digits[1][1].ToString());

            turnPos = getRelativeEndPos(startPos, turnPos);
            endPos = getRelativeEndPos(startPos, endPos);
            if (turnPos == 7)
            {
                if (endPos < 2 || endPos > 5) throw new Exception("V星星终点不合法\nVスライドエラー");
                return "L" + endPos;
            }

            if (turnPos == 3)
            {
                if (endPos < 5) throw new Exception("V星星终点不合法\nVスライドエラー");
                return "-L" + MirrorKeys(endPos);
            }

            throw new Exception("V星星拐点只能隔开一键\nVスライドエラー");
        }

        if (content.Contains('w'))
        {
            // wifi
            var str = content.Substring(0, 3);
            var digits = str.Split('w');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos != 5) throw new Exception("w星星尾部错误\nwスライドエラー");
            return "wifi";
        }

        return "";
    }

    private bool isUpperHalf(int key)
    {
        if (key == 7) return true;
        if (key == 8) return true;
        if (key == 1) return true;
        if (key == 2) return true;

        return false;
    }

    private bool isRightHalf(int key)
    {
        if (key == 1) return true;
        if (key == 2) return true;
        if (key == 3) return true;
        if (key == 4) return true;

        return false;
    }

    private int MirrorKeys(int key)
    {
        if (key == 1) return 1;
        if (key == 2) return 8;
        if (key == 3) return 7;
        if (key == 4) return 6;

        if (key == 5) return 5;
        if (key == 6) return 4;
        if (key == 7) return 3;
        if (key == 8) return 2;
        throw new Exception("Keys out of range: " + key);
    }
}