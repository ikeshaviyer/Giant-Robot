using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RobotBrainLogic : MonoBehaviour
{
    [Header("Game Logic")]
    public int roundsBeforeDeadline = 5;
    public static int difficultyLevel = 1;
    private int deadlineNumber = 1;

    [SerializeField]
    private List<BodyPart> bodyPartsToRepair = new List<BodyPart>();
    private List<BodyPart> selectedParts = new List<BodyPart>();

    [Header("UI Elements")]
    public TextMeshProUGUI deadlineText;
    public TextMeshProUGUI roundsLeftText;
    public CanvasGroup winGameCanvasGroup;
    public CanvasGroup loseGameCanvasGroup;

    private bool isRepairInProgress = false; // Track if a repair is in progress
    private float timeSinceLastSpeak;
    private bool canEndRound = false;
    public bool IsRepairInProgress => isRepairInProgress;  // Read-only property

    void Start()
    {
        StartNewGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || SerialReader.Instance.ButtonPressed)
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
        UpdateRoundsLeftText();
        UpdateDeadlineText();
    }

    private void UpdateDeadlineText()
    {
        if (deadlineText != null)
        {
            deadlineText.text = $"Deadline Number {deadlineNumber}";
        }
    }

    private void UpdateRoundsLeftText()
    {
        if (roundsLeftText != null)
        {
            roundsLeftText.text = $"Rounds Left: {roundsBeforeDeadline}";
        }
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
            part.SetRandomResourceRequirement(); // Randomize resources
            Debug.Log($"Part: {part.name} requires {part.requiredCircuits} circuits, {part.requiredEnergyCores} energy cores, and {part.requiredScrapMetal} scrap metal.");
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
        UpdateRoundsLeftText();
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
            DeadlineWin();
        }
        else
        {
            GameOver();
        }
    }

    void DeadlineWin()
    {
        winGameCanvasGroup.alpha = 1;
        winGameCanvasGroup.interactable = true;
        winGameCanvasGroup.blocksRaycasts = true;
    }

    void GameOver()
    {
        loseGameCanvasGroup.alpha = 1;
        loseGameCanvasGroup.interactable = true;
        loseGameCanvasGroup.blocksRaycasts = true;
    }

    public void NextDeadline()
    {
        HideWinGameCanvas();
        difficultyLevel++;
        deadlineNumber++;
        roundsBeforeDeadline = 5;
        Debug.Log("Next Deadline started");

        DisasterLogic.Instance.ResetDisasterForNewDeadline();
        MifareCardReader.Instance.ResetCards();
        RandomizeBodyPartRequirements();
        UpdateRoundsLeftText();
        UpdateDeadlineText();
    }

    public List<BodyPart> GetSelectedParts()
    {
        return selectedParts;
    }

    public void ReturnToTitle()
    {
        StartCoroutine(ReturnToTitleCoroutine());
    }

    private IEnumerator ReturnToTitleCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void HideWinGameCanvas()
    {
        winGameCanvasGroup.alpha = 0;
        winGameCanvasGroup.interactable = false;
        winGameCanvasGroup.blocksRaycasts = false;
    }
}
