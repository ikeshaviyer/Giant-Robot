using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlineScreen : MonoBehaviour
{
    public CanvasGroup confirmationCanvasGroup;
    public AudioSource audioManager;
    public AudioClip clickSound;

    void Start() 
    {
        HideConfirmation();
    }
    
    public void PressReturnToTitle()
    {
        if (confirmationCanvasGroup.alpha == 0)
        {
            ShowConfirmation();
            PlayClickSound();
        }
    }

    public void PressYes()
    {
        PlayClickSound();
        StartCoroutine(ReturnToTitleCoroutine());
    }

    public void PressNo()
    {
        PlayClickSound();
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

    private void PlayClickSound()
    {
        if (audioManager != null && clickSound != null)
        {
            audioManager.PlayOneShot(clickSound);
            StartCoroutine(LerpPitch(audioManager, Random.Range(1.2f, 1.15f), Random.Range(1f, 0.95f), clickSound.length));
        }
    }

    private IEnumerator LerpPitch(AudioSource source, float startPitch, float endPitch, float duration)
    {
        float elapsedTime = 0f;
        source.pitch = startPitch;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            source.pitch = Mathf.Lerp(startPitch, endPitch, elapsedTime / duration);
            yield return null;
        }
        source.pitch = endPitch;
    }
}
