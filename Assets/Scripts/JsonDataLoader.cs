using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class JsonDataLoader : MonoBehaviour
{
    public float speed = 10f;
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

    public Text diffText;
    public Text levelText;
    public Text titleText;
    public Text artistText;
    public Text designText;
    public RawImage cardImage;
    public Color[] diffColors = new Color[7];

    ObjectCount objectCount;
    CustomSkin customSkin;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        objectCount = GameObject.Find("ObjectCount").GetComponent<ObjectCount>();
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

                        if (timing.noteList.Count > 1) NDCompo.isEach = true;
                        NDCompo.isBreak = note.isBreak;
                        NDCompo.isEX = note.isEx;
                        NDCompo.time = (float)timing.time;
                        NDCompo.startPosition = note.startPosition;
                        NDCompo.speed = speed * timing.HSpeed;
                    }
                    if (note.noteType == SimaiNoteType.Hold)
                    {
                        var GOnote = Instantiate(holdPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<HoldDrop>();

                        NDCompo.tapSpr = customSkin.Hold;
                        NDCompo.eachSpr = customSkin.Hold_Each;
                        NDCompo.exSpr = customSkin.Hold_Ex;

                        if (timing.noteList.Count > 1) NDCompo.isEach = true;
                        NDCompo.time = (float)timing.time;
                        NDCompo.lastFor = (float)note.holdTime;
                        NDCompo.startPosition = note.startPosition;
                        NDCompo.speed = speed * timing.HSpeed;
                        NDCompo.isEX = note.isEx;
                    }
                    if (note.noteType == SimaiNoteType.TouchHold)
                    {
                        var GOnote = Instantiate(touchHoldPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<TouchHoldDrop>();
                        NDCompo.time = (float)timing.time;
                        NDCompo.lastFor = (float)note.holdTime;
                        NDCompo.speed = 0.2f * timing.HSpeed;
                        NDCompo.isFirework = note.isHanabi;
                    }
                    if (note.noteType == SimaiNoteType.Touch)
                    {
                        var GOnote = Instantiate(touchPrefab, notes.transform);
                        var NDCompo = GOnote.GetComponent<TouchDrop>();
                        NDCompo.time = (float)timing.time;
                        NDCompo.areaPosition = note.touchArea;
                        NDCompo.startPosition = note.startPosition;
                        if (timing.noteList.Count > 1) NDCompo.isEach = true;
                        NDCompo.speed = 0.2f * timing.HSpeed;
                        NDCompo.isFirework = note.isHanabi;
                    }
                    if (note.noteType == SimaiNoteType.Slide)
                    {

                        if (note.noteContent.Contains('w')) //wifi
                        {
                            InstantiateWifi(timing, note, lastNoteTime);
                        }
                        else
                        {
                            InstantiateStar(timing, note, i, lastNoteTime);
                        }

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
                    lineDrop.speed = speed * timing.HSpeed;

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
                    if (note.noteType == SimaiNoteType.Tap) objectCount.tapSum++;
                    if (note.noteType == SimaiNoteType.Hold) objectCount.holdSum++;
                    if (note.noteType == SimaiNoteType.TouchHold) objectCount.holdSum++;
                    if (note.noteType == SimaiNoteType.Touch) objectCount.touchSum++;
                    if (note.noteType == SimaiNoteType.Slide)
                    {
                        if (!note.isSlideNoHead) objectCount.tapSum++;
                        objectCount.slideSum++;
                    }
                }
                else
                {
                    if (note.noteType == SimaiNoteType.Slide)
                    {
                        if (!note.isSlideNoHead) objectCount.breakSum++;
                        objectCount.slideSum++;
                    }
                    else { objectCount.breakSum++; }
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
                if (note.noteType == SimaiNoteType.Tap) objectCount.tapCount++;
                if (note.noteType == SimaiNoteType.Hold) objectCount.holdCount++;
                if (note.noteType == SimaiNoteType.TouchHold) objectCount.holdCount++;
                if (note.noteType == SimaiNoteType.Touch) objectCount.touchCount++;
                if (note.noteType == SimaiNoteType.Slide)
                {
                    if (!note.isSlideNoHead) objectCount.tapCount++;
                    objectCount.slideCount++;
                }
            }
            else
            {
                if (note.noteType == SimaiNoteType.Slide)
                {
                    if (!note.isSlideNoHead) objectCount.breakCount++;
                    objectCount.slideCount++;
                }
                else { objectCount.breakCount++; }
            }
        }
    }

    void InstantiateWifi(SimaiTimingPoint timing,SimaiNote note,double lastNoteTime)
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

        NDCompo.rotateSpeed = (float)note.slideTime;
        NDCompo.isEX = note.isEx;
        NDCompo.isBreak = note.isBreak;

        var slideWifi = Instantiate(slidePrefab[36], notes.transform);
        slideWifi.SetActive(false);
        NDCompo.slide = slideWifi;
        var WifiCompo = slideWifi.GetComponent<WifiDrop>();

        WifiCompo.normalStar = customSkin.Star;
        WifiCompo.eachStar = customSkin.Star_Each;

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

        NDCompo.isNoHead = note.isSlideNoHead;
        NDCompo.time = (float)timing.time;
        NDCompo.startPosition = note.startPosition;
        NDCompo.speed = speed * timing.HSpeed;

        WifiCompo.isJustR = detectJustType(note.noteContent);
        WifiCompo.speed = speed * timing.HSpeed;
        WifiCompo.timeStart = (float)timing.time;
        WifiCompo.startPosition = note.startPosition;
        WifiCompo.time = (float)note.slideStartTime;
        WifiCompo.LastFor = (float)note.slideTime;
        WifiCompo.sortIndex = -7000 + (int)((lastNoteTime - timing.time) * -100);
    }

    void InstantiateStar(SimaiTimingPoint timing, SimaiNote note, int sort, double lastNoteTime)
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

        NDCompo.isNoHead = note.isSlideNoHead;
        NDCompo.time = (float)timing.time;
        NDCompo.startPosition = note.startPosition;
        NDCompo.speed = speed * timing.HSpeed;


        SliCompo.isMirror = isMirror;
        SliCompo.isJustR = detectJustType(note.noteContent);
        SliCompo.speed = speed * timing.HSpeed;
        SliCompo.timeStar = (float)timing.time;
        SliCompo.startPosition = note.startPosition;
        SliCompo.star_slide = slide_star;
        SliCompo.time = (float)note.slideStartTime;
        SliCompo.LastFor = (float)note.slideTime;
        SliCompo.sortIndex = -7000 + (int)((lastNoteTime - timing.time) * -100) + sort * 5;
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
