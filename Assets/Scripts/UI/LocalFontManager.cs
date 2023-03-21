using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class is to adjust global and other individual fonts
// without affecting the whole scene file.
//
// Something as a "delta-patch" to the scene.
public class LocalFontManager : MonoBehaviour
{
    [Tooltip("Base font to apply on.")]
    public TMP_FontAsset GlobalFont;
    
    public TMP_FontAsset AllPerfectFont;
    public TMP_FontAsset SongInfoFont;
    public TMP_FontAsset SideInfoFont;
    public TMP_FontAsset CenterInfoFont;

    // Start is called before the first frame update
    void Start()
    {
        SetAllPerfectFont();
        SetSideInfoFont();
        SetSongInfoFont();
        SetCenterInfoFont();
    }

    // Update is called once per frame
    void Update()
    {
        // No need to fill this in.
        // This is for init object only.
    }

    void SetAllPerfectFont()
    {
        ApplyFont(GameObject.Find("CanvasAllPerfect"), AllPerfectFont);
    }

    void SetSideInfoFont()
    {
        ApplyFont(GameObject.Find("CanvasInfo"), SideInfoFont);
    }

    void SetSongInfoFont()
    {
        ApplyFont(GameObject.Find("CanvasSongDetail"), SongInfoFont);
    }

    void SetCenterInfoFont()
    {
        ApplyFont(GameObject.Find("CanvasCombo"), CenterInfoFont);
    }

    private void ApplyFont(GameObject root, TMP_FontAsset font)
    {
        if (font == null)
            font = GlobalFont;
        if (font == null) return;

        var fontSource = ObtainFont(font);

        TextMeshProUGUI[] tmpElms = root.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI cElm in tmpElms) { cElm.font = font; }

        if (fontSource != null)
        {
            Text[] textElms = root.GetComponentsInChildren<Text>();
            foreach (Text cElm in textElms) { cElm.font = fontSource; }
        }
    }
    
    private Font ObtainFont(TMP_FontAsset fontAsset)
    {
        if (fontAsset.sourceFontFile != null)
            return fontAsset.sourceFontFile;

        return null; // I don't have a workaround.
    }
}