using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A rough dynamic music manager
/// </summary>
public class MusicManager : MonoBehaviour
{
    public AudioClip baseMusic;
    public AudioClip baseMusicDrums;
    public AudioClip baseMusicDrumsFast;
    float baseMusicLength;
    public float musicTailLength = 5;

    int musicSourceId;
    int musicDrumSourceId;
    Audio musicDrumFastSource;
    public float transitionTime = 1.5f;

    bool playDrumVariant;
    public bool PlayDrumVariant
    {
        get { return playDrumVariant; }
        set {
            var musicAudio = EazySoundManager.GetMusicAudio(musicSourceId);
            var musicDrumAudio = EazySoundManager.GetMusicAudio(musicDrumSourceId);
            
            musicAudio.SetVolume(!value ? 1 : 0);
            musicDrumAudio.SetVolume(value ? 1 : 0);

            playDrumVariant = value;
        }
    }

    [Header("Debug")]
    public bool forcePlayDrumVariant = false;

    // Start is called before the first frame update
    void Start()
    {
        baseMusicLength = baseMusic.length - musicTailLength;

        StartCoroutine(PlayMusic());
    }

    // Update is called once per frame
    void Update()
    {
        if (forcePlayDrumVariant)
        {
            Debug.Log("Forcing change on drum variant");
            forcePlayDrumVariant = false;
            PlayDrumVariant = !PlayDrumVariant;
        }
    }
    
    public IEnumerator PlayFastDrumsEnum()
    {
        StopAllCoroutines();

        int id = EazySoundManager.PlayMusic(baseMusicDrumsFast, 1, false, false, 0, 3, 0.2f, null);
        musicDrumFastSource = EazySoundManager.GetAudio(id);

        // SAFETY
        var iterations = 0;
        while (iterations < 1000)
        {
            yield return new WaitForSecondsRealtime(baseMusicDrumsFast.length - musicTailLength);
            musicDrumFastSource.Play();
            iterations++;
        }
    }

    /// <summary>
    /// Loops through 
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayMusic()
    {
        // Initial play music
        musicSourceId = PlayMusic(baseMusic, GetVolume(false), false, false, transitionTime, transitionTime);
        musicDrumSourceId = PlayMusic(baseMusicDrums, GetVolume(true), false, false, transitionTime, transitionTime);

        // SAFETY
        var iterations = 0;
        while (iterations < 1000)
        {
            StartCoroutine(PlayAmbience());
            yield return PlayAmbienceDrums();
            iterations++;
        }
    }

    int GetVolume(bool isDrums)
    {
        if (isDrums)
            return playDrumVariant ? 1 : 0;
        else
            return !playDrumVariant ? 1 : 0;
    }

    IEnumerator PlayAmbience()
    {
        var musicAudio = EazySoundManager.GetMusicAudio(musicSourceId);
        musicAudio.Play(GetVolume(false));
        yield return new WaitForSecondsRealtime(baseMusicLength);
    }

    IEnumerator PlayAmbienceDrums()
    {
        var musicDrumAudio = EazySoundManager.GetMusicAudio(musicDrumSourceId);
        musicDrumAudio.Play(GetVolume(true));
        yield return new WaitForSecondsRealtime(baseMusicLength);
    }


    /// <summary>
    /// Play background music
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="volume"> The volume the music will have</param>
    /// <param name="loop">Wether the music is looped</param>
    /// <param name="persist"> Whether the audio persists in between scene changes</param>
    /// <param name="fadeInSeconds">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
    /// <param name="fadeOutSeconds"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
    /// <returns>The ID of the created Audio object</returns>
    public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
    {
        return PlayAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
    }


    private static int PlayAudio(
        Audio.AudioType audioType, 
        AudioClip clip, 
        float volume,
        bool loop, 
        bool persist, 
        float fadeInSeconds, 
        float fadeOutSeconds, 
        float currentMusicfadeOutSeconds, 
        Transform sourceTransform)
    {
        int audioID = EazySoundManager.PrepareAudio(audioType, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);

        EazySoundManager.GetAudio(audioType, false, audioID).Play();

        return audioID;
    }
}
