using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// oh god i tried to hack in up and down paralax
/// for a thing that im not even sure will look good
/// </summary>
public class ScrollingParalax : MonoBehaviour
{
    [Header("References")]
    private Transform cameraTransform;
    private Sprite thisSprite;

    [Header("Tweaks")]
    [SerializeField] private float paralaxSpeed = 0.25f;
    [SerializeField] private float viewZone = 0.5f;
    [SerializeField] private bool placeAtStart = true;
    [SerializeField] private bool scrolling = true;
    [SerializeField] private bool paralax = true;

    [Header("Logic fields")]
    private Transform[] layers;
    private float backgroundSize;
    private float lastCameraX;
    private float lastCameraY;
    private int upIndex;
    private int downIndex;
    private int leftIndex;
    private int rightIndex;

    private void Start()
    {
        cameraTransform = Camera.main.transform;

        // Let's get some refs
        int cc = transform.childCount;
        if (cc < 3)
        {
            Debug.Log("This object needs atleast 3 child sprites");
            return;
        }

        layers = new Transform[cc];
        for (int i = 0; i < cc; i++)
        {
            layers[i] = transform.GetChild(i);
        }

        backgroundSize = layers[0].GetComponent<SpriteRenderer>().bounds.size.x;
        leftIndex = 0;
        rightIndex = layers.Length - 1;

        // Move assets to their initial location
        if (placeAtStart)
        {
            for (int i = 0; i < layers.Length; i++)
                ScrollLeft();
            for (int i = 0; i < layers.Length; i++)
                ScrollRight();
        }
    }

    private void Update()
    {
        if (paralax)
        {
            float deltaX = cameraTransform.position.x - lastCameraX;
            transform.position += Vector3.right * (deltaX * paralaxSpeed);
            lastCameraX = cameraTransform.position.x;

            float deltaY = cameraTransform.position.y - lastCameraY;
            transform.position += Vector3.up * (deltaY * paralaxSpeed);
            lastCameraY = cameraTransform.position.y;
        }

        if (scrolling && cameraTransform)
        {
            if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + viewZone))
                ScrollLeft();

            if (cameraTransform.position.x > (layers[rightIndex].transform.position.x - viewZone))
                ScrollRight();

            if (cameraTransform.position.y < (layers[downIndex].transform.position.y + viewZone))
                ScrollUp();

            if (cameraTransform.position.y > (layers[upIndex].transform.position.y - viewZone))
                ScrollDown();
        }
    }

    private void ScrollLeft()
    {
        float x = (layers[leftIndex].position.x - backgroundSize);
        layers[rightIndex].position = new Vector3(x, layers[rightIndex].position.y, layers[rightIndex].position.z);
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
            rightIndex = layers.Length - 1;
    }
    private void ScrollRight()
    {
        float x = (layers[rightIndex].position.x + backgroundSize);
        layers[leftIndex].position = new Vector3(x, layers[rightIndex].position.y, layers[rightIndex].position.z);
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
            leftIndex = 0;
    }

    private void ScrollUp()
    {
        float y = (layers[downIndex].position.y - backgroundSize);
        layers[upIndex].position = new Vector3(layers[upIndex].position.x, y, layers[upIndex].position.z);
        downIndex = upIndex;
        upIndex--;
        if (upIndex < 0)
            upIndex = layers.Length - 1;
    }
    private void ScrollDown()
    {
        float y = (layers[upIndex].position.y + backgroundSize);
        layers[downIndex].position = new Vector3(layers[upIndex].position.x, y, layers[upIndex].position.z);
        upIndex = downIndex;
        downIndex++;
        if (downIndex == layers.Length)
            downIndex = 0;
    }
}