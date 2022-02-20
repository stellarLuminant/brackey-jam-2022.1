using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManagerTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            FadeManager.instance.FadeOut(1, Color.black);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            FadeManager.instance.FadeIn(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            FadeManager.instance.FadeOut(1, Color.white);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            FadeManager.instance.FadeIn(3);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            FadeManager.instance.FadeOut(1, Color.yellow, PrettyLittleFunction);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            FadeManager.instance.FadeIn(1, Color.yellow, ReadyToGo);
    }

    private void PrettyLittleFunction()
    {
        Debug.Log("We're completly faded...");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    private void ReadyToGo()
    {
        Debug.Log("Screen is clear, let's do it reddit");
    }
}