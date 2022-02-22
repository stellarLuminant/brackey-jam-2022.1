using Hellmade.Sound;
using UnityEngine;

public static class SoundHelper
{
	public static void PlayRandomSound(AudioClip[] clips, float volume, float pitchVariation = 0.05f)
	{
		var index = Random.Range(0, clips.Length);
		var randomAudio = clips[index];

		PlaySoundWithVariation(randomAudio, volume, pitchVariation);
	}

	public static void PlaySoundWithVariation(AudioClip clip, float volume, float volumeVariation = 0, float pitchVariation = 0.05f)
	{
		int id = EazySoundManager.PlaySound(clip, volume + Random.Range(-volumeVariation, volumeVariation));
		Audio audio = EazySoundManager.GetSoundAudio(id);
		if (audio != null)
		{
			audio.Pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
		}
	}
}
