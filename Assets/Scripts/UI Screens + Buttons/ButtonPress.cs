using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour
{
    void Start() {}
    
    public void OnButtonPress()
    {
        Debug.Log("Button pressed: " + gameObject.name);
        StartCoroutine(GoToSettings());
    }

    public void OnBackButtonPress()
    {
        Debug.Log("Back button pressed.");
        StartCoroutine(GoToMainMenu());
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
}
