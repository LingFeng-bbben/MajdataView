using UnityEngine;

public class NoteEffectManager : MonoBehaviour
{
    public Sprite hex;
    public Sprite star;
    private readonly Animator[] judgeAnimators = new Animator[8];
    private readonly GameObject[] judgeEffects = new GameObject[8];
    private readonly Animator[] tapAnimators = new Animator[8];

    private readonly GameObject[] tapEffects = new GameObject[8];

    // Start is called before the first frame update
    private void Start()
    {
        var tapEffectParent = transform.GetChild(0).gameObject;
        var judgeEffectParent = transform.GetChild(1).gameObject;

        for (var i = 0; i < tapEffectParent.transform.childCount; i++)
        {
            tapEffects[i] = tapEffectParent.transform.GetChild(i).gameObject;
            tapAnimators[i] = tapEffects[i].GetComponent<Animator>();
            tapEffects[i].SetActive(false);
        }

        for (var i = 0; i < judgeEffectParent.transform.childCount; i++)
        {
            judgeEffects[i] = judgeEffectParent.transform.GetChild(i).gameObject;
            judgeAnimators[i] = judgeEffects[i].GetComponent<Animator>();
        }

        LoadSkin();
    }

    /// <summary>
    ///     加载判定文本的皮肤
    /// </summary>
    private void LoadSkin()
    {
        var customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();

        foreach (var judgeEffect in judgeEffects)
        {
            judgeEffect.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite =
                customSkin.JudgeText_Normal;
            judgeEffect.transform.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite =
                customSkin.JudgeText_Break;
        }
    }

    // Update is called once per frame
    public void PlayEffect(int position, bool isBreak)
    {
        var pos = position - 1;
        tapEffects[pos].SetActive(true);
        if (isBreak)
        {
            tapAnimators[pos].SetTrigger("break");
            judgeAnimators[pos].SetTrigger("break");
        }
        else
        {
            tapAnimators[pos].SetTrigger("tap");
            judgeAnimators[pos].SetTrigger("perfect");
        }
    }
}