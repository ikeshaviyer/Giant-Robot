using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBrainLogic : MonoBehaviour
{
    public int roundsBeforeDeadline = 5;
    public static int difficultyLevel = 1;

    [SerializeField]
    private List<BodyPart> bodyPartsToRepair = new List<BodyPart>();
    private List<BodyPart> selectedParts = new List<BodyPart>();

    void Start()
    {
        StartNewGame();
    }

    void StartNewGame()
    {
        RandomizeBodyPartRequirements();
    }

        void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndRound();
        }
    }

    public void RandomizeBodyPartRequirements()
    {
        // Set all body parts to repaired by default
        foreach (var part in bodyPartsToRepair)
        {
            part.isRepaired = true;
            part.canRepair = false; // Reset repair state
        }

        // Determine the number of parts to repair based on difficulty, capping at the list count
        int partsToRepairCount = Mathf.Min(difficultyLevel, bodyPartsToRepair.Count);

        // Randomly select the required number of parts for repair
        selectedParts = SelectRandomSubset(bodyPartsToRepair, partsToRepairCount);

        // Set requirements only for the selected parts
        foreach (var part in selectedParts)
        {
            part.isRepaired = false;
            part.SetRandomResourceRequirement();
        }
    }

    // Utility function to select a random subset of body parts
    private List<BodyPart> SelectRandomSubset(List<BodyPart> list, int count)
    {
        List<BodyPart> shuffledList = new List<BodyPart>(list);
        for (int i = 0; i < shuffledList.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledList.Count);
            var temp = shuffledList[i];
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }
        return shuffledList.GetRange(0, count);
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
            CheckGameOver();
        }
    }

    void CheckGameOver()
    {
        // Check if all selected parts are repaired
        bool allRepaired = true;
        foreach (var part in selectedParts)
        {
            if (!part.isRepaired)
            {
                allRepaired = false;
                break;
            }
        }

        if (allRepaired)
        {
            NextDeadline();
        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - You didn't repair all required parts in time.");
    }

    void NextDeadline()
    {
        difficultyLevel++;
        roundsBeforeDeadline = 5; // Reset rounds for the next deadline
        Debug.Log("Next Deadline started");
        RandomizeBodyPartRequirements();
    }

    void Success()
    {
        //what happens after a successful deadline
    }
}
