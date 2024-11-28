using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlineScreen : MonoBehaviour
{
    public CanvasGroup confirmationCanvasGroup;
    void Start() 
    {
        HideConfirmation();
    }
    
    public void PressReturnToTitle()
    {
        if (confirmationCanvasGroup.alpha == 0)
        {
            ShowConfirmation();
        }
        else
        {
            HideConfirmation();
        }
    }

    public void PressYes()
    {
        StartCoroutine(ReturnToTitleCoroutine());
    }

    public void PressNo()
    {
        HideConfirmation();
    }

    private IEnumerator ReturnToTitleCoroutine()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void ShowConfirmation()
    {
        confirmationCanvasGroup.alpha = 1;
        confirmationCanvasGroup.interactable = true;
        confirmationCanvasGroup.blocksRaycasts = true;
    }

    private void HideConfirmation()
    {
        confirmationCanvasGroup.alpha = 0;
        confirmationCanvasGroup.interactable = false;
        confirmationCanvasGroup.blocksRaycasts = false;
    }
}
