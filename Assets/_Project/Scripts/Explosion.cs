using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	private Animator _animator;
	public string ExplosionName = "big explosion with stars";
	public bool PlayOnStart = true;

	[Header("Debug")]
	public bool PlayExplosion;
	public bool DoNotDestroy;

	// Start is called before the first frame update
	void Start()
	{
		_animator = GetComponent<Animator>();
		if (PlayOnStart)
		{
			Explode();
		}
	}

	void Update()
	{
		if (PlayExplosion)
		{
			PlayExplosion = false;
			Explode();
		}
	}

	public void Explode()
	{
		_animator.Play(ExplosionName);
	}

	public void Destroy()
	{
		if (!DoNotDestroy)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Debug.LogWarning("DoNotDestroy was true on Explosion; persisting");
		}
	}
}
