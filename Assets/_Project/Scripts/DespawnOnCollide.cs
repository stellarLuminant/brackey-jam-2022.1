using System;
using UnityEngine;

public class DespawnOnCollide : MonoBehaviour
{
    // The layers that the trigger will check for.
    public String[] Layers;

    private Int32[] _layerIndices;

    // Start is called before the first frame update
    void Start() 
    { 
        _layerIndices = new Int32[Layers.Length];

        for (Int32 i = 0; i < Layers.Length; i++)
            _layerIndices[i] = LayerMask.NameToLayer(Layers[i]);
    }

    // Update is called once per frame
    void Update() { }

    void OnTriggerEnter2D(Collider2D c)
    {
        for (Int32 i = 0; i < _layerIndices.Length; i++)
        {
            if (c.gameObject.layer != _layerIndices[i])
                continue;
            
            Destroy(gameObject);
            return;
        }
    }
}
