using UnityEngine;

public class NoteEffectManager : MonoBehaviour
{
    public enum JudgeType
    {
        Miss,
        LateGood,
        LateGreat2,
        LateGreat1,
        LateGreat,
        LatePerfect2,
        LatePerfect1,
        Perfect,
        FastPerfect1,
        FastPerfect2,
        FastGreat,
        FastGreat1,
        FastGreat2,
        FastGood
    }
    public Sprite hex;
    public Sprite star;
    private readonly Animator[] judgeAnimators = new Animator[8];
    private readonly GameObject[] judgeEffects = new GameObject[8];
    private readonly Animator[] tapAnimators = new Animator[8];
    private readonly Animator[] greatAnimators = new Animator[8];
    private readonly Animator[] goodAnimators = new Animator[8];

    private readonly GameObject[] tapEffects = new GameObject[8];
    private readonly GameObject[] greatEffects = new GameObject[8];
    private readonly GameObject[] goodEffects = new GameObject[8];

    private readonly Animator[] fastLateAnims = new Animator[8];
    private readonly GameObject[] fastLateEffects = new GameObject[8];
    Sprite[] judgeText;

    // Start is called before the first frame update
    private void Start()
    {
        var tapEffectParent = transform.GetChild(0).gameObject;
        var greatEffectParent = transform.GetChild(2).gameObject;
        var goodEffectParent = transform.GetChild(3).gameObject;
        var judgeEffectParent = transform.GetChild(1).gameObject;
        var flParent = transform.GetChild(4).gameObject;

        for (var i = 0; i < 8; i++)
        {
            judgeEffects[i] = judgeEffectParent.transform.GetChild(i).gameObject;
            judgeAnimators[i] = judgeEffects[i].GetComponent<Animator>();

            fastLateEffects[i] = flParent.transform.GetChild(i).gameObject;
            fastLateAnims[i] = fastLateEffects[i].GetComponent<Animator>();

            goodEffects[i] = goodEffectParent.transform.GetChild(i).gameObject;
            greatAnimators[i] = goodEffects[i].GetComponent<Animator>();
            goodEffects[i].SetActive(false);

            greatEffects[i] = greatEffectParent.transform.GetChild(i).gameObject;
            greatAnimators[i] = greatEffects[i].GetComponent<Animator>();
            greatEffects[i].SetActive(false);

            tapEffects[i] = tapEffectParent.transform.GetChild(i).gameObject;
            tapAnimators[i] = tapEffects[i].GetComponent<Animator>();
            tapEffects[i].SetActive(false);

        }

        LoadSkin();
    }

    /// <summary>
    ///     加载判定文本的皮肤
    /// </summary>
    private void LoadSkin()
    {
        var customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();
        judgeText = customSkin.JudgeText;

        foreach (var judgeEffect in judgeEffects)
        {
            judgeEffect.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite =
                customSkin.JudgeText[0];
            judgeEffect.transform.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite =
                customSkin.JudgeText_Break;
        }
    }

    // Update is called once per frame
    public void PlayEffect(int position, bool isBreak,JudgeType judge = JudgeType.Perfect)
    {
        var pos = position - 1;

        switch (judge)
        {
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                judgeEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[1];
                ResetEffect(position);
                if(isBreak)
                {
                    tapEffects[pos].SetActive(true);
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("bGood");
                }
                else
                    goodEffects[pos].SetActive(true);
                break;
            case JudgeType.LateGreat:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat2:
            case JudgeType.FastGreat2:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat:
                judgeEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[2];
                ResetEffect(position);
                if (isBreak)
                {
                    tapEffects[pos].SetActive(true);
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("bGreat");
                }
                else
                    greatEffects[pos].SetActive(true);
                break;
            case JudgeType.LatePerfect2:
            case JudgeType.FastPerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
                judgeEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[3];
                ResetEffect(position);
                tapEffects[pos].SetActive(true);
                if (isBreak)
                {
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("break");
                }
                break;
            case JudgeType.Perfect:
                judgeEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[4];
                ResetEffect(position);
                tapEffects[pos].SetActive(true);
                if (isBreak)
                {
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("break");
                }               
                break;
            default:
                judgeEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = judgeText[0];
                break;
        }

        if (isBreak && judge == JudgeType.Perfect)
            judgeAnimators[pos].SetTrigger("break");
        else
            judgeAnimators[pos].SetTrigger("perfect");
    }
    public void PlayFastLate(int position,JudgeType judge)
    {
        var customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();
        var pos = position - 1;
        if ((int)judge is (0 or 7))
        {
            fastLateEffects[pos].SetActive(false);
            return;
        }
        fastLateEffects[pos].SetActive(true);
        bool isFast = (int)judge > 7;
        if(isFast)
            fastLateEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = customSkin.FastText;
        else
            fastLateEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = customSkin.LateText;
        fastLateAnims[pos].SetTrigger("perfect");

    }
    public void ResetEffect(int position)
    {
        tapEffects[position - 1].SetActive(false);
        greatEffects[position - 1].SetActive(false);
        goodEffects[position - 1].SetActive(false);
    }
}