﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class JsonDataLoader : MonoBehaviour
{
    public float noteSpeed = 7f;
    public float touchSpeed = 7.5f;
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
    public RuntimeAnimatorController BreakShine;
    public RuntimeAnimatorController HoldShine;

    public Text diffText;
    public Text levelText;
    public Text titleText;
    public Text artistText;
    public Text designText;
    public RawImage cardImage;
    public Color[] diffColors = new Color[7];

    ObjectCounter ObjectCounter;
    CustomSkin customSkin;

    int slideLayer = -10000;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        ObjectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();
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

        double lastNoteTime = loadedData.timingList.Last().time;

        foreach (var timing in loadedData.timingList)
        {
            try {
                if (timing.time < ignoreOffset) {
                    CountNoteCount(timing.noteList);
                    continue;
                }
                for (int i = 0; i < timing.noteList.Count; i++)
                {
                    var note = timing.noteList[i];
                    if (note.noteType == SimaiNoteType.Tap)
                    {
                        var GOnote = Instantiate(tapPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<TapDrop>();

                        if (note.isForceStar)
                        {
                            NDCompo.normalSpr = customSkin.Star; 
                            NDCompo.eachSpr = customSkin.Star_Each;
                            NDCompo.breakSpr = customSkin.Star_Break;
                            NDCompo.exSpr = customSkin.Star_Ex;
                            NDCompo.tapLine = starLine;
                            NDCompo.isFakeStarRotate = note.isFakeRotate;
                        }
                        else
                        {
                            //自定义note样式
                            NDCompo.normalSpr = customSkin.Tap;
                            NDCompo.breakSpr = customSkin.Tap_Break;
                            NDCompo.eachSpr = customSkin.Tap_Each;
                            NDCompo.exSpr = customSkin.Tap_Ex;
                        }

                        NDCompo.BreakShine = BreakShine;

                        if (timing.noteList.Count > 1) NDCompo.isEach = true;
                        NDCompo.isBreak = note.isBreak;
                        NDCompo.isEX = note.isEx;
                        NDCompo.time = (float)timing.time;
                        NDCompo.startPosition = note.startPosition;
                        NDCompo.speed = noteSpeed * timing.HSpeed;
                    }
                    if (note.noteType == SimaiNoteType.Hold)
                    {
                        var GOnote = Instantiate(holdPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<HoldDrop>();

                        NDCompo.tapSpr = customSkin.Hold;
                        NDCompo.eachSpr = customSkin.Hold_Each;
                        NDCompo.exSpr = customSkin.Hold_Ex;
                        NDCompo.breakSpr = customSkin.Hold_Break;

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
                    if (note.noteType == SimaiNoteType.TouchHold)
                    {
                        var GOnote = Instantiate(touchHoldPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<TouchHoldDrop>();
                        NDCompo.time = (float)timing.time;
                        NDCompo.LastFor = (float)note.holdTime;
                        NDCompo.speed = touchSpeed * timing.HSpeed;
                        NDCompo.isFirework = note.isHanabi;

                        Array.Copy(customSkin.TouchHold, NDCompo.TouchHoldSprite, 5);
                        NDCompo.TouchPointSprite = customSkin.TouchPoint;
                    }
                    if (note.noteType == SimaiNoteType.Touch)
                    {
                        var GOnote = Instantiate(touchPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<TouchDrop>();
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

                        if (timing.noteList.Count > 1) NDCompo.isEach = true;
                        NDCompo.speed = touchSpeed * timing.HSpeed;
                        NDCompo.isFirework = note.isHanabi;
                    }
                    if (note.noteType == SimaiNoteType.Slide)
                    {
                        InstantiateStarGroup(timing, note, i, lastNoteTime);    // 星星组
                    }
                }
                var eachNotes = timing.noteList.FindAll(o => o.noteType != SimaiNoteType.Touch && o.noteType != SimaiNoteType.TouchHold);
                if (eachNotes.Count > 1)//有多个非touchnote
                {
                    int startPos = eachNotes[0].startPosition;
                    int endPos = eachNotes[1].startPosition;
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
                    lineDrop.curvLength = endPos-1;
                }
            }catch(Exception e)
            {
                GameObject.Find("ErrText").GetComponent<Text>().text = "在第"+(timing.rawTextPositionY+1 )+"行发现问题：\n"+e.Message;
            }
        }
    }


    void CountNoteSum(Majson json)
    {
        foreach (var timing in json.timingList)
        {
            foreach (var note in timing.noteList)
            {
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
                        {
                            ObjectCounter.breakSum++;
                        }
                        else
                        {
                            ObjectCounter.slideSum++;
                        }
                    }
                }
                else
                {
                    if (note.noteType == SimaiNoteType.Slide)
                    {
                        if (!note.isSlideNoHead) ObjectCounter.breakSum++;
                        if (note.isSlideBreak)
                        {
                            ObjectCounter.breakSum++;
                        }
                        else
                        {
                            ObjectCounter.slideSum++;
                        }
                    }
                    else { ObjectCounter.breakSum++; }
                }
            }
        }
    }

    void CountNoteCount(List<SimaiNote> timing)
    {
        foreach (var note in timing)
        {
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
                    {
                        ObjectCounter.breakCount++;
                    }
                    else
                    {
                        ObjectCounter.slideCount++;
                    }
                }
            }
            else
            {
                if (note.noteType == SimaiNoteType.Slide)
                {
                    if (!note.isSlideNoHead) ObjectCounter.breakCount++;
                    if (note.isSlideBreak)
                    {
                        ObjectCounter.breakCount++;
                    }
                    else
                    {
                        ObjectCounter.slideCount++;
                    }
                }
                else { ObjectCounter.breakCount++; }
            }
        }
    }

    void InstantiateStarGroup(SimaiTimingPoint timing, SimaiNote note, int sort, double lastNoteTime)
    {
        int charIntParse(char c)
        {
            return c - '0';
        }

        List<SimaiNote> subSlide = new List<SimaiNote>();
        List<int> subBarCount = new List<int>();
        int sumBarCount = 0;

        string noteContent = note.noteContent;
        int latestStartIndex = charIntParse(noteContent[0]); // 存储上一个Slide的结尾 也就是下一个Slide的起点
        int ptr = 1; // 指向目前处理的字符

        int specTimeFlag = 0; // 表示此组合slide是指定总时长 还是指定每一段的时长
        // 0-目前还没有读取 1-读取到了一个未指定时长的段落 2-读取到了一个指定时长的段落 3-（期望）读取到了最后一个时长指定

        while (ptr < noteContent.Length)
        {
            if (!Char.IsNumber(noteContent[ptr]))
            {
                // 读取到字符
                string slideTypeChar = noteContent[ptr++].ToString();

                SimaiNote slidePart = new SimaiNote();
                slidePart.noteType = SimaiNoteType.Slide;
                slidePart.startPosition = latestStartIndex;
                if (slideTypeChar == "V")
                {
                    // 转折星星
                    char middlePos = noteContent[ptr++];
                    char endPos = noteContent[ptr++];

                    slidePart.noteContent = latestStartIndex.ToString() + slideTypeChar + middlePos + endPos;
                    latestStartIndex = charIntParse(endPos);
                }
                else
                {
                    // 其他普通星星
                    // 额外检查pp和qq
                    if (noteContent[ptr] == slideTypeChar[0])
                    {
                        slideTypeChar += noteContent[ptr++];
                    }
                    char endPos = noteContent[ptr++];

                    slidePart.noteContent = latestStartIndex.ToString() + slideTypeChar + endPos;
                    latestStartIndex = charIntParse(endPos);
                }

                if (noteContent[ptr] == '[')
                {
                    // 如果指定了速度
                    if (specTimeFlag == 0)
                    {
                        // 之前未读取过
                        specTimeFlag = 2;
                    }
                    else if (specTimeFlag == 1)
                    {
                        // 之前读取到的都是未指定时长的段落 那么将flag设为3 如果之后又读取到时长 则报错
                        specTimeFlag = 3;
                    }
                    else if (specTimeFlag == 3)
                    {
                        // 之前读取到了指定时长 并期待那个时长就是最终时长 但是又读取到一个新的时长 则报错
                        throw new Exception("组合星星有错误\nSLIDE CHAIN ERROR");
                    }

                    while (ptr < noteContent.Length && noteContent[ptr] != ']')
                    {
                        slidePart.noteContent += noteContent[ptr++];
                    }
                    slidePart.noteContent += noteContent[ptr++];
                }
                else
                {
                    // 没有指定速度
                    if (specTimeFlag == 0)
                    {
                        // 之前未读取过
                        specTimeFlag = 1;
                    }
                    else if (specTimeFlag == 2 || specTimeFlag == 3)
                    {
                        // 之前读取到指定时长的段落了 说明这一条组合星星有的指定时长 有的没指定 则需要报错
                        throw new Exception("组合星星有错误\nSLIDE CHAIN ERROR");
                    }
                }

                var slideIndex = detectShapeFromText(slidePart.noteContent);
                if (slideIndex < 0) { slideIndex = -slideIndex; }

                int barCount = slidePrefab[slideIndex].transform.childCount;
                subBarCount.Add(barCount);
                sumBarCount += barCount;

                subSlide.Add(slidePart);
            }
            else
            {
                // 理论上来说 不应该读取到数字 因此如果读取到了 说明有语法错误
                throw new Exception("组合星星有错误\nwSLIDE CHAIN ERROR");
            }
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
        {
            // 如果到结束还是1 那说明没有一个指定了时长 报错
            throw new Exception("组合星星有错误\nwSLIDE CHAIN ERROR");
        }
        // 此时 flag为2表示每条指定语法 为3表示整体指定语法

        if (specTimeFlag == 3)
        {
            // 整体指定语法 使用slideTime来计算
            int tempBarCount = 0;
            for (int i = 0; i < subSlide.Count; i++)
            {
                subSlide[i].slideStartTime = note.slideStartTime + ((double)tempBarCount / sumBarCount) * note.slideTime;
                subSlide[i].slideTime = ((double)subBarCount[i] / sumBarCount) * note.slideTime;
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


                return (timeOneBeat * 4d / (double)divide) * (double)count;
            }

            double tempSlideTime = 0;
            for (int i = 0; i < subSlide.Count; i++)
            {
                subSlide[i].slideStartTime = note.slideStartTime+tempSlideTime;
                subSlide[i].slideTime = getTimeFromBeats(subSlide[i].noteContent, timing.currentBpm);
                tempSlideTime += subSlide[i].slideTime;
            }
        }

        for (int i = subSlide.Count - 1; i >= 0; i--)
        {
            if (note.noteContent.Contains('w')) //wifi
            {
                InstantiateWifi(timing, subSlide[i], i != 0, i == subSlide.Count - 1);
            }
            else
            {
                InstantiateStar(timing, subSlide[i], i != 0, i == subSlide.Count - 1);
            }
        }
    }

    void InstantiateWifi(SimaiTimingPoint timing, SimaiNote note, bool isGroupPart, bool isGroupPartEnd)
    {
        var str = note.noteContent.Substring(0, 3);
        var digits = str.Split('w');
        int startPos = int.Parse(digits[0]);
        int endPos = int.Parse(digits[1]);
        endPos = endPos - startPos;
        endPos = endPos < 0 ? endPos + 8 : endPos;
        endPos = endPos > 8 ? endPos - 8 : endPos;
        endPos++;
        if (endPos != 5) throw new Exception("w星星尾部错误\nwスライドエラー");

        var GOnote = Instantiate(starPrefab, notes.transform);
        var NDCompo = GOnote.GetComponent<StarDrop>();

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

        var slideWifi = Instantiate(slidePrefab[36], notes.transform);
        slideWifi.SetActive(false);
        NDCompo.slide = slideWifi;
        var WifiCompo = slideWifi.GetComponent<WifiDrop>();

        WifiCompo.normalStar = customSkin.Star;
        WifiCompo.eachStar = customSkin.Star_Each;
        WifiCompo.breakStar = customSkin.Star_Break;
        WifiCompo.slideShine = BreakShine;

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
            {
                WifiCompo.isEach = true;
            }
            var count = timing.noteList.FindAll(
            o => o.noteType == SimaiNoteType.Slide &&
            o.startPosition == note.startPosition).Count;
            if (count > 1)//有同起点
            {
                NDCompo.isDouble = true;
                if (count == timing.noteList.Count)
                    NDCompo.isEach = false;
                else
                    NDCompo.isEach = true;
            }
        }

        WifiCompo.isBreak = note.isSlideBreak;
        WifiCompo.isGroupPart = isGroupPart;
        WifiCompo.isGroupPartEnd = isGroupPartEnd;

        NDCompo.isNoHead = note.isSlideNoHead;
        NDCompo.time = (float)timing.time;
        NDCompo.startPosition = note.startPosition;
        NDCompo.speed = noteSpeed * timing.HSpeed;

        WifiCompo.isJustR = detectJustType(note.noteContent);
        WifiCompo.speed = noteSpeed * timing.HSpeed;
        WifiCompo.timeStart = (float)timing.time;
        WifiCompo.startPosition = note.startPosition;
        WifiCompo.time = (float)note.slideStartTime;
        WifiCompo.LastFor = (float)note.slideTime;
        WifiCompo.sortIndex = slideLayer;
        slideLayer += 5;
    }

    void InstantiateStar(SimaiTimingPoint timing, SimaiNote note, bool isGroupPart, bool isGroupPartEnd)
    {

        var GOnote = Instantiate(starPrefab, notes.transform);
        var NDCompo = GOnote.GetComponent<StarDrop>();

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

        var slideIndex = detectShapeFromText(note.noteContent);
        bool isMirror = false;
        if (slideIndex < 0) { isMirror = true; slideIndex = -slideIndex; }

        var slide = Instantiate(slidePrefab[slideIndex], notes.transform);
        var slide_star = Instantiate(star_slidePrefab, notes.transform);
        slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star;
        slide_star.SetActive(false);
        slide.SetActive(false);
        NDCompo.slide = slide;
        var SliCompo = slide.AddComponent<SlideDrop>();

        SliCompo.spriteNormal = customSkin.Slide;
        SliCompo.spriteEach = customSkin.Slide_Each;
        SliCompo.spriteBreak = customSkin.Slide_Break;
        SliCompo.slideShine = BreakShine;

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

        SliCompo.isBreak = note.isSlideBreak;
        SliCompo.isGroupPart = isGroupPart;
        SliCompo.isGroupPartEnd = isGroupPartEnd;
        if (note.isSlideBreak)
        {
            slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star_Break;
        }

        NDCompo.isNoHead = note.isSlideNoHead;
        NDCompo.time = (float)timing.time;
        NDCompo.startPosition = note.startPosition;
        NDCompo.speed = noteSpeed * timing.HSpeed;


        SliCompo.isMirror = isMirror;
        SliCompo.isJustR = detectJustType(note.noteContent);
        if ((slideIndex - 26) > 0 && (slideIndex - 26) <= 8)
        {
            // known slide sprite issue
            //    1 2 3 4 5 6 7 8
            // p  X X X X X X O O
            // q  X O O X X X X X
            int pqEndPos = slideIndex - 26;
            SliCompo.isSpecialFlip = isMirror == (pqEndPos == 7 || pqEndPos == 8);
        } else {
            SliCompo.isSpecialFlip = isMirror;
        }
        SliCompo.speed = noteSpeed * timing.HSpeed;
        SliCompo.timeStar = (float)timing.time;
        SliCompo.startPosition = note.startPosition;
        SliCompo.star_slide = slide_star;
        SliCompo.time = (float)note.slideStartTime;
        SliCompo.LastFor = (float)note.slideTime;
        //SliCompo.sortIndex = -7000 + (int)((lastNoteTime - timing.time) * -100) + sort * 5;
        SliCompo.sortIndex = slideLayer++;
        slideLayer += 5;
    }
    bool detectJustType(string content)
    {
        // > < ^ V w
        if (content.Contains('>')) {
            var str = content.Substring(0, 3); 
            var digits = str.Split('>');
            int startPos = int.Parse(digits[0]);
            if (isUpperHalf(startPos)) {
                return true;
            } else {
                return false;
            }
        } else if (content.Contains('<')) {
            var str = content.Substring(0, 3);
            var digits = str.Split('<');
            int startPos = int.Parse(digits[0]);
            if (!isUpperHalf(startPos)) {
                return true;
            } else {
                return false;
            }
        } else if (content.Contains('^')) {
            var str = content.Substring(0, 3); 
            var digits = str.Split('^');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;

            if (endPos < 4) {
                return true;
            } else if (endPos > 4) {
                return false;
            }
        } else if (content.Contains('V')) {
            var str = content.Substring(0, 4);
            var digits = str.Split('V');
            int endPos = int.Parse(digits[1][1].ToString());

            if (isRightHalf(endPos)) {
                return true;
            } else {
                return false;
            }
        } else if (content.Contains('w')) {
            var str = content.Substring(0, 3);
            var endPos = int.Parse(str.Substring(2, 1));
            if (isUpperHalf(endPos)) {
                return true;
            } else {
                return false;
            }
        } else {
            int endPos;
            if (content.Contains("qq") || content.Contains("pp")) {
                endPos = int.Parse(content.Substring(3, 1));
            } else {
                endPos = int.Parse(content.Substring(2, 1));
            }
            if (isRightHalf(endPos)) {
                return true;
            } else {
                return false;
            }
        }
        
        return true;
    }
    int detectShapeFromText(string content)
    {
        //print(content);
        if (content.Contains('-'))
        {
            /*
             * SlidePrefab 0 1 2 3 4
             * Star_Line   3 4 5 6 7
             */
            var str = content.Substring(0, 3); //something like "8-6"
            var digits = str.Split('-');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            if (endPos < 3 || endPos > 7) throw new Exception("-星星至少隔开一键\n-スライドエラー");
            return endPos - 3;
        }
        if (content.Contains('>'))
        {
            /*
             * SlidePrefab 5 6 7 8 9 10 11 12
             * Star_Circle 1 2 3 4 5 6  7  8 
             */
            var str = content.Substring(0, 3); 
            var digits = str.Split('>');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            if (isUpperHalf(startPos))
            {
                endPos = endPos - startPos;
                endPos = endPos < 0 ? endPos + 8 : endPos;
                endPos = endPos > 8 ? endPos - 8 : endPos;
                endPos++;
                return endPos + 4;
            }
            else
            {
                endPos = endPos - startPos;
                endPos = endPos < 0 ? endPos + 8 : endPos;
                endPos = endPos > 8 ? endPos - 8 : endPos;
                endPos++;
                if (endPos != 1) endPos = MirrorKeys(endPos);
                return -(endPos + 4); //Mirror
            }
        }
        if (content.Contains('<'))
        {
            /* (Mirror)
             * SlidePrefab 5 6 7 8 9 10 11 12
             * Star_Circle 1 2 3 4 5 6  7  8 
             */
            var str = content.Substring(0, 3);
            var digits = str.Split('<');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            if (!isUpperHalf(startPos))
            {
                endPos = endPos - startPos;
                endPos = endPos < 0 ? endPos + 8 : endPos;
                endPos = endPos > 8 ? endPos - 8 : endPos;
                endPos++;
                return endPos + 4;
            }
            else
            {
                endPos = endPos - startPos;
                endPos = endPos < 0 ? endPos + 8 : endPos;
                endPos = endPos > 8 ? endPos - 8 : endPos;
                endPos++;
                if (endPos != 1) endPos = MirrorKeys(endPos);
                return -(endPos + 4); //Mirror
            }
        }
        if (content.Contains('^'))
        {
            var str = content.Substring(0, 3); 
            var digits = str.Split('^');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;

            if (endPos < 4)
            {
                return endPos + 1+4;
            }
            if (endPos > 4)
            {
                return -(MirrorKeys(endPos + 1) + 4);
            }
            throw new Exception("^星星不合法\n^スライドエラー");
        }
        if (content.Contains('v'))
        {
            /* (Mirror)
             * SlidePrefab 13 14 15 16 17 18
             * Star_Circle 2  3  4  6  7  8
             */
            var str = content.Substring(0, 3);
            var digits = str.Split('v');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            if (endPos == 5 || endPos == 1) throw new Exception("v星星不合法\nvスライドエラー");
            if (endPos > 4) return endPos + 10;
            if (endPos < 6) return endPos + 11;
        }
        if (content.Contains("pp"))
        {
            /* (Mirror)
             * SlidePrefab 19 20 21 22 23 24 25 26
             * Star_Circle 1  2  3  4  5  6  7  8
             */
            var str = content.Substring(0, 4);
            var digits = str.Split('p');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[2]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            return endPos + 18;
        }
        if (content.Contains("qq"))
        {
            var str = content.Substring(0, 4);
            var digits = str.Split('q');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[2]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            if (endPos != 1) endPos = MirrorKeys(endPos);
            return -(endPos + 18);
        }
        if (content.Contains('p'))
        {
            /* 
             * SlidePrefab 27 28 29 30 31 32 33 34
             * Star_Circle 1  2  3  4  5  6  7  8  
             */
            var str = content.Substring(0, 3);
            var digits = str.Split('p');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            return endPos + 26;
        }
        if (content.Contains('q'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('q');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            if (endPos != 1) endPos = MirrorKeys(endPos);
            return -(endPos + 26);
        }
        if (content.Contains('s'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('s');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            if(endPos!=5) throw new Exception("s星星尾部错误\nsスライドエラー");
            return 35;
        }
        if (content.Contains('z'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('z');
            int startPos = int.Parse(digits[0]);
            int endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            if (endPos != 5) throw new Exception("z星星尾部错误\nzスライドエラー");
            return -35;
        }
        if (content.Contains('V'))
        {
            /* 
             * SlidePrefab 37  38  39  40
             * Star_Circle 7-2 7-3 7-4 7-5
             */
            var str = content.Substring(0, 4);
            var digits = str.Split('V');
            int startPos = int.Parse(digits[0]);
            int turnPos = int.Parse(digits[1][0].ToString());
            int endPos = int.Parse(digits[1][1].ToString());
           
            turnPos = turnPos - startPos;
            turnPos = turnPos < 0 ? turnPos + 8 : turnPos;
            turnPos = turnPos > 8 ? turnPos - 8 : turnPos;
            turnPos++;
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            endPos++;
            if (turnPos == 7)
            {
                if (endPos < 2 || endPos > 5) throw new Exception("V星星终点不合法\nVスライドエラー");
                return endPos + 35;
            }
            if (turnPos == 3)
            {
                if (endPos < 5) throw new Exception("V星星终点不合法\nVスライドエラー");
                return -(MirrorKeys(endPos) + 35);
            }
            throw new Exception("V星星拐点只能隔开一键\nVスライドエラー");
        }
        return 0;
    }

    bool isUpperHalf(int key)
    {
        if (key == 7) return true;
        if (key == 8) return true;
        if (key == 1) return true;
        if (key == 2) return true;

        return false;
    }

    bool isRightHalf(int key)
    {
        if (key == 1) return true;
        if (key == 2) return true;
        if (key == 3) return true;
        if (key == 4) return true;

        return false;
    }

    int MirrorKeys(int key)
    {
        if (key == 1) return 1;
        if (key == 2) return 8;
        if (key == 3) return 7;
        if (key == 4) return 6;

        if (key == 5) return 5;
        if (key == 6) return 4;
        if (key == 7) return 3;
        if (key == 8) return 2;
        throw new System.Exception("Keys out of range: "+ key);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
