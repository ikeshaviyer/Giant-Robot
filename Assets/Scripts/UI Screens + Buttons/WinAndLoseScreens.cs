using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinAndLoseScreens : MonoBehaviour
{
    public CanvasGroup winScreenCanvasGroup;
    public CanvasGroup loseScreenCanvasGroup;

    void Start() 
    {
        winScreenCanvasGroup.alpha = 0;
        loseScreenCanvasGroup.alpha = 0;
    }
    
    public void ReturnToTitle()
    {
        StartCoroutine(ReturnToTitleCoroutine());
    }

    private IEnumerator ReturnToTitleCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void GetNextDeadlineMission()
    {
        // This function will be called when the player clicks the "Next Mission" button
        // It will load the next mission scene
        // For now, it will just return to the title screen
        StartCoroutine(ReturnToTitleCoroutine());
    }
}
