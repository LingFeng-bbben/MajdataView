using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class JsonDataLoader : MonoBehaviour
{
    public float speed = 10f;
    public Sprite slideEach;
    public Sprite starEach;
    public GameObject tapPrefab;
    public GameObject holdPrefab;
    public GameObject starPrefab;
    public GameObject notes;
    public GameObject star_slidePrefab;
    public GameObject[] slidePrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoadJson(string json)
    {
        
        MajLoadedData.timingList = JsonConvert.DeserializeObject<Majson>(json).timingList;
        foreach (var timing in MajLoadedData.timingList)
        {
            for (int i = 0; i < timing.noteList.Count; i++)
            {
                var note = timing.noteList[i];
                if (note.noteType == SimaiNoteType.Tap)
                {
                    var GOnote = Instantiate(tapPrefab, notes.transform);
                    var NDCompo = GOnote.GetComponent<TapDrop>();
                    if (timing.noteList.Count > 1) NDCompo.isEach = true;
                    if (note.isBreak) NDCompo.isBreak = true;
                    NDCompo.time = (float)timing.time;
                    NDCompo.startPosition = note.startPosition;
                    NDCompo.speed = speed;
                }
                if (note.noteType == SimaiNoteType.Hold)
                {
                    var GOnote = Instantiate(holdPrefab, notes.transform);
                    var NDCompo = GOnote.GetComponent<HoldDrop>();
                    if (timing.noteList.Count > 1) NDCompo.isEach = true;
                    NDCompo.time = (float)timing.time;
                    NDCompo.lastFor = (float)note.holdTime;
                    NDCompo.startPosition = note.startPosition;
                    NDCompo.speed = speed;
                }
                if (note.noteType == SimaiNoteType.Slide)
                {
                    var GOnote = Instantiate(starPrefab, notes.transform);
                    var NDCompo = GOnote.GetComponent<StarDrop>();

                    if (note.isBreak)
                    {
                        NDCompo.isBreak = true;
                        note.noteContent = note.noteContent.Replace("b", "");
                    }
                    if (note.noteContent.Contains('w')) //wifi
                    {
                        var slideWifi = Instantiate(slidePrefab[36], notes.transform);
                        var Wifi_star1 = Instantiate(star_slidePrefab, notes.transform);
                        var Wifi_star2 = Instantiate(star_slidePrefab, notes.transform);
                        var Wifi_star3 = Instantiate(star_slidePrefab, notes.transform);

                        Wifi_star1.SetActive(false);
                        Wifi_star2.SetActive(false);
                        Wifi_star3.SetActive(false);

                        slideWifi.SetActive(false);
                        NDCompo.slide = slideWifi;
                        var WifiCompo = slideWifi.AddComponent<WifiDrop>();


                        if (timing.noteList.Count > 1)
                        {
                            NDCompo.isEach = true;
                            NDCompo.isDouble = true;
                        }


                        NDCompo.time = (float)timing.time;
                        NDCompo.startPosition = note.startPosition;
                        NDCompo.speed = speed;

                        WifiCompo.speed = speed;
                        WifiCompo.timeStar = (float)timing.time;
                        WifiCompo.startPosition = note.startPosition;
                        WifiCompo.star_slide[0] = Wifi_star1;
                        WifiCompo.star_slide[1] = Wifi_star2;
                        WifiCompo.star_slide[2] = Wifi_star3;
                        WifiCompo.time = (float)note.slideStartTime;
                        WifiCompo.LastFor = (float)note.slideTime;
                        break;
                    }
                    var slideIndex = detectShapeFromText(note.noteContent);
                    bool isMirror = false;
                    if (slideIndex < 0) { isMirror = true; slideIndex = -slideIndex; }

                    var slide = Instantiate(slidePrefab[slideIndex], notes.transform);
                    var slide_star = Instantiate(star_slidePrefab, notes.transform);


                    slide_star.SetActive(false);
                    slide.SetActive(false);
                    NDCompo.slide = slide;
                    var SliCompo = slide.AddComponent<SlideDrop>();


                    if (timing.noteList.Count > 1)
                    {
                        NDCompo.isEach = true;

                        if (timing.noteList.FindAll(
                            o => o.noteType == SimaiNoteType.Slide &&
                            o.startPosition == note.startPosition).Count
                            > 1)
                        {
                            NDCompo.isDouble = true;
                        }
                        if (timing.noteList.FindAll(
                            o => o.noteType == SimaiNoteType.Slide).Count
                            > 1)
                        {
                            SliCompo.isEach = true;
                            slide_star.GetComponent<SpriteRenderer>().sprite = starEach;
                        }
                    }


                    NDCompo.time = (float)timing.time;
                    NDCompo.startPosition = note.startPosition;
                    NDCompo.speed = speed;

                    SliCompo.spriteEach = slideEach;
                    SliCompo.isMirror = isMirror;
                    SliCompo.speed = speed;
                    SliCompo.timeStar = (float)timing.time;
                    SliCompo.startPosition = note.startPosition;
                    SliCompo.star_slide = slide_star;
                    SliCompo.time = (float)note.slideStartTime;
                    SliCompo.LastFor = (float)note.slideTime;

                }
            }
        }
    }

    int detectShapeFromText(string content)
    {
        print(content);
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
            print("DEBUG"+endPos);
            return endPos - 2;
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
            throw new System.Exception("^ Error");
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
             * Star_Circle 1  2  3  4  5  6  7  8   希望开源以后有更多人来写
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
            return 35;
        }
        if (content.Contains('z'))
        {
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
                return endPos + 35;
            }
            if (turnPos == 3)
            {
                return -(MirrorKeys(endPos) + 35);
            }
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
