using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [Header("Cutscene Settings")]
    // How long to hold the Skip input for, in seconds.
    public float HoldToSkipLength = 2f;
    public CutsceneImage[] CutsceneImages;
    public float ImageHeldLength = 2f;
    public float ImageReleaseLength = 2f;

    public float WaitBeforeStarting = .5f;
    public float WaitAfterFinished = 1f;

    public UIManager.GameState NewSceneState = UIManager.GameState.Game;
    public string SceneName;
    
    public bool JankMusicChange = false;
    public int JankMusicChangeIndex = 3;

    [Header("Inputs")]
    // Multiple inputs we can allow.
    public KeyCode[] Input_Skip = { KeyCode.Escape };
    public KeyCode[] Input_Interact = { KeyCode.Space, KeyCode.Return, KeyCode.Z };

    private int _sceneImageIndex;
    private float _holdToSkipTimer;
    private bool _transitioning;

    #region Helpers
    private bool IsSkipping => Utils.CheckInputsHeld(Input_Skip);

    private bool IsInteracting => Utils.CheckInputsPressed(Input_Interact);
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        var uIManager = UIManager.Instance;
        uIManager.State = UIManager.GameState.Cutscene;
        if (JankMusicChange)
            uIManager.FadeManager.fadeGroup.alpha = 0;

        StartCoroutine(StartCutscene());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSkipping)
        {
            _holdToSkipTimer += Time.deltaTime;
            if (_holdToSkipTimer >= HoldToSkipLength && !_transitioning)
            {
                Debug.Log("Going to game scene");
                _transitioning = true;
            }
        } else
        {
            _holdToSkipTimer = 0;
        }
    }

    private IEnumerator StartCutscene()
    {
        bool firstTime = true;
        var music = MusicManager.Instance;

        for (int i = 0; i < CutsceneImages.Length; i++)
        {
            if (JankMusicChange && i == JankMusicChangeIndex)
            {
                music.StopMusic();
                music.State = MusicManager.MusicState.End;
            }

            var newImage = CutsceneImages[i];

            if (newImage == null) 
            {
                Debug.LogWarning("Cutscene Image is missing! Skipping...");
                continue;
            }
            
            // Waits either input, or time elapsed
            float timer = 0;
            bool uwu = false;
            do
            {
                timer += Time.deltaTime;

                // Delays showing the image if it's the first time
                if ((timer >= WaitBeforeStarting || !firstTime) && !uwu)
                {
                    Debug.Log($"Showing element {newImage.name} | {firstTime}");
                    newImage.Show();
                    uwu = true;
                }

                yield return null;
            }
            while (!(IsInteracting || timer > ImageHeldLength));

            firstTime = false;

            // If interaction was done, hide the thing immediately since there's
            // no guarantee it's already shown
            if (IsInteracting)
            {
                Debug.Log("Showing immediately");
                newImage.ShowImmediately();
            }
            Debug.Log("Shown");

            // Wait for the current input state to clear
            yield return null;

            // Wait for next input
            while (!IsInteracting)
            {
                yield return null;
            }

            Debug.Log("Hiding");
            newImage.Hide();

            // Waits for either input, or time elapsed
            timer = 0;
            do
            {
                timer += Time.deltaTime;
                yield return null;
            }
            while (!(IsInteracting || timer > ImageReleaseLength));

            // If interaction was done, hide the thing immediately since there's
            // no guarantee it's already hidden
            if (IsInteracting)
            {
                Debug.Log("Hiding immediately");
                newImage.HideImmediately();
            }
            Debug.Log("Hid");
        }

        music.StopMusic();

        Debug.Log("Done, waiting for a bit before transitioning to new scene.");
        yield return new WaitForSeconds(WaitAfterFinished);

        Debug.Log("Transitioning...");
        SceneManager.LoadScene(SceneName);
        var uIManager = UIManager.Instance;
        uIManager.FadeManager.FadeIn(uIManager.StartFadeTime, Color.black);
        uIManager.State = NewSceneState;
    }
}
