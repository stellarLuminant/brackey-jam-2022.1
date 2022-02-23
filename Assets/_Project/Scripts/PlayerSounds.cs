using System.Linq;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
	[System.Serializable]
	public class AudioStepClip
	{
		public AudioClip stepClip;
		public float volume = 1;
		public float volumeVariation;
		public float pitchVariation = 0.1f;
	}

	public Collider2D _feet;

	public LayerMask GroundMask;
	public float PillowSqueakMultiplier = 0.25f;

	private Player _player;
	private int _squeakAlternatingIndex = 0;
	private int _alternatingIndex = 0;

	private bool _disableDiagonalFootstepRetrigger;

	[Header("Game Objects")]
	public GameObject PillowTilemap;
	public GameObject BridgeTilemap;
	public GameObject CobbleTilemap;


	[Header("Sound Data")]
	public AudioStepClip HurtClip;
	public AudioStepClip[] AttackClips;
	public AudioStepClip[] SqueakAudioStep;
	public AudioStepClip[] PillowAudioStep;
	public AudioStepClip[] BridgeAudioStep;
	public AudioStepClip[] CobbleAudioStep;

	[Header("Debug")]
	public bool UseDoubleSqueak = true;
	public bool DisableSqueak = true;
	public bool MakeStepSound;

	#region Unity Behavior

	private void Start()
	{
		_player = GetComponent<Player>();

		Debug.Assert(_player != null, "PlayerSounds couldn't find Player component");
		Debug.Assert(CobbleTilemap != null, "CobbleTilemap must be filled in inspector");
		Debug.Assert(BridgeTilemap != null, "BridgeTilemap must be filled in inspector");
		Debug.Assert(PillowTilemap != null, "PillowTilemap must be filled in inspector");
	}

	private void Update()
	{
		// Debug
		if (MakeStepSound)
		{
			MakeStepSound = false;
			DoWalkSound();
		}
	}

	#endregion

	private void PlaySoundGenerically(AudioStepClip clip)
	{
		SoundHelper.PlaySoundWithVariation(
			clip.stepClip,
			clip.volume,
			clip.volume != 0 ? clip.volumeVariation : 0,
			clip.pitchVariation);
	}
	public void DoWalkSound()
	{
		// By walking diagonally, even though only one animation
		// shows, DoWalkSound() will be triggered twice.
		// To compensate, _disableDiagonalFootstepRetrigger is used.
		if (_disableDiagonalFootstepRetrigger)
		{
			_disableDiagonalFootstepRetrigger = false;
			return;
		}

		// Are we going diagonally?
		Vector3 a = _player.GetMoveDirection();
		if (Mathf.Abs(a.x) > 0 && Mathf.Abs(a.y) > 0)
		{
			_disableDiagonalFootstepRetrigger = true;
		}

		AudioStepClip[] stepClips = PillowAudioStep;
		var hitColliders = Physics2D.OverlapCircleAll(_feet.transform.position, 1, GroundMask);
		if (hitColliders.Length > 0)
		{
			var closestCollider = hitColliders[0];
			for (var i = 0; i < hitColliders.Length; i++)
			{
				if (hitColliders[i].name.Equals(CobbleTilemap.name))
				{
					stepClips = CobbleAudioStep;
				}
				else if (hitColliders[i].name.Equals(BridgeTilemap.name))
				{
					stepClips = BridgeAudioStep;

				}
				else if (hitColliders[i].name.Equals(PillowTilemap.name))
				{
					stepClips = PillowAudioStep;

				}
			}
		}
		else
		{
			// Debug.LogWarning("No hit collider found");
		}

		// var index = Random.Range(0, stepClips.Length);
		var index = Mathf.Min(_alternatingIndex, stepClips.Length - 1);
		var randomAudio = stepClips[index];

		var squeakIndex = Mathf.Min(_squeakAlternatingIndex, SqueakAudioStep.Length - 1);
		var squeakClip = SqueakAudioStep[squeakIndex];

		// Debug.Log($"randomAudio.stepClip: ${randomAudio.stepClip.name}");
		PlaySoundGenerically(randomAudio);

		// Squeaky footsteps logic
		if (!DisableSqueak)
		{
			var squeakVolume = squeakClip.volume;
			var squeakVolumeVariation = squeakVolume != 0 ? squeakClip.volumeVariation : 0;

			if (stepClips.SequenceEqual(PillowAudioStep))
			{
				squeakVolume = squeakVolume * PillowSqueakMultiplier;
				squeakVolumeVariation = squeakVolumeVariation * PillowSqueakMultiplier;
			}
			if (UseDoubleSqueak)
			{
				SoundHelper.PlaySoundWithVariation(
					squeakClip.stepClip,
					squeakVolume / 2,
					squeakVolumeVariation,
					squeakClip.pitchVariation);

				SoundHelper.PlaySoundWithVariation(
					squeakClip.stepClip,
					squeakVolume / 2,
					squeakVolumeVariation);
			}
			else
			{
				// Debug.Log($"squeakClip.stepClip: ${squeakClip.stepClip.name}");
				SoundHelper.PlaySoundWithVariation(
					squeakClip.stepClip,
					squeakVolume,
					squeakVolumeVariation,
					squeakClip.pitchVariation);

			}
		}

		_alternatingIndex = (_alternatingIndex + 1) % stepClips.Length;
		_squeakAlternatingIndex = (squeakIndex + 1) % SqueakAudioStep.Length;
	}

	public void DoAttackSound(int attackTimes)
	{
		attackTimes = attackTimes % AttackClips.Length;
		PlaySoundGenerically(AttackClips[attackTimes]);
	}

	public void DoHurtSound()
	{
		PlaySoundGenerically(HurtClip);
	}

}
