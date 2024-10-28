using System.Collections;
using UnityEngine;

public class RoundLogic
{
    private int roundsBeforeDeadline;
    private int difficultyLevel;
    private RobotBrainVisuals visuals;
    private RepairLogic repairLogic;

    public RoundLogic(RobotBrainVisuals visuals, int difficultyLevel, int roundsBeforeDeadline)
    {
        this.visuals = visuals;
        this.difficultyLevel = difficultyLevel;
        this.roundsBeforeDeadline = roundsBeforeDeadline;

        repairLogic = visuals.GetComponent<RepairLogic>(); // Get the RepairLogic script
    }

    public void GenerateRepairs()
    {
        repairLogic.GenerateRepairs(difficultyLevel);

        // Start the coroutine to display the message
        visuals.StartCoroutine(visuals.DisplayMessage($"Deadline started! {roundsBeforeDeadline} rounds to repair robot.", 2f));
    }

    public void DisplayRoundsLeft(int currentRound)
    {
        // Start the coroutine to display the message
        visuals.StartCoroutine(visuals.DisplayMessage($"Round {currentRound}/{roundsBeforeDeadline}", 2f));
    }

    public bool AreAllRepairsSuccessful()
    {
        return repairLogic.AreAllRepairsSuccessful();
    }

    public void UpdateDifficulty(int newDifficultyLevel, int newRoundsBeforeDeadline)
    {
        this.difficultyLevel = newDifficultyLevel;
        this.roundsBeforeDeadline = newRoundsBeforeDeadline;
        GenerateRepairs(); // Generate new repairs based on the increased difficulty level
    }

    // NEW: Get details of repairs needed for each part
    public string GetRepairDetails()
    {
        return repairLogic.GetRepairDetails();
    }
}
