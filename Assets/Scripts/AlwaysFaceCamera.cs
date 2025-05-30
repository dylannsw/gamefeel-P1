using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.forward = transform.position - Camera.main.transform.position;
    }
}
