using System;
using UnityEngine;

public static class Utils
{
    public static Vector3 ToGridPosition(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), pos.z);
    }

    public static Vector2 To2D(Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 NormalizeAtMagnitude(Vector2 v, float magnitude) {
        return v.normalized * magnitude;
    }

    public static bool CheckInputsHeld(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKey(key))
                return true;

        return false;
    }

    public static bool CheckInputsPressed(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKeyDown(key))
                return true;

        return false;
    }

    public static bool CheckInputsLifted(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKeyUp(key))
                return true;

        return false;
    }

    public static readonly int ObstacleLayer = LayerMask.GetMask("Obstacle");

    public static readonly int PlayerLayer = LayerMask.GetMask("Player");

    public static readonly int EnemyLayer = LayerMask.GetMask("Enemy");

    public static readonly int PlayerAttackLayer = LayerMask.GetMask("PlayerAttack");

    public static readonly int EnemyAttackLayer = LayerMask.GetMask("EnemyAttack");

    public static readonly int GroundLayer = LayerMask.GetMask("Ground");
}
