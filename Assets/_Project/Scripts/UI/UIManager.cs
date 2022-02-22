using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public enum GameState { Title, Cutscene, Game }
    public GameState State
    {
        get { return _state; }
        set
        {
            ChangeState(value);
            _state = value;
        }
    }
    private GameState _state;
    public GameState StartingState = GameState.Title;

    [Header("UI Elements")]
    public GameObject TitleScreen;
    public GameObject GameUI;
    public Button PlayButton;
    // public TutorialText TutorialText;
    // public LevelNameText LevelNameText;

    [Header("Fade Manager")]
    public FadeManager FadeManager;
    public float StartFadeTime = 1.5f;

    [Header("Debug")]
    public GameState ForcedSetGameState;
    public bool TriggerForcedSetGameState;

    private void ChangeState(GameState newState)
    {
        Array values = Enum.GetValues(typeof(GameState));
        foreach (GameState val in values)
        {
            //if (newState == val)
            //{
            //    continue;
            //}
        }

        bool activeState = true;
        switch (newState)
        {
            case GameState.Title:
                TitleScreen.SetActive(activeState);
                GameUI.SetActive(!activeState);
                Time.timeScale = 1;

                PlayButton.Select();
                // Tutorial.Reset();
                break;
            case GameState.Cutscene:
                TitleScreen.SetActive(!activeState);
                GameUI.SetActive(!activeState);
                Time.timeScale = 1;

                break;
            case GameState.Game:
                TitleScreen.SetActive(!activeState);
                GameUI.SetActive(activeState);
                Time.timeScale = 1;
                break;
            default:
                Debug.LogError("wtf");
                break;
        }
    }

    #region Unity events
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
        State = StartingState;
    }

    // Update is called once per frame
    private void Update()
    {
        if (TriggerForcedSetGameState)
        {
            TriggerForcedSetGameState = false;
            State = ForcedSetGameState;
        }
    }
    #endregion
}
