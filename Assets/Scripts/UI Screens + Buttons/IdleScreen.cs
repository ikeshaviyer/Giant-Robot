using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleScreen : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup idleScreenCanvasGroup;
    public CanvasGroup missionDeadlineCanvasGroup;

    private void Start()
    {
        idleScreenCanvasGroup.alpha = 0;
    }

    public void Update()
    {
        if (missionDeadlineCanvasGroup != null && missionDeadlineCanvasGroup.alpha == 1)
        {
            StartCoroutine(IdleScreenCoroutine());
        }
    }

    private IEnumerator IdleScreenCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(15);  // 15 seconds of inactivity, can be shortened/lengthened later
            idleScreenCanvasGroup.alpha = 1;

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
}
