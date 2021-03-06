using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
	#region State

	[Header("State")]
	// The current direction the character is looking at.
	public Vector3 LookDirection;

	// The current time in seconds for when the player cannot receive damage.
	public Single InvulTimer;

	// The current time in seconds for when the player cannot attack.
	public Single AttackTimer;

	// The current number of life points the player has.
	public Int32 CurrentLife;

	private Rigidbody2D Rigidbody;

	private Animator Animator;

	// The position the player was last frame.
	private Vector2 OldPosition;

	private PlayerSounds Sounds;

	// The amount of times the player has chained attacks.
	private Int32 AttackTimes;

	// Will be true if the attack key was pressed during cooldown.
	private bool AttackBuffer;

	// Contains Time.time, when the first attack animation was played.
	private float AttackTimestamp;

	private HeartContainer Hearts;

	#endregion State

	#region Parameters

	[Header("Parameters")]
	// Tiles per second.
	public Single MoveSpeed = 100;

	// Time in seconds when the player is invul after taking damage.
	public Single InvulDuration = 2;

	// Time in seconds when the player cannot attack again.
	public Single AttackCooldown = 0.5f;

	// Time in seconds when the player cannot move after an attack.
	public Single AttackMoveCooldown = 0.125f;

	// Distance in pixels that the attack object will spawn away from the player.
	public Single AttackDistance = 10;

	// Player starts the game with this many life points.
	public Int32 StartingLife = 3;

	// Player can store up to a maximum of this many life points.
	public Int32 MaximumLife = 5;

	// The attack object spawned by the attack action.
	public GameObject AttackPrefab;

	// Multiple inputs we can allow.
	public KeyCode[] Input_MoveUp = { KeyCode.UpArrow, KeyCode.W };
	public KeyCode[] Input_MoveDown = { KeyCode.DownArrow, KeyCode.S };
	public KeyCode[] Input_MoveLeft = { KeyCode.LeftArrow, KeyCode.A };
	public KeyCode[] Input_MoveRight = { KeyCode.RightArrow, KeyCode.D };
	public KeyCode[] Input_Attack = { KeyCode.Space, KeyCode.Return, KeyCode.Z };

	#endregion Parameters

	#region Helpers

	private bool IsMovingUp => Utils.CheckInputsHeld(Input_MoveUp);

	private bool IsMovingDown => Utils.CheckInputsHeld(Input_MoveDown);

	private bool IsMovingLeft => Utils.CheckInputsHeld(Input_MoveLeft);

	private bool IsMovingRight => Utils.CheckInputsHeld(Input_MoveRight);

	private bool IsAttackPressed => Utils.CheckInputsPressed(Input_Attack);

	private bool IsInAttackStop => AttackTimer > AttackCooldown - AttackMoveCooldown;

	private Vector3 GetVerticalMoveDirection()
	{
		bool up = IsMovingUp;
		bool down = IsMovingDown;

		if (up)
		{
			return down
				? Vector3.zero
				: new Vector3(0, 1, 0);
		}

		if (down)
			return new Vector3(0, -1, 0);

		return Vector3.zero;
	}

	private Vector3 GetHorizontalMoveDirection()
	{
		bool left = IsMovingLeft;
		bool right = IsMovingRight;

		if (left)
		{
			return right
				? Vector3.zero
				: new Vector3(-1, 0, 0);
		}

		if (right)
			return new Vector3(1, 0, 0);

		return Vector3.zero;
	}

	public Vector3 GetMoveDirection()
	{
		Vector3 direction = GetVerticalMoveDirection() + GetHorizontalMoveDirection();

		return direction == Vector3.zero
			? Vector3.zero
			: Vector3.Normalize(direction);
	}

	// Sets the LookDirection to MoveDirection if it is non-zero
	private void SetLookDirection(Vector3 moveDir)
	{
		if (moveDir == Vector3.zero)
			return;

		// Animator should know the direction the player is going, including diagonals
		Animator.SetFloat("Horizontal", moveDir.x);
		Animator.SetFloat("Vertical", moveDir.y);

		// If movement is diagonal
		if (moveDir.x != 0 && moveDir.y != 0)
		{
			// If there's y position changes, pass that through
			// due to how the animator works.
			// Else, ignore diagonal x movement.
			if (moveDir.y != 0)
				LookDirection = new Vector3(0, Math.Sign(moveDir.y), 0);

			return;
		}

		LookDirection = moveDir;
	}

	private bool IsApproximately(Vector2 a, Vector2 b, float range = 0.001f)
	{
		return IsApproximately(a.x, b.x) && IsApproximately(a.y, b.y);
	}

	private bool IsApproximately(float a, float b, float range = 0.001f)
	{
		return Math.Abs(b - a) < range;
	}

	private Vector3 GetAttackDisplacement()
	{
		return LookDirection * AttackDistance;
	}

	#endregion Helpers

	#region Unity Behaviour

	// Start is called before the first frame update
	private void Start()
	{
		Animator = GetComponent<Animator>();
		Rigidbody = GetComponent<Rigidbody2D>();
		Sounds = GetComponent<PlayerSounds>();
		Hearts = FindObjectOfType<HeartContainer>();

		// Player looks down on init.
		SetLookDirection(new Vector3(0, -1, 0));

		// Animation variable startup
		Animator.SetBool("Idle", true);

		// Health logic
		CurrentLife = StartingLife;
		Hearts.Init(CurrentLife);
	}

	private void UpdateMovement()
	{
		OldPosition = Rigidbody.position;

		if (IsInAttackStop)
			return;

		if (CurrentLife <= 0)
			return;

		Vector3 moveDir = GetMoveDirection();
		Rigidbody.velocity = moveDir * MoveSpeed;
		SetLookDirection(moveDir);
	}

	private void UpdateAttack()
	{
		if (CurrentLife <= 0)
			return;

		AttackTimer = Mathf.Max(0, AttackTimer - Time.deltaTime);

		if (AttackTimer > 0 || (!IsAttackPressed && !AttackBuffer))
		{
			if (IsAttackPressed)
				AttackBuffer = true;
			return;
		}

		if (AttackBuffer)
		{
			AttackTimes++;
			AttackTimes = AttackTimes % 2;
		}
		else
		{
			AttackTimes = 0;
		}

		AttackBuffer = false;
		AttackTimer = AttackCooldown;
		var attackPosition = (Vector3)Rigidbody.position + GetAttackDisplacement();
		var attackRotation = Utils.NormalToDeg(Utils.To2D(LookDirection));
		var attackObject = UnityEngine.Object.Instantiate(AttackPrefab, attackPosition, Quaternion.Euler(0, 0, attackRotation));

		// Attack Aniamtion 
		if (AttackTimes == 0)
		{
			Animator.Play("Attack");
			AttackTimestamp = Time.time;
		}
		else
		{
			Animator.Play("Attack 2", 0, (Time.time - AttackTimestamp) / .6f);
		}
		Sounds.DoAttackSound(AttackTimes);
		Animator.SetBool("Idle", true);
	}

	private void UpdateInvul()
	{
		InvulTimer = Mathf.Max(0, InvulTimer - Time.fixedDeltaTime);
	}

	private void UpdateAnimation()
	{
		if (CurrentLife <= 0)
		{
			// Death animation handled in Player.ReceiveAttack()
			return;
		}

		if (IsInAttackStop)
		{
			// Attack animation handled by Player.UpdateAttack()
			return;
		}

		var idle = Animator.GetBool("Idle");
		Vector3 moveDir = GetMoveDirection();
		if (moveDir == Vector3.zero)
		{
			if (!idle)
			{
				// Debug.Log("Idle moveDir == Vector3.zero");

				Animator.Play("Idle");
				Animator.SetBool("Idle", true);
			}
			return;
		}

		// If player is trying to move, but isn't due to collisions,
		// play the idle animation
		if (IsApproximately(OldPosition, Rigidbody.position))
		{
			if (!idle)
			{
				// Debug.Log("Idle wall");

				Animator.Play("Idle");
				Animator.SetBool("Idle", true);
			}
			return;
		}

		if (idle)
		{
			// Debug.Log("Movement");
			Animator.Play("Movement");
			Animator.SetBool("Idle", false);
		}
	}

	private void ReceiveAttack(GameObject attack)
	{
		if (InvulTimer > 0)
			return;

		if (CurrentLife <= 0)
			return;

		CurrentLife -= 1;
		InvulTimer = InvulDuration;

		Hearts.Health = CurrentLife;

		var isIdle = GetMoveDirection() == Vector3.zero;
		if (CurrentLife > 0)
		{
			Debug.Log("Player.ReceiveAttack(): player received damage");
			// TODO: Hurt and game over states.
			Sounds.DoHurtSound();
			Animator.Play("Ouch");
			Animator.SetBool("Idle", isIdle);
		}
		else
		{
			Debug.Log("Player.ReceiveAttack(): player lost all life");
			Sounds.DoHurtSound();
			Sounds.DoDeathSound();
			Animator.Play("Game Over");
			Animator.SetBool("Idle", isIdle);
			// TellManagerThatTheresAGameOver();
		}
	}

	// FixedUpdate is called once per physics update
	private void FixedUpdate()
	{
		UpdateMovement();
		UpdateInvul();
	}

	// Update is called once per frame
	private void Update()
	{
		UpdateAttack();
		UpdateAnimation();
	}

	private void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.layer == Utils.EnemyAttackLayer)
		{
			ReceiveAttack(c.gameObject);
			return;
		}

		// add more collision cases above
		Debug.Log($"Player.OnTriggerEnter2D(Collider2D): layer \"{c.gameObject.layer}\" was not processed");
	}

	#endregion Unity Behaviour
}
