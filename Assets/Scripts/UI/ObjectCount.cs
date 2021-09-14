using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCount : MonoBehaviour
{
    Text text;
    Text rate;
    Text combo;
    GameObject comboObj;

    static bool isComboEnabled = false;

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
        text = GetComponent<Text>();
        rate = GameObject.Find("ObjectRate").GetComponent<Text>();
        comboObj = GameObject.Find("ComboText");
        combo = comboObj.GetComponent<Text>();
        comboObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (FiSumScore() == 0) return;
        comboObj.SetActive(isComboEnabled);
        var comboN = tapCount + holdCount + slideCount + touchCount + breakCount;
        combo.text = comboN.ToString();
        text.text = string.Format(
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
            ((float)FiNowScore() / FiSumScore()) * 100,
            ((float)DxNowScore() / DxSumScore()) * 100 + ((float)breakCount /breakSum)
            );
    }

    public void ToggleCombo()
    {
        if (FiSumScore() == 0) return;
        if (isComboEnabled)
        {
            comboObj.SetActive(false);
            isComboEnabled = false;
        }
        else
        {
            comboObj.SetActive(true);
            isComboEnabled = true;
        }
    }

    int FiSumScore()
    {
        return tapSum * 500 + holdSum * 1000 + slideSum * 1500 + touchSum * 500 + breakSum * 2500;
    }

    int FiNowScore()
    {
        return tapCount * 500 + holdCount * 1000 + slideCount * 1500 + touchCount * 500 + breakCount * 2600;
    }

    int DxSumScore()
    {
        return tapSum * 1 + holdSum * 2 + slideSum * 3 + touchSum * 1 + breakSum * 5;
    }

    int DxNowScore()
    {
        return tapCount * 1 + holdCount * 2 + slideCount * 3 + touchCount * 1 + breakCount * 5;
    }
}
