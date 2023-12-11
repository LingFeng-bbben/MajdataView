using System;
using UnityEngine;
using UnityEngine.UI;

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

    private EditorComboIndicator textMode = EditorComboIndicator.Combo;

    // Start is called before the first frame update
    private void Start()
    {
        table = GameObject.Find("ObjectCount").GetComponent<Text>();
        rate = GameObject.Find("ObjectRate").GetComponent<Text>();

        statusCombo = GameObject.Find("ComboText").GetComponent<Text>();
        statusScore = GameObject.Find("ScoreText").GetComponent<Text>();
        statusAchievement = GameObject.Find("AchievementText").GetComponent<Text>();
        statusDXScore = GameObject.Find("DXScoreText").GetComponent<Text>();

        statusCombo.gameObject.SetActive(false);
        statusScore.gameObject.SetActive(false);
        statusAchievement.gameObject.SetActive(false);
        statusDXScore.gameObject.SetActive(false);
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
        if (FiSumScore() == 0) return;
        UpdateSideOutput();
    }

    private void UpdateMainOutput()
    {
        var comboValue = tapCount + holdCount + slideCount + touchCount + breakCount;
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
                UpdateAchievementColor(accValues[0]);
                statusAchievement.text = string.Format("{0,6:0.00}%", Math.Truncate(accValues[0] * 100) / 100);
                break;
            case EditorComboIndicator.AchievementDownClassic: // Achievement (-) Classic (from 100%)
                UpdateAchievementColor(accValues[1]);
                statusAchievement.text = string.Format("{0,6:0.00}%", Math.Truncate(accValues[1] * 100) / 100);
                break;
            case EditorComboIndicator.AchievementDeluxe: // Achievement (+) Deluxe
                UpdateAchievementColor(accValues[2]);
                statusAchievement.text = string.Format("{0,8:0.0000}%", Math.Truncate(accValues[2] * 10000) / 10000);
                break;
            case EditorComboIndicator.AchievementDownDeluxe: // Achievement (-) Deluxe (from 100%)
                UpdateAchievementColor(accValues[3]);
                statusAchievement.text = string.Format("{0,8:0.0000}%", Math.Truncate(accValues[3] * 10000) / 10000);
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
                statusCombo.text = comboValue > 0 ? comboValue.ToString() : "";
                break;
        }
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
            "ALL: {10} / {11}",
            tapCount, holdCount, slideCount, touchCount, breakCount,
            tapSum, holdSum, slideSum, touchSum, breakSum,
            comboN,
            tapSum + holdSum + slideSum + touchSum + breakSum
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

    private void UpdateAchievementColor(float achievementRate)
    {
        var newColor = AchievementDudColor;
        if (achievementRate >= 100f)
            newColor = AchievementGoldColor;
        else if (achievementRate >= 97f)
            newColor = AchievementSilverColor;
        else if (achievementRate >= 80f)
            newColor = AchievementBronzeColor;

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