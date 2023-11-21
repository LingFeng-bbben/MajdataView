using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCounter : MonoBehaviour
{
    Text table;
    Text rate;

    #if COUNTER_USE_TEXTMESHPRO
    TextMeshProUGUI statusCombo;
    TextMeshProUGUI statusScore;
    TextMeshProUGUI statusAchievement;
    TextMeshProUGUI statusDXScore;
    #else
    Text statusCombo;
    Text statusScore;
    Text statusAchievement;
    Text statusDXScore;
    #endif

    EditorComboIndicator textMode = EditorComboIndicator.Combo;

    public Color AchievementDudColor;// = new Color32(63, 127, 176, 255);
    public Color AchievementBronzeColor;// = new Color32(127, 48, 32, 255);
    public Color AchievementSilverColor;// = new Color32(160, 160, 160, 255);
    public Color AchievementGoldColor;// = new Color32(224, 191, 127, 255);

    // Public Accessible Stat
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

    // Start is called before the first frame update
    void Start()
    {
        table = GameObject.Find("ObjectCount").GetComponent<Text>();
        rate = GameObject.Find("ObjectRate").GetComponent<Text>();

        #if COUNTER_USE_TEXTMESHPRO
        statusCombo       = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        statusScore       = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        statusAchievement = GameObject.Find("AchievementText").GetComponent<TextMeshProUGUI>();
        statusDXScore     = GameObject.Find("DXScoreText").GetComponent<TextMeshProUGUI>();
        #else
        statusCombo       = GameObject.Find("ComboTextOriginal").GetComponent<Text>();
        statusScore       = GameObject.Find("ScoreTextOriginal").GetComponent<Text>();
        statusAchievement = GameObject.Find("AchievementTextOriginal").GetComponent<Text>();
        statusDXScore     = GameObject.Find("DXScoreTextOriginal").GetComponent<Text>();
        #endif

        statusCombo.gameObject.SetActive(false);
        statusScore.gameObject.SetActive(false);
        statusAchievement.gameObject.SetActive(false);
        statusDXScore.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        UpdateOutput();
    }

    void UpdateOutput()
    {
        UpdateMainOutput();
        if (FiSumScore() == 0) return;
        UpdateSideOutput();
    }

    void UpdateMainOutput()
    {
        int comboValue = tapCount + holdCount + slideCount + touchCount + breakCount;
        int scoreSSSValue = FiSumScore();
        int[] scoreValues = {
          FiNowScore(), DeDxNowScore(), DeDxNowBreakScore()
        };
        float[] accValues = {
          (scoreSSSValue > 0) ? ((float)FiNowScore() / scoreSSSValue) * 100 : 0,
          (scoreSSSValue > 0) ? ((float)FiNowBreakScore() / scoreSSSValue) * 100 : 0,
          (scoreSSSValue > 0) ? ((float)DxNowScore() / DxSumScore()) * 100 + BreakRate() : 0,
          100f + BreakRate()
        };

        Func<double, int, double> cutToDecimals = (value, decimalNum) => {
            double rate = Math.Pow(10, decimalNum);
            return Math.Truncate(value * rate) / rate;
        };

        float monowidth = 0.7f;

        switch(textMode) {
        case EditorComboIndicator.ScoreClassic: // Score (+) Classic
          ApplyMonospaceText(statusScore, string.Format("{0:#,##0}", scoreValues[0]), monowidth);
          break;
        case EditorComboIndicator.AchievementClassic: // Achievement (+) Classic
          UpdateAchievementColor(accValues[0]);
          ApplyMonospaceText(statusAchievement, string.Format("{0,6:0.00}%", cutToDecimals(accValues[0], 2)), monowidth);
          break;
        case EditorComboIndicator.AchievementDownClassic: // Achievement (-) Classic (from 100%)
          UpdateAchievementColor(accValues[1]);
          ApplyMonospaceText(statusAchievement, string.Format("{0,6:0.00}%", cutToDecimals(accValues[1], 2)), monowidth);
          break;
        case EditorComboIndicator.AchievementDeluxe: // Achievement (+) Deluxe
          UpdateAchievementColor(accValues[2]);
          ApplyMonospaceText(statusAchievement, string.Format("{0,8:0.0000}%", cutToDecimals(accValues[2], 4)), monowidth);
          break;
        case EditorComboIndicator.AchievementDownDeluxe: // Achievement (-) Deluxe (from 100%)
          UpdateAchievementColor(accValues[3]);
          ApplyMonospaceText(statusAchievement, string.Format("{0,8:0.0000}%", cutToDecimals(accValues[3], 4)), monowidth);
          break;
        case EditorComboIndicator.ScoreDeluxe: // DX Score (+)
          ApplyMonospaceText(statusDXScore, DxExNowScore().ToString(), monowidth);
          break;
        case EditorComboIndicator.CScoreDedeluxe: // Score (+) DeDX
          ApplyMonospaceText(statusScore, string.Format("{0:#,##0}", scoreValues[1]), monowidth);
          break;
        case EditorComboIndicator.CScoreDownDedeluxe: // Score (-) DeDX (from 100% rate)
          ApplyMonospaceText(statusScore, string.Format("{0:#,##0}", scoreValues[2]), monowidth);
          break;
        case EditorComboIndicator.Combo:
        default:
          ApplyMonospaceText(statusCombo, comboValue > 0 ? comboValue.ToString() : "", monowidth);
          break;
        }
    }

    void UpdateSideOutput()
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
            Math.Truncate(((float)FiNowScore() / FiSumScore()) * 10000)/100,
            Math.Truncate((((float)DxNowScore() / DxSumScore()) * 100 + BreakRate())*10000)/10000
            );
        
    }

    void UpdateState()
    {
        // Only define this when debugging (of this feature) is needed.
        // I don't bother compiling this as Debug.
        #if DEVELOPMENT_BUILD || COMBO_CAN_SWAP_NOW
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

    void ApplyMonospaceText(Text obj, string content, float width)
    {
        // Just a normal overload.
        obj.text = string.Format("{0}", content);
    }

    void ApplyMonospaceText(TextMeshProUGUI obj, string content, float width)
    {
        obj.text = string.Format("<{0}={1:0.00}em>{2}", "mspace", width, content);
    }

    private void UpdateAchievementColor(float achievementRate)
    {
        Color newColor = AchievementDudColor;
        if (achievementRate >= 100f)
            newColor = AchievementGoldColor;
        else if (achievementRate >= 97f)
            newColor = AchievementSilverColor;
        else if (achievementRate >= 80f)
            newColor = AchievementBronzeColor;

        TextMeshProUGUI[] tmpElms = statusAchievement.gameObject.GetComponentsInChildren<TextMeshProUGUI>();

        foreach(TextMeshProUGUI cElm in tmpElms) {
            if (cElm.color != newColor)
                cElm.color = newColor;
        }
 
        Text[] textElms = statusAchievement.gameObject.GetComponentsInChildren<Text>();

        foreach(Text cElm in textElms) {
            if (cElm.color != newColor)
                cElm.color = newColor;
        }
    }

    public void ComboSetActive(bool isActive)
    {
        ComboSetActive((EditorComboIndicator)(isActive ? 1 : 0));
    }

    public void ComboSetActive(EditorComboIndicator newComboMode)
    {
        textMode = newComboMode;
        bool isActive = textMode > 0;
        bool isAccClassic = textMode == EditorComboIndicator.AchievementClassic || textMode == EditorComboIndicator.AchievementDownClassic;
        bool isPtsClassic = textMode == EditorComboIndicator.ScoreClassic;
        bool isAccDeluxe = textMode == EditorComboIndicator.AchievementDeluxe || textMode == EditorComboIndicator.AchievementDownDeluxe;
        bool isPtsDeluxe = textMode == EditorComboIndicator.ScoreDeluxe;
        bool isPtsNormDeluxe = textMode == EditorComboIndicator.CScoreDedeluxe || textMode == EditorComboIndicator.CScoreDownDedeluxe;
        bool isDefault = !(
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

    int FiSumScore()
    {
        return tapSum * 500 + holdSum * 1000 + slideSum * 1500 + touchSum * 500 + breakSum * 2500;
    }

    int FiNowScore()
    {
        return tapCount * 500 + holdCount * 1000 + slideCount * 1500 + touchCount * 500 + breakCount * 2600;
    }

    int FiNowBreakScore()
    {
        return tapSum * 500 + holdSum * 1000 + slideSum * 1500 + touchSum * 500 + breakSum * 2500 + breakCount * 100;
    }

    int DxSumScore()
    {
        return tapSum * 1 + holdSum * 2 + slideSum * 3 + touchSum * 1 + breakSum * 5;
    }

    int DxNowScore()
    {
        return tapCount * 1 + holdCount * 2 + slideCount * 3 + touchCount * 1 + breakCount * 5;
    }

    int DxExSumScore()
    {
        return (tapSum + holdSum + slideSum + touchSum + breakSum) * 3;
    }

    int DxExNowScore()
    {
        return (tapCount + holdCount + slideCount + touchCount + breakCount) * 3;
    }

    int DeDxNowScore()
    {
        return (int)Math.Round(FiSumScore() * ((float)DxNowScore() / DxSumScore() + BreakRate() / 100f) / 5) * 5;
    }

    int DeDxNowBreakScore()
    {
        return (int)Math.Round(FiSumScore() * (1f + BreakRate() / 100f) / 5) * 5;
    }

    float BreakRate()
    {
        return breakSum > 0 ? (float)breakCount / breakSum : 0f;
    }
}
