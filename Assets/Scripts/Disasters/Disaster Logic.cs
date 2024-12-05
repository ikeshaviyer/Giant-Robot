using System.Collections.Generic;
using UnityEngine;

public class DisasterLogic : MonoBehaviour
{
    public static DisasterLogic Instance { get; private set; }

    private bool disasterOccurredThisDeadline = false;
    private AudioSource audioSource;
    private AudioClip sound;
    public List<AudioClip> disasterSounds;

    public List<Disaster> easyDisasters; // List of disasters for Easy difficulty
    public List<Disaster> mediumDisasters; // List of disasters for Medium difficulty
    public List<Disaster> catastrophicDisasters; // List of disasters for Catastrophic difficulty

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    private void Start()
    {
        // Initialize disaster lists
        InitializeDisasters();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("No audio clip assigned to this disaster.");
            return;
        }
    }

    private void InitializeDisasters()
    {
        easyDisasters = new List<Disaster>
        {
            new RoundReductionDisaster { name = "Round Reduction", probability = 0.6f, message = "Rounds have been drastically reduced!", sound = disasterSounds[0]},
            new ResourceDrainDisaster { name = "Resource Drain", probability = 0.5f, message = "Most resources have been drained!", sound = disasterSounds[1]},
            new ActionDrainDisaster { name = "Action Drain", probability = 0.4f, message = "Action points have been severely reduced!", sound = disasterSounds[2]},
            // Add more Easy disasters here
        };

        mediumDisasters = new List<Disaster>
        {
            new MediumRoundReductionDisaster { name = "Medium Round Reduction", probability = 0.6f, message = "Rounds have been drastically reduced!", sound = disasterSounds[3] },
            new MediumResourceDrainDisaster { name = "Medium Resource Drain", probability = 0.5f, message = "Most resources have been drained!", sound = disasterSounds[4] },
            new MediumActionDrainDisaster { name = "Medium Action Drain", probability = 0.4f, message = "Action points have been severely reduced!", sound = disasterSounds[5] },
            // Add more Medium disasters here
        };

        catastrophicDisasters = new List<Disaster>
        {
            new HardRoundReductionDisaster { name = "Catastrophic Round Reduction", probability = 0.6f, message = "Rounds have been drastically reduced!", sound = disasterSounds[6] },
            new HardResourceDrainDisaster { name = "Catastrophic Resource Drain", probability = 0.5f, message = "Most resources have been drained!", sound = disasterSounds[7] },
            new HardActionDrainDisaster { name = "Catastrophic Action Drain", probability = 0.4f, message = "Action points have been severely reduced!", sound = disasterSounds[8] },
            // Add more Catastrophic disasters here
        };
    }

    // Method to determine if a disaster should happen
    public void CheckForDisaster(int difficultyLevel, ref int roundsBeforeDeadline)
    {
        List<Disaster> selectedDisasterList;

        if (difficultyLevel <= 3)
        {
            selectedDisasterList = easyDisasters;
        }
        else if (difficultyLevel <= 6)
        {
            selectedDisasterList = mediumDisasters;
        }
        else
        {
            selectedDisasterList = catastrophicDisasters;
        }

        if (disasterOccurredThisDeadline) return; // Only one disaster per deadline

        foreach (var disaster in selectedDisasterList)
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
