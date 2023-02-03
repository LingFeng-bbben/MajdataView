using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCounter : MonoBehaviour
{
    Text table;
    Text rate;
    Text combo;

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
        combo = GameObject.Find("ComboText").GetComponent<Text>();
        combo.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (FiSumScore() == 0) return;
        var comboN = tapCount + holdCount + slideCount + touchCount + breakCount;
        combo.text = comboN.ToString();
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
            ((float)FiNowScore() / FiSumScore()) * 100,
            ((float)DxNowScore() / DxSumScore()) * 100 + ((float)breakCount /breakSum)
            );
    }

    public void ComboSetActive(bool isActive)
    {
        combo.gameObject.SetActive(isActive);
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
