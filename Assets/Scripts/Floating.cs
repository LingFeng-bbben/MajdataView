using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floating : MonoBehaviour
{
    public float rotate;
    public float wx;
    public float wy;
    Vector3 startPos;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.Rotate(new Vector3(0f, 0f, rotate));
        rectTransform.localPosition = startPos + new Vector3(Mathf.Sin(Time.realtimeSinceStartup * wx), Mathf.Sin(Time.realtimeSinceStartup * wy));
    }
}
