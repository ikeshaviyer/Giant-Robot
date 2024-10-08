using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class RobotBrainVisuals : MonoBehaviour
{
    // UI reference for the robot's speech display
    public TextMeshProUGUI robotSpeechText; // This should be a UI TextMeshPro component in your Canvas

    // Function to display text on the robot's speech box
    public void DisplayMessage(string message)
    {
        if (robotSpeechText != null)
        {
            robotSpeechText.text = message;
        }
    }
}
