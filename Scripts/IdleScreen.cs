using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleScreen : MonoBehaviour
{
    public CanvasGroup idleScreenCanvasGroup;

    void Start()
    {
        StartCoroutine(IdleScreenCoroutine());
    }

    private IEnumerator IdleScreenCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(20);  // 20 seconds of inactivity, can be shortened/lengthened later
            idleScreenCanvasGroup.alpha = 1;
            // Add art stuff here later etc.

            // Wait until any input is detected
            while (true)
            {
                if (Input.anyKeyDown || Input.mousePresent || Input.touchCount > 0)
                {
                    idleScreenCanvasGroup.alpha = 0;
                    break;  // Exit the loop
                }
                yield return null;
            }
        }
    }

    public void ReturnToDeadline()  // Called by a button in the idle screen (feel free to change if another way is preferred)
    {
        idleScreenCanvasGroup.alpha = 0;
    }
}
