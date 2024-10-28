using System.Collections.Generic;
using UnityEngine;

public class RepairLogic : MonoBehaviour
{
    private Dictionary<RobotPart, List<Resource>> repairRequirements;

    // Generates repairs based on the difficulty level
    public void GenerateRepairs(int difficultyLevel)
    {
        repairRequirements = new Dictionary<RobotPart, List<Resource>>();

        // Example logic for assigning resources based on difficulty
        foreach (RobotPart part in System.Enum.GetValues(typeof(RobotPart)))
        {
            List<Resource> resourcesNeeded = new List<Resource>();

            // More resources required for higher difficulty levels
            int resourceCount = Mathf.Clamp(difficultyLevel, 1, System.Enum.GetValues(typeof(Resource)).Length);

            for (int i = 0; i < resourceCount; i++)
            {
                resourcesNeeded.Add((Resource)Random.Range(0, System.Enum.GetValues(typeof(Resource)).Length));
            }

            repairRequirements.Add(part, resourcesNeeded);
        }
    }

    // Returns a formatted string of parts and required resources
    public string GetRepairDetails()
    {
        string details = "";
        foreach (var repair in repairRequirements)
        {
            details += $"{repair.Key}: ";

            foreach (var resource in repair.Value)
            {
                details += $"{resource} ";
            }
            details += "\n";
        }
        return details.TrimEnd();
    }

    // Checks if all repairs are successful
    public bool AreAllRepairsSuccessful()
    {
        // Your logic for checking whether repairs were successful
        return true; // Placeholder
    }
}
