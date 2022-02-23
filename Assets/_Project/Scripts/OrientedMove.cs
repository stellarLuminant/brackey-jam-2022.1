using System;
using UnityEngine;

public class OrientedMove : MonoBehaviour
{
    [Header("State")]
    // The current pixels per second of the object.
    public Single Speed;
    
    // The current seconds left before self destruction.
    public Single Timer;

    private Rigidbody2D Rigidbody;

    [Header("Parameters")]
    // Speed in pixels per second when the object is first initialized.
    public Single InitialSpeed = 150;

    // Acceleration in pixels per second^2.
    public Single Acceleration = -300;

    // Lifetime in seconds.
    public Single Duration;

    // Start is called before the first frame update
    void Start()
    {
        Speed = InitialSpeed;
        Timer = Duration;
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Timer -= Time.fixedDeltaTime;
        if (Timer <= 0)
        {
            Destroy(gameObject);
            return;
        }

        Speed += Acceleration * Time.fixedDeltaTime;
        var direction = Utils.DegToNormal(transform.localRotation.eulerAngles.z); 
        var motion = direction * Speed * Time.fixedDeltaTime;
        Rigidbody.MovePosition(Rigidbody.position + motion);
    }

    // Update is called once per frame
    void Update() { }
}
