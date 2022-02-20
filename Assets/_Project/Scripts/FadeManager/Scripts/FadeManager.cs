using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    [Header("Fade references")]
    public CanvasGroup fadeGroup;
    public RawImage fadeImage;

    [Header("Start Fade")]
    [SerializeField] bool startWithFade = true;
    [SerializeField] float startFadeTime = 1.5f;
    [SerializeField] Color startFadeColor = Color.black;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }

        // Keep this alive throughout the game
        DontDestroyOnLoad(gameObject);

        if (startWithFade)
        {
            fadeGroup.alpha = 1;
            FadeIn(startFadeTime, startFadeColor);
        }
        else
        {
            fadeGroup.alpha = 0;
        }
    }

    public void FadeOut(float transitionTime)
    {
        // Re-use last color, no action at the end
        FadeOut(transitionTime, fadeImage.color, null);
    }
    public void FadeOut(float transitionTime, Color fadeColor)
    {
        // Apply new color, no action at the end
        FadeOut(transitionTime, fadeColor, null);
    }
    public void FadeOut(float transitionTime, Color fadeColor, Action func)
    {
        fadeImage.color = fadeColor;
        StartCoroutine(UpdateFadeOut(transitionTime,func));
    }

    public void FadeIn(float transitionTime)
    {
        // Re-use last color, no action at the end
        FadeIn(transitionTime, fadeImage.color, null);
    }
    public void FadeIn(float transitionTime, Color fadeColor)
    {
        // Apply new color, no action at the end
        FadeIn(transitionTime, fadeColor, null);
    }
    public void FadeIn(float transitionTime, Color fadeColor, Action func)
    {
        fadeImage.color = fadeColor;
        StartCoroutine(UpdateFadeIn(transitionTime, func));
    }

    IEnumerator UpdateFadeOut(float transitionTime, Action func)
    {
        for (float t = 0.0f; t <= 1; t += Time.deltaTime/transitionTime)
        {
            fadeGroup.alpha = t;
            yield return null;
        }

        fadeGroup.alpha = 1;
        func?.Invoke();
    }
    IEnumerator UpdateFadeIn(float transitionTime, Action func)
    {
        for (float t = 0.0f; t <= 1; t += Time.deltaTime / transitionTime)
        {
            fadeGroup.alpha = 1 - t;
            yield return null;
        }

        fadeGroup.alpha = 0;
        func?.Invoke();
    }
}
