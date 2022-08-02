using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollide : MonoBehaviour
{
    AudioTimeProvider timeProvider;
    StarLogger logger;
    SlideEffectManager slideEffect;
    // Start is called before the first frame update
    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        logger = GameObject.Find("sensor").GetComponent<StarLogger>();
        slideEffect = GameObject.Find("SlideEffects").GetComponent<SlideEffectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log(name + ",Enter," + timeProvider.AudioTime);
        //logger.Log(name);
        //simulateHandler.operateArea(name, true, collider.name);
        //Debug.Log(collider.name);
        //Debug.Log(name);
        slideEffect.PlayEffect(name);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        //Debug.Log(name + ",Exit," + timeProvider.AudioTime);
        //simulateHandler.operateArea(name, false, collider.name);
    }

}
