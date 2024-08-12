using System.Collections.Generic;

internal class Majson
{
    public string artist = "default";
    public string designer = "default";
    public string difficulty = "EZ";
    public int diffNum = 0;
    public string level = "1";
    public List<SimaiTimingPoint> timingList = new();
    public string title = "default";
}

internal class SimaiTimingPoint
{
    public float currentBpm;
    public bool havePlayed;
    public float HSpeed = 1.0f;
    public string noteContent;
    public List<SimaiNote> noteList = new();
    public int rawTextPositionX;
    public int rawTextPositionY;
    public double time;
}

internal enum SimaiNoteType
{
    Tap,
    Slide,
    Hold,
    Touch,
    TouchHold
}

internal class SimaiNote
{
    public double holdTime = 0d;
    public bool isBreak = false;
    public bool isEx = false;
    public bool isFakeRotate = false;
    public bool isForceStar = false;
    public bool isHanabi = false;
    public bool isSlideBreak = false;
    public bool isSlideNoHead = false;

    public string noteContent; //used for star explain
    public SimaiNoteType noteType;

    public double slideStartTime = 0d;
    public double slideTime = 0d;

    public int startPosition = 1; //键位（1-8）
    public char touchArea = ' ';
}

internal class EditRequestjson
{
    public float audioSpeed;
    public float backgroundCover;
    public EditorComboIndicator comboStatusType;
    public EditorPlayMethod editorPlayMethod;
    public EditorControlMethod control;
    public string jsonPath;
    public float noteSpeed;
    public long startAt;
    public float startTime;
    public float touchSpeed;
    public bool smoothSlideAnime;
}

public enum EditorComboIndicator
{
    None,

    // List of viable indicators that won't be a static content.
    // ScoreBorder, AchievementMaxDown, ScoreDownDeluxe are static.
    Combo,
    ScoreClassic,
    AchievementClassic,
    AchievementDownClassic,
    AchievementDeluxe = 11,
    AchievementDownDeluxe,
    ScoreDeluxe,

    // Please prefix custom indicator with C
    CScoreDedeluxe = 101,
    CScoreDownDedeluxe,
    MAX
}

internal enum EditorControlMethod
{
    Start,
    Stop,
    OpStart,
    Pause,
    Continue,
    Record
}

public enum EditorPlayMethod
{
    Classic, DJAuto, Random, Disabled
}