using Assets.Scripts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static NoteEffectManager;

public class ObjectCounter : MonoBehaviour
{
    public Color AchievementDudColor; // = new Color32(63, 127, 176, 255);
    public Color AchievementBronzeColor; // = new Color32(127, 48, 32, 255);
    public Color AchievementSilverColor; // = new Color32(160, 160, 160, 255);
    public Color AchievementGoldColor; // = new Color32(224, 191, 127, 255);

    public int tapCount;
    public int holdCount;
    public int slideCount;
    public int touchCount;
    public int breakCount;

    public int tapSum;
    public int holdSum;
    public int slideSum;
    public int touchSum;
    public int breakSum;
    private Text rate;
    private Text statusAchievement;

    private Text statusCombo;
    private Text statusDXScore;
    private Text statusScore;
    private Text table;
    private Text judgeResultCount;

    private EditorComboIndicator textMode = EditorComboIndicator.Combo;

    InputManager inputManager;
    NoteManager notes;

    double[] accRate = new double[5]
    {
        0.00,    // classic acc (+)
        100.00,  // classic acc (-)
        101.0000,// acc 101(-)
        100.0000,// acc 100(-)
        0.0000,  // acc (+)
    };

    long cPerfectCount = 0;
    long perfectCount = 0;
    long greatCount = 0;
    long goodCount = 0;
    long missCount = 0;
    long combo = 0;
    Dictionary<JudgeType, int> judgedTapCount;
    Dictionary<JudgeType, int> judgedHoldCount;
    Dictionary<JudgeType, int> judgedTouchCount;
    Dictionary<JudgeType, int> judgedTouchHoldCount;
    Dictionary<JudgeType, int> judgedSlideCount;
    Dictionary<JudgeType, int> judgedBreakCount;
    Dictionary<JudgeType, int> totalJudgedCount;

