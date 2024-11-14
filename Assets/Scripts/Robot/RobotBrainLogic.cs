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

    private List<string> idleDialogues = new List<string>
    {
        "I hope you can fix me soon...",
        "I'm starting to feel a little rusty.",
        "You're my only hope for survival!",
        "Please hurry, my systems are failing...",
        "I’m counting on you to repair me!"
    };

    private bool isRepairInProgress = false; // Track if a repair is in progress
    private float idleSpeakInterval = 10f;
    private float timeSinceLastSpeak;

    private bool canEndRound = false;

    public bool IsRepairInProgress => isRepairInProgress;  // Read-only property

    void Start()
    {
        StartNewGame();
        timeSinceLastSpeak = idleSpeakInterval;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!canEndRound)
            {
                canEndRound = true;
            }
            else
            {
                EndRound();
            }
        }
    }

    void StartNewGame()
    {
        RandomizeBodyPartRequirements();
    }

    public void RandomizeBodyPartRequirements()
    {
        foreach (var part in bodyPartsToRepair)
        {
            part.isRepaired = true;
            part.canRepair = false; // Reset repair state
        }

        int partsToRepairCount = Mathf.Min(difficultyLevel, bodyPartsToRepair.Count);
        selectedParts = SelectRandomSubset(bodyPartsToRepair, partsToRepairCount);

        foreach (var part in selectedParts)
        {
            part.isRepaired = false;
            part.SetRandomResourceRequirement();
        }
    }

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
        DisasterLogic.Instance.CheckForDisaster(difficultyLevel, ref roundsBeforeDeadline);

        if (roundsBeforeDeadline > 0)
        {
            foreach (var part in selectedParts)
            {
                part.attemptedToRepair = false;
            }
        }
        else
        {
            CheckGameOver();
        }

        roundsBeforeDeadline--;
    }

    void CheckGameOver()
    {
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
        DialogueManager.Instance.QueueDialogue("NOOOOOOOOOOO YOU'RE TOO LATE");
    }

    void NextDeadline()
    {
        difficultyLevel++;
        roundsBeforeDeadline = 5;
        Debug.Log("Next Deadline started");

        DisasterLogic.Instance.ResetDisasterForNewDeadline();
        MifareCardReader.Instance.ResetCards();
        RandomizeBodyPartRequirements();
    }

    void RandomIdleSpeak()
    {
        if (DialogueManager.Instance != null)
        {
            int randomIndex = Random.Range(0, idleDialogues.Count);
            string randomDialogue = idleDialogues[randomIndex];
            DialogueManager.Instance.QueueDialogue(randomDialogue);
            Debug.Log($"Idle Speak: {randomDialogue}");
        }
    }

        // Provide the selected parts list to the BodyPart script
    public List<BodyPart> GetSelectedParts()
    {
        return selectedParts;
    }
}
