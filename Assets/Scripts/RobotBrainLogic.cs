using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBrainLogic : MonoBehaviour
{
    public int roundsBeforeDeadline = 5;
    public static int difficultyLevel = 1; // Made static to be accessed in other classes

    private ResourceManager resourceManager;

    // Static list defining the number of body parts needed to be repaired at each difficulty level
    private static List<int> partsNeededPerDifficulty = new List<int>
    {
        1, // Difficulty 1 needs 1 part
        2, // Difficulty 2 needs 2 parts
        3, // Difficulty 3 needs 3 parts
        4,
    };

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        StartNewGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Simulate round ending
        {
            EndRound();
        }

        // Check for NFC input or any dynamic resource scanning method
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Simulating a scan for Metal
        {
            ScanResource("Metal");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Simulating a scan for Circuit
        {
            ScanResource("Circuit");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Simulating a scan for Battery
        {
            ScanResource("Battery");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // Simulating a scan for Wire
        {
            ScanResource("Wire");
        }
    }

    void StartNewGame()
    {
        // Randomize resource requirements for body parts at the start of each deadline
        RandomizeBodyPartRequirements();
    }

    public void RandomizeBodyPartRequirements()
    {
        BodyPart[] bodyParts = FindObjectsOfType<BodyPart>();
        foreach (var part in bodyParts)
        {
            part.SetRandomResourceRequirement(); // Set a random resource requirement for each body part
        }
    }

    void EndRound()
    {
        roundsBeforeDeadline--;

        if (roundsBeforeDeadline > 0)
        {
            Debug.Log($"Rounds left before deadline: {roundsBeforeDeadline}");
        }
        else
        {
            StartCoroutine(InitiateRepairProcess());
        }
    }

    void GameOver()
    {
        Debug.Log($"GAME OVER YOU LOST");
    }

    void NextDeadline()
    {
        difficultyLevel++;
        Debug.Log($"Next Deadline started");
        StartCoroutine(InitiateRepairProcess());
    }

    IEnumerator InitiateRepairProcess()
    {
        // Here you can implement the logic to check if players are covering the sensors
        BodyPart[] bodyParts = FindObjectsOfType<BodyPart>();

        // Determine how many parts to repair based on the difficulty level
        int partsToRepair = partsNeededPerDifficulty[difficultyLevel - 1];
        List<BodyPart> partsToRepairList = new List<BodyPart>();

        // Randomly select body parts to repair
        for (int i = 0; i < partsToRepair; i++)
        {
            if (bodyParts.Length > 0)
            {
                int randomIndex = Random.Range(0, bodyParts.Length);
                if (!bodyParts[randomIndex].isRepaired && bodyParts[randomIndex].canRepair)
                {
                    partsToRepairList.Add(bodyParts[randomIndex]);
                }
            }
        }

        // Attempt to repair the selected parts
        foreach (var part in partsToRepairList)
        {
            part.AttemptRepair(); // Attempt to repair if conditions are met
            yield return new WaitForSeconds(1f); // Simulate time taken for repair process
        }

        // Reset rounds after repairs
        roundsBeforeDeadline = 5;
        RandomizeBodyPartRequirements(); // Reset random requirements for the next deadline
    }

    private void ScanResource(string resourceType)
    {
        BodyPart[] bodyParts = FindObjectsOfType<BodyPart>();

        foreach (var part in bodyParts)
        {
            if (!part.isRepaired && part.canRepair)
            {
                part.ScanResource(resourceType); // Scan the resource dynamically
                break; // Only scan for the first body part that needs repair
            }
        }
    }
}
