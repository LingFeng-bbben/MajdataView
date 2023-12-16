using UnityEngine;

public class Floating : MonoBehaviour
{
    public float rotate;
    public float wx;
    public float wy;
    private RectTransform rectTransform;

    private Vector3 startPos;

    // Start is called before the first frame update
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        rectTransform.Rotate(new Vector3(0f, 0f, rotate));
        rectTransform.localPosition = startPos + new Vector3(Mathf.Sin(Time.time * wx), Mathf.Sin(Time.time * wy));
    }
}