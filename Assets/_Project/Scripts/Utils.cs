using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 ToGridPosition(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), pos.z);
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

    private static Vector2 CollisionBoxSize = new Vector2(0.125f, 0.125f);
    private static Single CollisionBoxAngle = 0;
    private static Vector2 CollisionBoxDirection = new Vector2(1, 0);
    private static Single CollisionBoxDistance = 0;
    private static LayerMask ObstacleLayerMask = (LayerMask)(LayerMask.GetMask("Wall") + LayerMask.GetMask("Objects"));
    private static LayerMask ObjectLayerMask = LayerMask.GetMask("Objects");

    public static bool IsTileEmpty(Vector3 pos)
    {
        return Physics2D.BoxCast(pos, CollisionBoxSize, CollisionBoxAngle, CollisionBoxDirection, CollisionBoxDistance, ObstacleLayerMask).collider == null;
    }

    public static Collider2D CastForObjectOnTile(Vector3 pos)
    {
        return Physics2D.BoxCast(pos, CollisionBoxSize, CollisionBoxAngle, CollisionBoxDirection, CollisionBoxDistance, ObjectLayerMask).collider;
    }
}
