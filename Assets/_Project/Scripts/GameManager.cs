using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        TitleScreen
    }
    public GameState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }
    private GameState state;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
    // Called first
    void OnEnable()
    {
        Debug.Log("GameManager.OnEnable()");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager.OnSceneLoaded(Scene \"{scene.name}\", LoadSceneMode {mode})");
    }

    // Called when the game is terminated
    void OnDisable()
    {
        Debug.Log("GameManager.OnDisable()");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Loads game from title screen
    public IEnumerator StartGame()
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
