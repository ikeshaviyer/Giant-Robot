using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    // Base resource amounts
    private Dictionary<string, int> baseResourceAmounts = new Dictionary<string, int>
    {
        { "Metal", 5 },
        { "Circuit", 3 },
        { "Battery", 2 },
        { "Wire", 4 }
    };

    // This dictionary keeps track of resources that have been scanned
    private Dictionary<string, int> scannedResources = new Dictionary<string, int>();

    private RobotBrainLogic robotBrainLogic;
    private int difficultyLevel;

    void Start()
    {
        difficultyLevel = RobotBrainLogic.difficultyLevel;
        // Initialize scanned resources
        foreach (var resource in baseResourceAmounts.Keys)
        {
            scannedResources[resource] = 0; // Start with zero scanned resources
        }

        // Get the RobotBrainLogic instance
        robotBrainLogic = FindObjectOfType<RobotBrainLogic>();
    }

    // Get a random resource type
    public string GetRandomResourceType()
    {
        List<string> resourceKeys = new List<string>(baseResourceAmounts.Keys);
        return resourceKeys[Random.Range(0, resourceKeys.Count)];
    }

    // Get a random amount of a resource required for repair
    public int GetRandomResourceAmount(string resourceType)
    {
        int baseAmount = baseResourceAmounts[resourceType];
        // Access difficultyLevel through the instance of RobotBrainLogic
        return Random.Range(1, baseAmount + difficultyLevel + 1); // Random from 1 to (base + difficulty)
    }

    // Scan a resource and add to the scanned resources
    public void ScanResource(string resourceType)
    {
        if (baseResourceAmounts.ContainsKey(resourceType))
        {
            scannedResources[resourceType]++;
            Debug.Log($"{resourceType} scanned. Total scanned: {scannedResources[resourceType]}");
        }
        else
        {
            Debug.LogWarning($"Resource type {resourceType} not recognized.");
        }
    }

    // Check if there are enough scanned resources of a specific type
    public bool CheckAndConsumeResources(string resourceType, int amount)
    {
        if (scannedResources.ContainsKey(resourceType) && scannedResources[resourceType] >= amount)
        {
            scannedResources[resourceType] -= amount; // Consume the resources
            return true; // Successfully consumed
        }
        return false; // Not enough resources
    }
}
