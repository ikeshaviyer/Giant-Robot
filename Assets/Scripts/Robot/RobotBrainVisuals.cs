using UnityEngine;
using TMPro;
using System.Collections;

public class RobotBrainVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText; // Reference to the TextMesh Pro UI element
    [SerializeField] private GameObject robotEyes; // Parent object for the robot eyes animation
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;

    private Vector3 leftEyeStartPos;
    private Vector3 rightEyeStartPos;
    private Vector3 eyeOriginalScale = Vector3.one; // Store the original eye scale
    private bool isBlinking = false;
    private bool isMessageActive = false;

    private void Start()
    {
        // Save the initial positions of the eyes
        leftEyeStartPos = leftEye.localPosition;
        rightEyeStartPos = rightEye.localPosition;

        // Ensure robot eyes are visible and message text is hidden at start
        robotEyes.SetActive(true);
        messageText.gameObject.SetActive(false);

        // Start the idle eye animation
        StartCoroutine(EyeIdleAnimation());
    }

    // Coroutine to display a message and temporarily hide the robot eyes
    public IEnumerator DisplayMessage(string message, float displayDuration)
    {
        isMessageActive = true;

        // Hide the robot eyes and show the message text
        robotEyes.SetActive(false);
        messageText.gameObject.SetActive(true);
        messageText.text = message;

        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Hide the message text and show the robot eyes again
        messageText.gameObject.SetActive(false);
        robotEyes.SetActive(true);

        isMessageActive = false;
    }

    // Coroutine for eye idle animations with blinking and random movement
    private IEnumerator EyeIdleAnimation()
    {
        while (true)
        {
            if (!isMessageActive)
            {
                // Random eye movement
                leftEye.localPosition = Vector3.Lerp(leftEye.localPosition, leftEyeStartPos + (Vector3)Random.insideUnitCircle * 0.05f, 0.2f);
                rightEye.localPosition = Vector3.Lerp(rightEye.localPosition, rightEyeStartPos + (Vector3)Random.insideUnitCircle * 0.05f, 0.2f);

                // Random blinking with a 5% chance
                if (!isBlinking && Random.value < 0.05f)
                {
                    StartCoroutine(BlinkEyes());
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Coroutine to handle the blinking animation
    private IEnumerator BlinkEyes()
    {
        isBlinking = true;

        // Smoothly squash the eyes to simulate blinking
        float blinkDuration = 0.1f;
        float blinkTime = 0f;
        while (blinkTime < blinkDuration)
        {
            blinkTime += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0.1f, blinkTime / blinkDuration);
            leftEye.localScale = new Vector3(1, scale, 1);
            rightEye.localScale = new Vector3(1, scale, 1);
            yield return null;
        }

        // Wait for a brief moment with eyes closed
        yield return new WaitForSeconds(0.1f);

        // Smoothly open the eyes
        blinkTime = 0f;
        while (blinkTime < blinkDuration)
        {
            blinkTime += Time.deltaTime;
            float scale = Mathf.Lerp(0.1f, 1f, blinkTime / blinkDuration);
            leftEye.localScale = new Vector3(1, scale, 1);
            rightEye.localScale = new Vector3(1, scale, 1);
            yield return null;
        }

        // Reset the eye scale
        leftEye.localScale = eyeOriginalScale;
        rightEye.localScale = eyeOriginalScale;

        isBlinking = false;

        // Wait a moment before the next possible blink
        yield return new WaitForSeconds(0.5f);
    }
}
