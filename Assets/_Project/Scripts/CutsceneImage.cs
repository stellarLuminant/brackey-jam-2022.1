using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneImage : MonoBehaviour
{
    private Animator Animator
    { 
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                return _animator;
            }
            return _animator;
        }
        set { _animator = value; }
    }
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void Show()
    {
        Animator.Play("Show");
    }

    public void Hide()
    {
        Animator.Play("Hide");
    }

    public void ShowImmediately()
    {
        Animator.Play("Full");
    }

    public void HideImmediately()
    {
        Animator.Play("Empty");
    }
}
