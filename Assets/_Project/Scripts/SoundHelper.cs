using Hellmade.Sound;
using UnityEngine;

public static class SoundHelper
{
    public static void PlayRandomSound(AudioClip[] clips, float volume, float pitchVariation = 0.05f)
    {
        var index = Random.Range(0, clips.Length);
        var randomAudio = clips[index];

        PlaySoundWithPitchVariation(randomAudio, volume, pitchVariation);
    }

    public static void PlaySoundWithPitchVariation(AudioClip clip, float volume, float pitchVariation = 0.05f)
    {
        int id = EazySoundManager.PlaySound(clip, volume);
        Audio audio = EazySoundManager.GetSoundAudio(id);
        if (audio != null)
        {
            audio.Pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        }
    }
}
