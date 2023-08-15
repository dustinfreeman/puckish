using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleThrobber : MonoBehaviour
{
    float ThrobDuration = 3.0f;
    float ThrobFactor = 0.1f; 

    void Update()
    {
      float throbPhase = Time.time * ThrobDuration;
      transform.localScale = Vector3.one * ( Mathf.Cos(throbPhase) * ThrobFactor + 1);
    }
}
