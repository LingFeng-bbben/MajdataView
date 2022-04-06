using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Majson
{
    public string level = "1";
    public string difficulty = "EZ";
    public int diffNum = 0;
    public string title = "default";
    public string artist = "default";
    public string designer = "default";
    public List<SimaiTimingPoint> timingList = new List<SimaiTimingPoint>();
}

class SimaiTimingPoint
{
    public double time;
    public bool havePlayed;
    public int rawTextPositionX;
    public int rawTextPositionY;
    public string noteContent;
    public float currentBpm;
    public List<SimaiNote> noteList = new List<SimaiNote>();
    public float HSpeed = 1.0f;
}
enum SimaiNoteType
{
    Tap, Slide, Hold, Touch, TouchHold
}
class SimaiNote
{
    public SimaiNoteType noteType;
    public bool isBreak = false;
    public bool isHanabi = false;
    public bool isEx = false;
    public bool isSlideNoHead = false;
    public bool isForceStar = false;
    public bool isFakeRotate = false;

    public int startPosition = 1; //键位（1-8）
    public char touchArea = ' ';

    public double holdTime = 0d;

    public double slideStartTime = 0d;
    public double slideTime = 0d;

    public string noteContent; //used for star explain
}

class EditRequestjson
{
    public EditorControlMethod control;
    public float startTime;
    public long startAt;
    public string jsonPath;
    public float playSpeed;
    public float backgroundCover;
    public float audioSpeed;
}

enum EditorControlMethod
{
    Start, Stop, OpStart, Pause, Continue
}
