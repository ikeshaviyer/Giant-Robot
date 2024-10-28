using UnityEngine;
using System.Collections;

public enum RobotPart { Head, Arm, Leg, Core }
public enum Resource { Metal, Circuit, Battery, Wire }

public class RobotBrainLogic : MonoBehaviour
{
    public int roundsBeforeDeadline = 5;
    public int difficultyLevel = 1;

    private RoundLogic roundLogic;
    private RobotBrainVisuals visuals;

    void Start()
    {
        visuals = GetComponent<RobotBrainVisuals>();
        roundLogic = new RoundLogic(visuals, difficultyLevel, roundsBeforeDeadline);
        StartNewGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndRound();
        }
    }

    void StartNewGame()
    {
        roundLogic.GenerateRepairs(); // Generate repairs based on difficulty

        // Get repair information from the RepairLogic and display it
        string repairDetails = roundLogic.GetRepairDetails();
        StartCoroutine(visuals.DisplayMessage($"Parts to Repair: {repairDetails}", 3f));

        // After showing parts to repair, show the rounds left message
        StartCoroutine(ShowRoundsAfterRepairDetails());
    }

    IEnumerator ShowRoundsAfterRepairDetails()
    {
        yield return new WaitForSeconds(3f); // Wait for the parts display message to finish
        StartCoroutine(visuals.DisplayMessage($"{roundsBeforeDeadline} rounds left before DEADLINE.", 2f));
    }

    void EndRound()
    {
        roundsBeforeDeadline--;

        if (roundsBeforeDeadline > 0)
        {
            roundLogic.DisplayRoundsLeft(roundsBeforeDeadline);
        }
        else
        {
            CheckRepairStatus();
        }
    }

    void CheckRepairStatus()
    {
        bool allRepaired = roundLogic.AreAllRepairsSuccessful();

        if (allRepaired)
        {
            StartCoroutine(visuals.DisplayMessage("Good Job! Repairs successful.", 3f));
            difficultyLevel++;
            roundsBeforeDeadline = 5;
            roundLogic.UpdateDifficulty(difficultyLevel, roundsBeforeDeadline);
        }
        else
        {
            StartCoroutine(visuals.DisplayMessage("Game OVER! Repairs failed.", 3f));
        }
    }
}
