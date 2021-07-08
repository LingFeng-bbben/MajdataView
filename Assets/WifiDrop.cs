using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiDrop : MonoBehaviour
{
    public GameObject star_slide1;
    public GameObject star_slide2;
    public GameObject star_slide3;

    SpriteRenderer spriteRenderer_star;

    public bool isMirror;

    public float time;
    public float timeStar;
    public float LastFor = 1f;
    public float speed;

    public int startPosition = 1;
    //TODO : Add 3 stars
}
