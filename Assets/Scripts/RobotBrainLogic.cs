using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RobotBrainLogic : MonoBehaviour
{
    // Define robot parts and resources
    public enum RobotPart { Head, Arm, Leg, Core }
    public enum Resource { Metal, Circuit, Battery, Wire }

    public int roundsBeforeDeadline = 5;
    public int difficultyLevel = 1;

    // Store the robot parts and their repair status
    private Dictionary<RobotPart, List<Resource>> repairsNeeded = new Dictionary<RobotPart, List<Resource>>();
    private Dictionary<RobotPart, bool> repairStatus = new Dictionary<RobotPart, bool>();

    private RobotBrainVisuals visuals;

    void Start()
    {
        visuals = GetComponent<RobotBrainVisuals>();
        GenerateRepairs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndRound();
        }
    }

    void GenerateRepairs()
    {
        repairsNeeded.Clear();
        repairStatus.Clear();

        // Calculate how many parts need repairs based on difficulty level
        int partsToRepair = Mathf.Min(difficultyLevel, System.Enum.GetValues(typeof(RobotPart)).Length);

        // Randomly determine which parts need repair and with what resources
        List<RobotPart> availableParts = new List<RobotPart>((RobotPart[])System.Enum.GetValues(typeof(RobotPart)));
        for (int i = 0; i < partsToRepair; i++)
        {
            RobotPart selectedPart = availableParts[Random.Range(0, availableParts.Count)];
            availableParts.Remove(selectedPart);

            // Generate a random resource for the repair
            Resource requiredResource = (Resource)Random.Range(0, System.Enum.GetValues(typeof(Resource)).Length);
            repairsNeeded.Add(selectedPart, new List<Resource> { requiredResource });
            repairStatus.Add(selectedPart, false);

            // Display the repair message for each part
            StartCoroutine(DisplayMessageWithDelay($"{selectedPart} needs repairing, bring {requiredResource}.", 2f * (i + 1)));
        }

        // Display the initial round countdown message after all parts' repair messages
        StartCoroutine(DisplayMessageWithDelay($"{roundsBeforeDeadline} rounds left before DEADLINE.", partsToRepair * 2f + 2f));
    }

    // Coroutine to handle displaying a message with a delay
    private IEnumerator DisplayMessageWithDelay(string message, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(visuals.DisplayMessage(message, 3f));
    }

    // Call this to simulate the end of a round and update the round count only when space bar is pressed
    public void EndRound()
    {
        if (roundsBeforeDeadline > 0)
        {
            roundsBeforeDeadline--;

            if (roundsBeforeDeadline > 0)
            {
                StartCoroutine(visuals.DisplayMessage($"{roundsBeforeDeadline} rounds left before DEADLINE.", 2f));
            }
            else
            {
                CheckRepairStatus();
            }
        }
    }

    // Check if the repairs are successful or if the game is over
    private void CheckRepairStatus()
    {
        bool allRepaired = true;

        foreach (var part in repairsNeeded.Keys)
        {
            if (!repairStatus[part])
            {
                allRepaired = false;
                break;
            }
        }

        if (allRepaired)
        {
            StartCoroutine(visuals.DisplayMessage("Good Job! Repairs successful.", 3f));
            difficultyLevel++;
            roundsBeforeDeadline = 5;
            GenerateRepairs();
        }
        else
        {
            StartCoroutine(visuals.DisplayMessage("Game OVER! Repairs failed.", 3f));
        }
    }
}
