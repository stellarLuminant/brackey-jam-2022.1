using System;
using UnityEngine;

public class BehaviorImp : MonoBehaviour
{
    // Two states:
    //      Idle:       Wanders within a radius of a fixed point. No attack present/despawns currently attached attack.
    //                  Detects player within detection radius and switches to Disturbed state.
    //      Disturbed:  Follows player using TestEnemyFollow mechanics. Attack is anchored to enemy as "contact damage".
    //                  If attack is despawned, return to Idle state.

    [Header("State")]
    public Boolean IsIdle;

    // The anchor that the imp idles around.
    public Vector2 IdleAnchor;

    // The time when the imp can be disturbed.
    public Single CanBeDisturbedTime;

    // Clock time when the next "reposition" will occur.
    public Single NextIdleWanderTime;

    // Clock time when the last "reposition" occurred.
    public Single LastIdleWanderTime;

    // The current position where the imp is walking to in idle state.
    public Vector2 NextIdleTarget;

    // The last idle target position.
    public Vector2 LastIdleTarget;

    private Transform _playerTarget;

    private Rigidbody2D _rigidbody;

    private Animator _animator;

    private GameObject _contactAttackObject;

    [Header("Parameters")]
    // Minimum time to stay in idle state before becoming Disturbed.
    public Single MinIdleDuration = 1.5f;

    // The furthest the imp can idle from its idle anchor.
    public Single IdleWanderRadius = 25f;

    // Duration in seconds between each idle reposition.
    public Single IdleWanderDuration = 1f;

    // Distance the player needs to be withiin to trigger a state switch to Disturbed.
    public Single DetectionRadius = 300f;

    public Single DisturbedMoveSpeed = 100f;

    public Single DisturbedAccelFactor = 1.4f;

    public GameObject ContactAttackPrefab;


    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindWithTag("Player");

        if (player == null) {
            Debug.LogError("BehaviorImp.Start(): could not find object tagged \"Player\" to target");
            _playerTarget = transform;
        } else {
            _playerTarget = player.transform;
        }

        _rigidbody = GetComponent<Rigidbody2D>();

        if (_rigidbody == null) {
            Debug.LogError("BehaviorImp.Start(): could not find Rigidbody2D component");
        }

        if (ContactAttackPrefab == null) {
            Debug.LogError("BehaviorImp.Start(): could not find attached contact attack prefab");
        }

        _animator = GetComponent<Animator>();

        InitIdle();
    }

    private static Vector2 GenerateNewTarget(Vector2 anchor, Single radius)
    {
        var radians = UnityEngine.Random.Range(0, Mathf.PI * 2f);
        var distance = UnityEngine.Random.Range(0.5f, 1f) * radius;
        var x = Mathf.Cos(radians) * distance;
        var y = Mathf.Sin(radians) * distance;

        return anchor + new Vector2(x, y);
    }

    void IdleReposition(Vector2 lastIdleTarget)
    {
        LastIdleTarget = lastIdleTarget;
        NextIdleTarget = GenerateNewTarget(IdleAnchor, IdleWanderRadius);
        LastIdleWanderTime = Time.fixedTime;
        NextIdleWanderTime = LastIdleWanderTime + IdleWanderDuration;
    }

    Vector2 GetPlayerDisplacement()
    {
        return Utils.To2D(_playerTarget.position) - _rigidbody.position;
    }

    void InitIdle()
    {
        IsIdle = true;
        IdleAnchor = _rigidbody.position;
        CanBeDisturbedTime = Time.fixedTime + MinIdleDuration;
        IdleReposition(_rigidbody.position);
        _animator.SetBool("Idle", true);
        if (_contactAttackObject != null)
            Destroy(_contactAttackObject);
    }

    void UpdateIdle()
    {
        if (Time.fixedTime >= NextIdleWanderTime)
            IdleReposition(NextIdleTarget);

        // If targets were repositioned this frame, "t" should be zero.
        var t = (Time.fixedTime - LastIdleWanderTime) / (NextIdleWanderTime - LastIdleWanderTime);
        var x = Mathf.SmoothStep(LastIdleTarget.x, NextIdleTarget.x, t);
        var y = Mathf.SmoothStep(LastIdleTarget.y, NextIdleTarget.y, t);

        _rigidbody.MovePosition(new Vector2(x, y));

        if (Time.fixedTime < CanBeDisturbedTime)
            return;

        if (GetPlayerDisplacement().sqrMagnitude <= DetectionRadius * DetectionRadius)
            InitDisturbed();
    }

    void InitDisturbed()
    {
        IsIdle = false;
        _contactAttackObject = UnityEngine.GameObject.Instantiate(ContactAttackPrefab, transform);
        _animator.SetBool("Idle", false);
    }

    void UpdateDisturbed()
    {
        var direction = GetPlayerDisplacement().normalized;
        var acceleration = DisturbedMoveSpeed * DisturbedAccelFactor;
        var velocity = _rigidbody.velocity + (direction * acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = Utils.NormalizeAtMagnitude(velocity, DisturbedMoveSpeed);

        if (_contactAttackObject == null)
            InitIdle();
    }

    void UpdateAnimation()
    {
        var displacement = IsIdle ? NextIdleTarget - LastIdleTarget : _rigidbody.velocity;

        if (displacement == Vector2.zero)
            return;

        var direction = displacement.normalized;
        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
    }


    void FixedUpdate()
    {
        if (IsIdle)
            UpdateIdle();
        else
            UpdateDisturbed();
    }

    void Update()
    {
        UpdateAnimation();
    }
}