    // Start is called before the first frame update
    private void Start()
    {
        notes = GameObject.Find("Notes").GetComponent<NoteManager>();
        judgeResultCount = GameObject.Find("JudgeResultCount").GetComponent<Text>();
        table = GameObject.Find("ObjectCount").GetComponent<Text>();
        rate = GameObject.Find("ObjectRate").GetComponent<Text>();
        inputManager = GameObject.Find("Input").GetComponent<InputManager>();

        statusCombo = GameObject.Find("ComboText").GetComponent<Text>();
        statusScore = GameObject.Find("ScoreText").GetComponent<Text>();
        statusAchievement = GameObject.Find("AchievementText").GetComponent<Text>();
        statusDXScore = GameObject.Find("DXScoreText").GetComponent<Text>();

        statusCombo.gameObject.SetActive(false);
        statusScore.gameObject.SetActive(false);
        statusAchievement.gameObject.SetActive(false);
        statusDXScore.gameObject.SetActive(false);

        judgedTapCount = new()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };
        judgedHoldCount = new()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };
        judgedTouchCount = new()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };
        judgedTouchHoldCount = new()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };
        judgedSlideCount = new()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };
        judgedBreakCount = new()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };
        totalJudgedCount = new()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateState();
        UpdateOutput();
    }

    private void UpdateOutput()
    {
        UpdateMainOutput();
        UpdateJudgeResult();
        if (FiSumScore() == 0) return;
        UpdateSideOutput();
    }
    NoteScore GetNoteScoreSum()
    {
        Dictionary<JudgeType, int> collection = null;
        long score = 0;
        long lostScore = 0;
        long extraScore = 0;
        long extraScoreClassic = 0;
        long lostExtraScore = 0;
        long lostExtraScoreClassic = 0;
        int baseScore = 500;

        foreach(var type in new SimaiNoteType[] { SimaiNoteType.Tap, SimaiNoteType.Slide, SimaiNoteType.Hold, SimaiNoteType.Touch })
        {
            switch (type)
            {
                case SimaiNoteType.Tap:
                    collection = judgedTapCount;
                    baseScore = 500;
                    break;
                case SimaiNoteType.Slide:
                    collection = judgedSlideCount;
                    baseScore = 1500;
                    break;
                case SimaiNoteType.TouchHold:
                case SimaiNoteType.Hold:
                    collection = judgedHoldCount;
                    baseScore = 1000;
                    break;
                case SimaiNoteType.Touch:
                    collection = judgedTouchCount;
                    baseScore = 500;
                    break;
            }

            foreach (var judgeResult in collection)
            {
                var count = judgeResult.Value;
                switch (judgeResult.Key)
                {
                    case JudgeType.LatePerfect2:
                    case JudgeType.LatePerfect1:
                    case JudgeType.Perfect:
                    case JudgeType.FastPerfect1:
                    case JudgeType.FastPerfect2:
                        score += baseScore * 1 * count;
                        break;
                    case JudgeType.LateGreat2:
                    case JudgeType.LateGreat1:
                    case JudgeType.LateGreat:
                    case JudgeType.FastGreat:
                    case JudgeType.FastGreat1:
                    case JudgeType.FastGreat2:
                        score += (long)(baseScore * 0.8) * count;
                        lostScore += (long)(baseScore * 0.2) * count;
                        break;
                    case JudgeType.LateGood:
                    case JudgeType.FastGood:
                        score += (long)(baseScore * 0.5) * count;
                        lostScore += (long)(baseScore * 0.5) * count;
                        break;
                    case JudgeType.Miss:
                        lostScore += baseScore * count;
                        break;
                }
            }
        }
        foreach (var judgeResult in judgedBreakCount)
        {
            var count = judgeResult.Value;
            switch (judgeResult.Key)
            {
                case JudgeType.Perfect:
                    score += 2500 * count;
                    extraScore += 100 * count;
                    extraScoreClassic += 100 * count;
                    break;
                case JudgeType.LatePerfect1:  
                case JudgeType.FastPerfect1:
                    score += 2500 * count;
                    extraScore += 75 * count;
                    extraScoreClassic += 50 * count;
                    lostExtraScore += 25 * count;
                    lostExtraScoreClassic += 50 * count;
                    break;
                case JudgeType.LatePerfect2:
                case JudgeType.FastPerfect2:
                    score += 2500 * count;
                    extraScore += 50 * count;
                    extraScoreClassic += 0 * count;
                    lostExtraScore += 50 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGreat:
                case JudgeType.FastGreat:
                    score += 2000 * count;
                    extraScore += 40 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 500 * count;
                    lostExtraScore += 60 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGreat1:
                case JudgeType.FastGreat1:
                    score += 1500 * count;
                    extraScore += 40 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 1000 * count;
                    lostExtraScore += 60 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGreat2:
                case JudgeType.FastGreat2:
                    score += 1250 * count;
                    extraScore += 40 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 1250 * count;
                    lostExtraScore += 60 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGood:
                case JudgeType.FastGood:
                    score += 1000 * count;
                    extraScore += 30 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 1500 * count;
                    lostExtraScore += 70 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.Miss:
                    score += 0 * count;
                    extraScore += 0 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 2500 * count;
                    lostExtraScore += 100 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
            }
        }
        return new NoteScore()
        {
            TotalScore = score,
            TotalExtraScore = extraScore,
            TotalExtraScoreClassic = extraScoreClassic,
            LostScore = lostScore,
            LostExtraScore = lostExtraScore,
            LostExtraScoreClassic = lostExtraScoreClassic
        };
    }
    void CalAccRate()
    {
        long totalScore = 0;
        long totalExtraScore = 0;

        var currentNoteScore = GetNoteScoreSum();

        totalScore = (tapSum + touchSum) * 500 + holdSum * 1000 + slideSum * 1500 + breakSum * 2500;
        totalExtraScore = breakSum * 100;

        accRate[0] = ((currentNoteScore.TotalScore + currentNoteScore.TotalExtraScoreClassic) / (double)totalScore) * 100;
        accRate[1] = ((totalScore + currentNoteScore.TotalExtraScoreClassic - currentNoteScore.LostScore) / (double)totalScore) * 100;
        accRate[2] = ((totalScore - currentNoteScore.LostScore) / (double)totalScore) * 100 + ((totalExtraScore - currentNoteScore.LostExtraScore) / (double)totalExtraScore);
        accRate[3] = ((totalScore - currentNoteScore.LostScore) / (double)totalScore) * 100 + (currentNoteScore.TotalExtraScore / (double)totalExtraScore);
        accRate[4] = (currentNoteScore.TotalScore / (double)totalScore) * 100 + (currentNoteScore.TotalExtraScore / (double)totalExtraScore);
    }
    internal void ReportResult(NoteDrop note, JudgeType result,bool isBreak = false)
    {
        var noteType = GetNoteType(note);
        switch(noteType)
        {
            case SimaiNoteType.Tap:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedTapCount[result]++;
                    tapCount++;
                }
                break;
            case SimaiNoteType.Slide:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedSlideCount[result]++;
                    slideCount++;
                }
                break;
            case SimaiNoteType.Hold:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedHoldCount[result]++;
                    holdCount++;
                }
                break;
            case SimaiNoteType.Touch:
                judgedTouchCount[result]++; 
                touchCount++;
                break;
            case SimaiNoteType.TouchHold:
                judgedTouchHoldCount[result]++;
                holdCount++;
                break;

        }
        totalJudgedCount[result]++;
        if(result != 0)
            combo++;
        switch (result)
        {
            case JudgeType.Miss:
                missCount++;
                combo = 0;
                break;
            case JudgeType.Perfect:
                cPerfectCount++; 
                break;
            case JudgeType.LatePerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
            case JudgeType.FastPerfect2:
                perfectCount++;
                break;
            case JudgeType.LateGreat2:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat:
            case JudgeType.FastGreat:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat2:
                greatCount++;
                break;
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                goodCount++;
                break;
        }
        CalAccRate();
    }
    internal void NextNote(int pos) => notes.noteIndex[pos]++;
    internal void NextTouch(SensorType pos) => notes.touchIndex[pos]++;
    SimaiNoteType GetNoteType(NoteDrop note) => note switch
    {
        TapDrop => SimaiNoteType.Tap,
        StarDrop => SimaiNoteType.Tap,
        HoldDrop => SimaiNoteType.Hold,
        SlideDrop => SimaiNoteType.Slide,
        WifiDrop => SimaiNoteType.Slide,
        TouchHoldDrop => SimaiNoteType.TouchHold,
        TouchDrop => SimaiNoteType.Touch,
        _ => throw new InvalidOperationException()
    };
    private void UpdateMainOutput()
    {
        //var comboValue = tapCount + holdCount + slideCount + touchCount + breakCount;
        var scoreSSSValue = FiSumScore();
        int[] scoreValues =
        {
            FiNowScore(), DeDxNowScore(), DeDxNowBreakScore()
        };
        float[] accValues =
        {
            scoreSSSValue > 0 ? (float)FiNowScore() / scoreSSSValue * 100 : 0,
            scoreSSSValue > 0 ? (float)FiNowBreakScore() / scoreSSSValue * 100 : 0,
            scoreSSSValue > 0 ? (float)DxNowScore() / DxSumScore() * 100 + BreakRate() : 0,
            100f + BreakRate()
        };

        switch (textMode)
        {
            case EditorComboIndicator.ScoreClassic: // Score (+) Classic
                statusScore.text = string.Format("{0:#,##0}", scoreValues[0]);
                break;
            case EditorComboIndicator.AchievementClassic: // Achievement (+) Classic
                UpdateAchievementColor(accRate[0]);
                //statusAchievement.text = string.Format("{0,6:0.00}%", Math.Truncate(accValues[0] * 100) / 100);
                statusAchievement.text = string.Format("{0,6:0.00}%", accRate[0]);
                break;
            case EditorComboIndicator.AchievementDownClassic: // Achievement (-) Classic (from 100%)
                UpdateAchievementColor(accRate[1]);
                //statusAchievement.text = string.Format("{0,6:0.00}%", Math.Truncate(accValues[1] * 100) / 100);
                statusAchievement.text = string.Format("{0,6:0.00}%", accRate[1]);
                break;
            case EditorComboIndicator.AchievementDeluxe: // Achievement (+) Deluxe
                UpdateAchievementColor(accRate[4]);
                //statusAchievement.text = string.Format("{0,8:0.0000}%", Math.Truncate(accValues[2] * 10000) / 10000);
                statusAchievement.text = string.Format("{0,8:0.0000}%", accRate[4]);
                break;
            case EditorComboIndicator.AchievementDownDeluxe: // Achievement (-) Deluxe (from 100%)
                UpdateAchievementColor(accRate[3]);
                //statusAchievement.text = string.Format("{0,8:0.0000}%", Math.Truncate(accValues[3] * 10000) / 10000);
                statusAchievement.text = string.Format("{0,8:0.0000}%", accRate[3]);
                break;
            case EditorComboIndicator.ScoreDeluxe: // DX Score (+)
                statusDXScore.text = DxExNowScore().ToString();
                break;
            case EditorComboIndicator.CScoreDedeluxe: // Score (+) DeDX
                statusScore.text = string.Format("{0:#,##0}", scoreValues[1]);
                break;
            case EditorComboIndicator.CScoreDownDedeluxe: // Score (-) DeDX (from 100% rate)
                statusScore.text = string.Format("{0:#,##0}", scoreValues[2]);
                break;
            case EditorComboIndicator.Combo:
            default:
                statusCombo.text = combo > 0 ? combo.ToString() : "";
                break;
        }
    }
    void UpdateJudgeResult()
    {
        var fast = totalJudgedCount.Where(x => x.Key > JudgeType.Perfect && x.Key != JudgeType.Miss)
                                   .Select(x => x.Value)
                                   .Sum();
        var late = totalJudgedCount.Where(x => x.Key < JudgeType.Perfect && x.Key != JudgeType.Miss)
                                   .Select(x => x.Value)
                                   .Sum();
        judgeResultCount.text = $"{cPerfectCount}\n{perfectCount}\n{greatCount}\n{goodCount}\n{missCount}\n\n{fast}\n{late}";
    }

    private void UpdateSideOutput()
    {
        var comboN = tapCount + holdCount + slideCount + touchCount + breakCount;

        table.text = string.Format(
            "TAP: {0} / {5}\n" +
            "HOD: {1} / {6}\n" +
            "SLD: {2} / {7}\n" +
            "TOH: {3} / {8}\n" +
            "BRK: {4} / {9}\n" +
            "ALL: {10} / {11}\n" +
            "MOD: {12}",
            tapCount, holdCount, slideCount, touchCount, breakCount,
            tapSum, holdSum, slideSum, touchSum, breakSum,
            comboN,
            tapSum + holdSum + slideSum + touchSum + breakSum,
            InputManager.Mode
        );

        rate.text = string.Format(
            "FiNALE  Rate:\n" +
            "{0:000.00}   %\n" +
            "DELUXE Rate:\n" +
            "{1:000.0000} % ",
            Math.Truncate((float)FiNowScore() / FiSumScore() * 10000) / 100,
            Math.Truncate(((float)DxNowScore() / DxSumScore() * 100 + BreakRate()) * 10000) / 10000
        );
    }

    private void UpdateState()
    {
// Only define this when debugging (of this feature) is needed.
// I don't bother compiling this as Debug.
#if COMBO_CAN_SWAP_NOW
        if (Input.GetKeyDown(KeyCode.Space)) {
            var validModes = Enum.GetValues(textMode.GetType());
            int i = 0;
            foreach(EditorComboIndicator compareMode in validModes) {
                if (compareMode == textMode) {
                    ComboSetActive((EditorComboIndicator)validModes.GetValue((i + 1) % (validModes.Length - 1)));
                    break;
                }
                i += 1;
            }
        }
#endif
    }

    private void UpdateAchievementColor(double achievementRate)
    {
        var newColor = achievementRate switch
        {
            >= 100 => AchievementGoldColor,
            >= 97f => AchievementSilverColor,
            >= 80f => AchievementBronzeColor,
            _ => AchievementDudColor
        };

        var textElements = statusAchievement.gameObject.GetComponentsInChildren<Text>();

        foreach (var celm in textElements)
            if (celm.color != newColor)
                celm.color = newColor;
    }

    public void ComboSetActive(bool isActive)
    {
        ComboSetActive((EditorComboIndicator)(isActive ? 1 : 0));
    }

    public void ComboSetActive(EditorComboIndicator newComboMode)
    {
        textMode = newComboMode;
        var isActive = textMode > 0;
        var isAccClassic = textMode == EditorComboIndicator.AchievementClassic ||
                           textMode == EditorComboIndicator.AchievementDownClassic;
        var isPtsClassic = textMode == EditorComboIndicator.ScoreClassic;
        var isAccDeluxe = textMode == EditorComboIndicator.AchievementDeluxe ||
                          textMode == EditorComboIndicator.AchievementDownDeluxe;
        var isPtsDeluxe = textMode == EditorComboIndicator.ScoreDeluxe;
        var isPtsNormDeluxe = textMode == EditorComboIndicator.CScoreDedeluxe ||
                              textMode == EditorComboIndicator.CScoreDownDedeluxe;
        var isDefault = !(
            isAccClassic || isPtsClassic ||
            isAccDeluxe || isPtsDeluxe ||

            // De-DXfied 
            isPtsNormDeluxe ||
            false
        );

        statusCombo.gameObject.SetActive(isActive && isDefault);
        statusScore.gameObject.SetActive(isActive && (isPtsClassic || isPtsNormDeluxe));
        statusAchievement.gameObject.SetActive(isActive && (isAccClassic || isAccDeluxe));
        statusDXScore.gameObject.SetActive(isActive && isPtsDeluxe);
    }

    private int FiSumScore()
    {
        return tapSum * 500 + holdSum * 1000 + slideSum * 1500 + touchSum * 500 + breakSum * 2500;
    }

    private int FiNowScore()
    {
        return tapCount * 500 + holdCount * 1000 + slideCount * 1500 + touchCount * 500 + breakCount * 2600;
    }

    private int FiNowBreakScore()
    {
        return tapSum * 500 + holdSum * 1000 + slideSum * 1500 + touchSum * 500 + breakSum * 2500 + breakCount * 100;
    }

    private int DxSumScore()
    {
        return tapSum * 1 + holdSum * 2 + slideSum * 3 + touchSum * 1 + breakSum * 5;
    }

    private int DxNowScore()
    {
        return tapCount * 1 + holdCount * 2 + slideCount * 3 + touchCount * 1 + breakCount * 5;
    }

    private int DxExSumScore()
    {
        return (tapSum + holdSum + slideSum + touchSum + breakSum) * 3;
    }

    private int DxExNowScore()
    {
        return (tapCount + holdCount + slideCount + touchCount + breakCount) * 3;
    }

    private int DeDxNowScore()
    {
        return (int)Math.Round(FiSumScore() * ((float)DxNowScore() / DxSumScore() + BreakRate() / 100f) / 5) * 5;
    }

    private int DeDxNowBreakScore()
    {
        return (int)Math.Round(FiSumScore() * (1f + BreakRate() / 100f) / 5) * 5;
    }

    private float BreakRate()
    {
        return breakSum > 0 ? (float)breakCount / breakSum : 0f;
    }
}