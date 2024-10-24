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
    private bool isBlinking = false;
    private bool isMessageActive = false;

    private void Start()
    {
        leftEyeStartPos = leftEye.localPosition;
        rightEyeStartPos = rightEye.localPosition;

        robotEyes.SetActive(true);    // Start in passive state
        messageText.gameObject.SetActive(false); // Hide the message text initially

        StartCoroutine(EyeIdleAnimation());
    }

    // Coroutine to display a message and temporarily hide the robot eyes
    public IEnumerator DisplayMessage(string message, float displayDuration)
    {
        isMessageActive = true;
        robotEyes.SetActive(false); // Hide the robot eyes
        messageText.gameObject.SetActive(true); // Show the message text
        messageText.text = message; // Display the message

        yield return new WaitForSeconds(displayDuration); // Wait for the message display duration

        messageText.gameObject.SetActive(false); // Hide the message text
        robotEyes.SetActive(true); // Show the robot eyes again
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
                leftEye.localPosition = leftEyeStartPos + (Vector3)Random.insideUnitCircle * 0.05f;
                rightEye.localPosition = rightEyeStartPos + (Vector3)Random.insideUnitCircle * 0.05f;

                // Random blinking
                if (!isBlinking && Random.value < 0.05f) // 5% chance to blink every loop
                {
                    StartCoroutine(BlinkEyes());
                }
            }
            yield return new WaitForSeconds(0.1f); // Loop the idle animation every 0.1 seconds
        }
    }

    // Coroutine to handle the blinking animation
    private IEnumerator BlinkEyes()
    {
        isBlinking = true;

        leftEye.localScale = new Vector3(1, 0.1f, 1); // Blink (squash)
        rightEye.localScale = new Vector3(1, 0.1f, 1);
        yield return new WaitForSeconds(0.1f); // Blink duration

        leftEye.localScale = Vector3.one; // Open eyes
        rightEye.localScale = Vector3.one;
        yield return new WaitForSeconds(0.5f); // Wait before the next possible blink

        isBlinking = false;
    }
}
