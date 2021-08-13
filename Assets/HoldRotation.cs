using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldRotation : MonoBehaviour
{
    Quaternion rotation;
    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        print(transform.rotation);
        transform.rotation = rotation;
    }
}
