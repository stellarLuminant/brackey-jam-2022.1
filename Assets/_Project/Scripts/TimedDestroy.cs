using System;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [Header("State")]
    public Single Timer;

    [Header("Parameters")]
    public Single MinDuration;

    public Single MaxDuration;

    void Start()
    {
        Timer = UnityEngine.Random.Range(MinDuration, MaxDuration);
    }

    void Update() 
    { 
      Timer -= Time.deltaTime;
      if (Timer <= 0)
        Destroy(gameObject);
    }
}
