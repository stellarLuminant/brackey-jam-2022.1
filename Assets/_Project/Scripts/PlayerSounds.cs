﻿using System.Linq;
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

	private int _squeakAlternatingIndex = 0;
	private int _alternatingIndex = 0;

	[Header("Game Objects")]
	public GameObject PillowTilemap;
	public GameObject BridgeTilemap;
	public GameObject CobbleTilemap;


	[Header("Sound Data")]
	public AudioStepClip[] SqueakAudioStep;
	public AudioStepClip[] PillowAudioStep;
	public AudioStepClip[] BridgeAudioStep;
	public AudioStepClip[] CobbleAudioStep;

	[Header("Debug")]
	public bool MakeStepSound;

	private void Start()
	{
		_feet = GetComponent<Collider2D>();

		Debug.Assert(CobbleTilemap != null, "All tilemaps must be filled in inspector");
		Debug.Assert(BridgeTilemap != null, "All tilemaps must be filled in inspector");
		Debug.Assert(PillowTilemap != null, "All tilemaps must be filled in inspector");
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

	public void DoWalkSound()
	{
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
			Debug.LogWarning("No hit collider found");
		}

		// var index = Random.Range(0, stepClips.Length);
		var index = Mathf.Min(_alternatingIndex, stepClips.Length - 1);
		var randomAudio = stepClips[index];

		var squeakIndex = Mathf.Min(_squeakAlternatingIndex, SqueakAudioStep.Length - 1);
		var squeakClip = SqueakAudioStep[squeakIndex];

		SoundHelper.PlaySoundWithVariation(
			randomAudio.stepClip,
			randomAudio.volume,
			randomAudio.volumeVariation,
			randomAudio.pitchVariation);

		var squeakVolume = squeakClip.volume;
		var squeakVolumeVariation = squeakClip.volumeVariation;
		if (stepClips.SequenceEqual(PillowAudioStep))
		{
			squeakVolume = squeakVolume * PillowSqueakMultiplier;
			squeakVolumeVariation = squeakVolumeVariation * PillowSqueakMultiplier;
		}
		SoundHelper.PlaySoundWithVariation(
			squeakClip.stepClip,
			squeakVolume,
			squeakVolumeVariation,
			squeakClip.pitchVariation);

		_alternatingIndex = (_alternatingIndex + 1) % stepClips.Length;
		squeakIndex = (squeakIndex + 1) % SqueakAudioStep.Length;
	}
}
