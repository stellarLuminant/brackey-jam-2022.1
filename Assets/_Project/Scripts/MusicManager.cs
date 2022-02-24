using Hellmade.Sound;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MusicManager : MonoBehaviour
{
	public static MusicManager Instance;

	public enum MusicState { Off, Beginning, Game, End }
	public MusicState State
	{
		get { return _state; }
		set
		{
			if (_state == value) return;
			PlayMusic(value);
			_state = value;
		}
	}
	MusicState _state;
	public MusicState StartingState;

	[Header("Music Clips")]
	// The music clips and their settings.
	public AudioClip BeginningCutsceneMusic;
	public float BeginningCutsceneVolume = .5f;
	public AudioClip GameMusic;
	public float GameMusicVolume = 1f;
	public AudioClip GameDrumMusic;
	public float GameDrumMusicVolume = 1f;
	public AudioClip EndCutsceneMusic;
	public float EndCutsceneVolume = .5f;

	[Header("Scene Music Settings")]
	// Music tail length in seconds.
	public float MusicTailLength = 2.5f;
	public float GameMusicTailLength = 6f;
	public bool StartMusicByDefault = true;

	// Music source ID
	int musicSourceId = -1;
	int drumMusicSourceId = -1;

	[Header("User Music Settings")]
	public float DefaultGlobalMusicVolume = 0.5f;
	public float DefaultGlobalSoundsVolume = 0.5f;
	public float DefaultGlobalUISoundsVolume = 0.5f;
	public float DefaultGlobalVolume = 0.5f;

	[Header("Debug")]
	public bool ForcePlayMusic;
	public bool GetHellmadeSoundLevels;

	public void PlayMusic(MusicState newState)
	{
		// Stops any music already playing
		StopMusic();

		switch (newState)
		{
			case MusicState.Beginning:
				StartCoroutine(PlayMusic(BeginningCutsceneMusic, BeginningCutsceneVolume));
				break;
			case MusicState.Game:
				StartCoroutine(PlayGameMusic(GameMusic, GameDrumMusic, GameMusicVolume, GameDrumMusicVolume));
				break;
			case MusicState.End:
				StartCoroutine(PlayMusic(EndCutsceneMusic, EndCutsceneVolume));
				break;
			default:
				Debug.LogError("wtf");
				break;
		}
	}

	IEnumerator PlayGameMusic(AudioClip musicClip, AudioClip drumClip, float volume, float drumVolume, float fadeInSeconds = 0f, float fadeOutSeconds = 0f)
	{
		musicSourceId = PlayMusic(musicClip, volume, false, true, fadeInSeconds, fadeOutSeconds);
		drumMusicSourceId = PlayMusic(drumClip, drumVolume, false, true, fadeInSeconds, fadeOutSeconds);

		// Safety because I don't trust myself
		var iterations = 0;
		while (iterations < 100000)
		{
			Debug.Log($"Playing game music clips");
			var musicAudio = EazySoundManager.GetMusicAudio(musicSourceId);
			musicAudio.Play(true);

			var drumAudio = EazySoundManager.GetMusicAudio(drumMusicSourceId);
			drumAudio.Play(true);

			yield return new WaitForSecondsRealtime(musicClip.length - GameMusicTailLength);
			iterations++;
		}
	}

	IEnumerator PlayMusic(AudioClip clip, float volume, float fadeInSeconds = 0f, float fadeOutSeconds = 0f)
	{
		musicSourceId = PlayMusic(clip, volume, false, true, fadeInSeconds, fadeOutSeconds);

		// Safety because I don't trust myself
		var iterations = 0;
		while (iterations < 100000)
		{
			Debug.Log($"Playing music clip");
			var musicAudio = EazySoundManager.GetMusicAudio(musicSourceId);
			musicAudio.Play(true);
			yield return new WaitForSecondsRealtime(clip.length - MusicTailLength);

			iterations++;
		}
	}

	public void StopMusic(float fadeOutInSeconds = 2)
	{
		StopAllCoroutines();
		var musicAudio = EazySoundManager.GetMusicAudio(musicSourceId);
		if (musicAudio != null)
		{
			musicAudio.FadeInSeconds = fadeOutInSeconds;
			musicAudio.FadeOutSeconds = fadeOutInSeconds;
			musicAudio.Stop();
		}

		var drumAudio = EazySoundManager.GetMusicAudio(musicSourceId);
		if (drumAudio != null)
		{
			drumAudio.FadeInSeconds = fadeOutInSeconds;
			drumAudio.FadeOutSeconds = fadeOutInSeconds;
			drumAudio.Stop();
		}
	}

	#region Re-implementation of EazySoundManager
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

		EazySoundManager.GetAudio(audioType, false, audioID).Play(true);

		return audioID;
	}
	#endregion

	#region Unity events
	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	// Singleton pattern
	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(this);
	}

	// Start is called before the first frame update
	private void Start()
	{
		if (StartMusicByDefault)
		{
			PlayMusic(StartingState);
		}

		EazySoundManager.GlobalMusicVolume = DefaultGlobalMusicVolume;
		EazySoundManager.GlobalSoundsVolume = DefaultGlobalSoundsVolume;
		EazySoundManager.GlobalUISoundsVolume = DefaultGlobalUISoundsVolume;
		EazySoundManager.GlobalVolume = DefaultGlobalVolume;
	}

	// Update is called once per frame
	private void Update()
	{
		if (ForcePlayMusic)
		{
			ForcePlayMusic = false;
			PlayMusic(StartingState);
		}

		if (GetHellmadeSoundLevels)
		{
			GetHellmadeSoundLevels = false;
			Debug.Log($"music: {EazySoundManager.GlobalMusicVolume}");
			Debug.Log($"sound: {EazySoundManager.GlobalSoundsVolume}");
			Debug.Log($"ui: {EazySoundManager.GlobalUISoundsVolume}");
			Debug.Log($"global: {EazySoundManager.GlobalVolume}");
		}
	}
	#endregion

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{

	}
}
