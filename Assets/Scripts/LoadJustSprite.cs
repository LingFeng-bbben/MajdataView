using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadJustSprite : MonoBehaviour
{
    public int _0curv1str2wifi;
    public int indexOffset;
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Outline").GetComponent<CustomSkin>().Just[_0curv1str2wifi + 3];
        //setR();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int setR()
    {
        indexOffset = 0;
        refreshSprite();
        return _0curv1str2wifi;
    }

    public int setL()
    {
        indexOffset = 3;
        refreshSprite();
        return _0curv1str2wifi;
    }

    void refreshSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Outline").GetComponent<CustomSkin>().Just[_0curv1str2wifi + indexOffset];
    }
}
