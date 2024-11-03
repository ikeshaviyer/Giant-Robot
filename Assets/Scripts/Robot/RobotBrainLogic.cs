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

    private float idleSpeakInterval = 10f;
    private float timeSinceLastSpeak;

    void Start()
    {
        StartNewGame();
        timeSinceLastSpeak = idleSpeakInterval; // initialize to trigger an initial speak
    }

    void Update()
    {
        // End round with Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndRound();
        }

        // Trigger random idle speak at intervals
        // timeSinceLastSpeak += Time.deltaTime;
        // if (timeSinceLastSpeak >= idleSpeakInterval)
        // {
        //     RandomIdleSpeak();
        //     timeSinceLastSpeak = 0;
        // }
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
        roundsBeforeDeadline = 5; // Reset rounds for the next deadline
        Debug.Log("Next Deadline started");
        RandomizeBodyPartRequirements();
    }

    void Success()
    {
        // Actions to take on successful deadline
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
}
