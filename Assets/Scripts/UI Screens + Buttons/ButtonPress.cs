using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioManager;
    public AudioClip clickSound;
    
    void Start() {}
    
    public void PressStart()
    {
        PlayClickSound();
        StartCoroutine(GoToGame());
    }

    public void PressSettings()
    {
        PlayClickSound();
        StartCoroutine(GoToSettings());
    }

    public void PressQuit()
    {
        PlayClickSound();
        GameManager.Instance.QuitGame();
    }

    public void OnBackButtonPress()
    {
        PlayClickSound();
        StartCoroutine(GoToMainMenu());
        Debug.Log("Back button pressed.");
    }

    public IEnumerator GoToGame()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.LoadScene("Game");
    }

    public IEnumerator GoToSettings()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.LoadScene("Settings");
    }

    public IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.LoadScene("Start");
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
