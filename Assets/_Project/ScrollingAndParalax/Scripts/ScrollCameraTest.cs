using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCameraTest : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] private float speed = 5.0f;

    private void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, 0);
    }
}
