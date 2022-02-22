using Hellmade.Sound;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
	public Collider2D feet;

	[System.Serializable]
	public class AudioStepClip
	{
		public AudioClip stepClip;
		public float volume = 1;
		public float pitchVariation = 0.1f;
	}
	public AudioStepClip[] pillowAudioStep;
	public AudioStepClip[] bridgeAudioStep;
	public AudioStepClip[] cobbleAudioStep;

	[Header("Debug")]
	public bool makeStepSound;

	private void Start()
	{
		feet = GetComponent<Collider2D>();
	}

	private void Update()
	{
		// Debug
		if (makeStepSound)
		{
			makeStepSound = false;
			DoWalkSound();
		}
	}

	public void DoWalkSound()
	{
		AudioStepClip[] stepClips = pillowAudioStep;
		var hitColliders = Physics2D.OverlapCircleAll(feet.transform.position, 1, LayerMask.NameToLayer("Ground"));
		if (hitColliders.Length > 0)
		{
			var closestCollider = hitColliders[0];
			for (var i = 0; i < hitColliders.Length; i++)
			{
				switch (hitColliders[i].name)
				{
					case "Cobble":
						stepClips = cobbleAudioStep;
						break;
				}
			}
		} else {
            Debug.LogWarning("No hit collider found");
        }

		var index = Random.Range(0, pillowAudioStep.Length);
		var randomAudio = pillowAudioStep[index];

		SoundHelper.PlaySoundWithPitchVariation(
			randomAudio.stepClip,
			randomAudio.volume,
			randomAudio.pitchVariation);
	}
}
