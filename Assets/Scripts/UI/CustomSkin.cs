using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CustomSkin : MonoBehaviour
{
    SpriteRenderer Outline;
    // Start is called before the first frame update
    void Start()
    {
        var path = new DirectoryInfo(Application.dataPath).Parent.FullName + "/Skin/";
        Outline = gameObject.GetComponent<SpriteRenderer>();
        print(path);
        Outline.sprite = SpriteLoader.LoadSpriteFromFile(path + "/outline.png");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
