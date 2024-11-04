using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    // Base resource amounts
    private Dictionary<string, int> maxResourceAmounts = new Dictionary<string, int>
    {
        { "Scrap Metal", 5 },
        { "Energy Cores", 5 },
        { "Circuits", 5 }
    };

    // This dictionary keeps track of resources that have been scanned
    private Dictionary<string, int> scannedResources = new Dictionary<string, int>();

    private RobotBrainLogic robotBrainLogic;

    void Start()
    {
        // Initialize scanned resources
        foreach (var resource in maxResourceAmounts.Keys)
        {
            scannedResources[resource] = 0; // Start with zero scanned resources
        }

        // Get the RobotBrainLogic instance
        robotBrainLogic = FindObjectOfType<RobotBrainLogic>();
    }

    // Get a random resource type
    public string GetRandomResourceType()
    {
        List<string> resourceKeys = new List<string>(maxResourceAmounts.Keys);
        return resourceKeys[Random.Range(0, resourceKeys.Count)];
    }

    // Get a random amount of a resource required for repair
    public int GetRandomResourceAmount(string resourceType)
    {
        int maxAmount = maxResourceAmounts[resourceType];
        // Access difficultyLevel through the instance of RobotBrainLogic
        int resourceAmt = Random.Range(1, RobotBrainLogic.difficultyLevel);
        if (resourceAmt > maxAmount)
        {
            resourceAmt = maxAmount;
        }
        return resourceAmt; // Random from 1 to difficulty level
    }

    // Scan a resource and add to the scanned resources
    public void ScanResource(string resourceType)
    {
        if (maxResourceAmounts.ContainsKey(resourceType))
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
