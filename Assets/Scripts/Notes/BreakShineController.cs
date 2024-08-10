using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
#nullable enable
public class BreakShineController : MonoBehaviour
{
    public IFlasher? parent = null;

    SpriteRenderer spriteRenderer;
    AudioTimeProvider timeProvider;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if(parent is not null && parent.CanShine())
        {
            var extra = Math.Max(Mathf.Sin(timeProvider.GetFrame() * 0.17f) * 0.5f, 0);
            spriteRenderer.material.SetFloat("_Brightness", 0.95f + extra);
        }
    }
    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
    }
}
