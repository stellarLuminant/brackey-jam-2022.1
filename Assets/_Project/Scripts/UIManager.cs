using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    FadeManager fadeManager;
    GameManager gameManager;
    Canvas canvas;
    bool loadingScene = false;

    public float timeBetweenBlackScreenAndGame = 1;

    public bool isTitleScreen = true;

    [Header("Elements")]
    public GameObject titleScreen;
    public GameObject gameScreen;
    public GameObject endScreen;
    public HeartContainer heartContainer;

    // Singleton instance
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
    // Called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    // Called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded: {scene.name} | mode {mode}");
        
        canvas = GetComponent<Canvas>();
        Debug.Assert(canvas, "Canvas not found");
    
        // If the canvas doesn't ahve a render camera,
        // set it to the main camera
        if (canvas && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
    }
    
    // Start is called before the first frame update
    // Called third
    void Start()
    {
        fadeManager = FadeManager.instance;
        gameManager = GameManager.instance;

        if (gameManager.playerInputObject)
        {
            TitleScreenExtras();
            endScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Loads game from title screen event
    public void StartGame()
    {
        if (!loadingScene)
        {
            StartCoroutine(StartGameEnum());
        }
        loadingScene = true;
    }

    public void TitleScreen()
    {
        TitleScreenExtras();
        SceneManager.LoadScene("Title");
    }

    void TitleScreenExtras(bool isTitleScreen = true)
    {
        titleScreen.SetActive(isTitleScreen);
        heartContainer.gameObject.SetActive(!isTitleScreen);
        if (gameManager.playerInputObject)
            gameManager.playerInputObject.SetActive(isTitleScreen);
    }

    //[YarnCommand("thanks_for_playing")]
    //public void ThanksForPlaying()
    //{
    //    StartCoroutine(ThanksForPlayingEnum());
    //}

    IEnumerator StartGameEnum()
    {
        // Hide with fade manager
        fadeManager.fadeGroup.alpha = 1;
        fadeManager.fadeImage.color = Color.black;

        // Hide titlescreen
        titleScreen.SetActive(false);

        // Wait for a bit
        yield return new WaitForSeconds(timeBetweenBlackScreenAndGame);

        // Load the game
        yield return gameManager.StartGame();

        TitleScreenExtras(false);

        // Fix the pixel stuff, and fade into the game
        FindObjectOfType<CanvasPixelScaling>().Scale();
        fadeManager.FadeIn(2f);

        // Reset variable
        loadingScene = false;
    }
}
