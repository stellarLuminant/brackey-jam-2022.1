using System;
using UnityEngine;

public class TestEnemyAttack : MonoBehaviour
{
    [Header("State")]
    // The current time before the enemy can attack again.
    public Single AttackTimer;

    [Header("Parameters")]
    // Time spent between each attack.
    public Single AttackCooldown = 1.5f;

    // The furthest a target can be before the enemy stops shooting.
    public Single MaxAttackRange = 400;

    // The closest a target can be before the enemy stops shooting.
    public Single MinAttackRange = 25;

    // The attack object that is spawned upon attacking.
    public GameObject AttackPrefab;
    private Transform _target;


    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindWithTag("Player");

        if (player == null) {
            Debug.LogError("TestEnemyAttack.Start(): could not find object tagged \"Player\" to target");
            _target = transform;
        } else {
            _target = player.transform;
        }

        if (AttackPrefab == null) {
            Debug.LogError("TestEnemyAttack.Start(): could not find AttackPrefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        AttackTimer = Mathf.Max(0, AttackTimer - Time.deltaTime);

        if (AttackTimer > 0)
            return;
        
        var displacement = Utils.To2D(_target.position) - Utils.To2D(transform.position);

        if (displacement.sqrMagnitude < MinAttackRange * MinAttackRange)
            return;
        
        if (displacement.sqrMagnitude > MaxAttackRange * MaxAttackRange)
            return;
        
        var angle = Utils.NormalToDeg(displacement.normalized);
        UnityEngine.Object.Instantiate(AttackPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        AttackTimer = AttackCooldown;
    }
}
