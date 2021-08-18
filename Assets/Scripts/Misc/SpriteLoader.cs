using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class SpriteLoader 
{
    public static Sprite LoadSpriteFromFile(string path)
    {
        UnityWebRequest req = UnityWebRequest.Get("file://" + path);
        req.SendWebRequest();
        System.Drawing.Image image = System.Drawing.Image.FromFile(path);
        Texture2D texture = new Texture2D(image.Width, image.Height);
        texture.LoadImage(req.downloadHandler.data);
        return Sprite.Create(texture, new Rect(0, 0, image.Width, image.Height), new Vector2(0.5f, 0.5f));
    }
    public static Sprite LoadSpriteFromFile(string path,Vector4 border)
    {
        UnityWebRequest req = UnityWebRequest.Get("file://" + path);
        req.SendWebRequest();
        System.Drawing.Image image = System.Drawing.Image.FromFile(path);
        Texture2D texture = new Texture2D(image.Width, image.Height);
        texture.LoadImage(req.downloadHandler.data);
        return Sprite.Create(texture, new Rect(0, 0, image.Width, image.Height), new Vector2(0.5f, 0.5f),100,1,SpriteMeshType.FullRect,border);

    }
}
