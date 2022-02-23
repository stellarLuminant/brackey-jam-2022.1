using UnityEngine;

public class TestEnemyFollow : MonoBehaviour
{
    // Measured in pixels per second.
    public float MoveSpeed = 100f;

    // Multiply to MoveSpeed to get per-second acceleration.
    public float AccelFactor = 1.4f;

    // Distance where following will stop.
    public float MinDistance = 30f;
    
    private Transform _target;
    private Rigidbody2D _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindWithTag("Player");

        if (player == null) {
            Debug.LogError("TestEnemyFollow.Start(): could not find object tagged \"Player\" to target");
            _target = transform;
        } else {
            _target = player.transform;
        }

        _rigidbody = GetComponent<Rigidbody2D>();

        if (_rigidbody == null) {
            Debug.LogError("TestEnemyFollow.Start(): could not find Rigidbody2D component");
        }
    }

    // FixedUpdate is called once per physics update.
    void FixedUpdate()
    {
        var displacement = Utils.To2D(_target.position) - _rigidbody.position;

        if (displacement.sqrMagnitude < MinDistance * MinDistance)
            return;
        
        var direction = displacement.normalized;
        var acceleration = MoveSpeed * AccelFactor;

        var velocity = _rigidbody.velocity + (direction * acceleration * Time.fixedDeltaTime);

        _rigidbody.velocity = Utils.NormalizeAtMagnitude(velocity, MoveSpeed);
    }

    // Update is called once per frame
    void Update() { }
}
