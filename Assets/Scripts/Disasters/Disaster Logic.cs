using System.Collections.Generic;
using UnityEngine;

public class DisasterLogic : MonoBehaviour
{
    public static DisasterLogic Instance { get; private set; }

    private bool disasterOccurredThisDeadline = false;

    [SerializeField]
    private List<Disaster> disasters; // List of available disasters

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

        private void Start()
    {
        // Optionally initialize your disasters here
        InitializeDisasters();
    }



    private void InitializeDisasters()
    {
        disasters = new List<Disaster>
        {
            new RoundReductionDisaster { name = "Round Reduction", probability = 0.3f, message = "Rounds have been reduced!" },
            new ResourceDrainDisaster { name = "Resource Drain", probability = 0.1f, message = "Resources have been drained!" },
            new ActionDrainDisaster { name = "Action Drain", probability = 0.1f, message = "Action points have been reduced!" },
            // Add more disasters here as needed
        };
    }

    // Method to determine if a disaster should happen
    public void CheckForDisaster(int difficultyLevel, ref int roundsBeforeDeadline)
    {
        if (disasterOccurredThisDeadline) return; // Only one disaster per deadline
        foreach (var disaster in disasters)
        {
            if (Random.value < Mathf.Min(disaster.probability * difficultyLevel, 1f))
            {
                disaster.ApplyEffect(ref roundsBeforeDeadline);
                disasterOccurredThisDeadline = true; // Prevent further disasters this deadline
                break; // Stop after the first disaster is applied
            }
        }
    }

    // Reset disaster occurrence flag for each new deadline
    public void ResetDisasterForNewDeadline()
    {
        disasterOccurredThisDeadline = false;
    }
}
