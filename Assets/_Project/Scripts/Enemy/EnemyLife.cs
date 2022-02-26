using System;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
	[Header("State")]
	// The current number of life points the enemy has.
	public Int32 Life = 1;

	private Animator _animator;

	private Rigidbody2D _rigidbody;

	[Header("Parameters")]
	// Emits a "Hurt" trigger to the animator if true.
	public Boolean HasHurtAnimation = false;

	public GameObject ExplosionPrefab;

	void Start()
	{
		if (HasHurtAnimation)
			_animator = GetComponent<Animator>();

		_rigidbody = GetComponent<Rigidbody2D>();
	}

	void Update() { }

	private void ReceiveAttack()
	{
		Life = Mathf.Max(0, Life - 1);

		_rigidbody.velocity = Vector2.zero;

		if (HasHurtAnimation)
			_animator?.SetTrigger("Hurt");

		if (Life == 0)
		{
			if (ExplosionPrefab)
			{
				Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
			}
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.layer == Utils.PlayerAttackLayer)
		{
			ReceiveAttack();
			return;
		}
	}
}
