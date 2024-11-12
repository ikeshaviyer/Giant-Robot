using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public bool isRepaired = false;
    public bool canRepair = false;
    
    // Resource requirements
    public int requiredCircuits;
    public int requiredEnergyCores;
    public int requiredScrapMetal;

    void Start()
    {
        isRepaired = true;
    }

    public void SetRandomResourceRequirement()
    {
        // Reset resources
        requiredCircuits = 0;
        requiredEnergyCores = 0;
        requiredScrapMetal = 0;

        // Generate random resource amounts
        int totalAmount = RobotBrainLogic.difficultyLevel;
        requiredCircuits = Random.Range(0, totalAmount + 1);
        totalAmount -= requiredCircuits;

        requiredEnergyCores = Random.Range(0, totalAmount + 1);
        totalAmount -= requiredEnergyCores;

        requiredScrapMetal = totalAmount; // Remaining goes to Scrap Metal

        // Display the resource requirement using DialogueManager
        DialogueManager.Instance.QueueDialogue($"Required resources for {gameObject.name}: Circuits = {requiredCircuits}, Energy Cores = {requiredEnergyCores}, Scrap Metal = {requiredScrapMetal}");
    }

    public void AttemptRepair()
    {
        if (canRepair && !isRepaired)
        {
            Debug.Log($"Attempting to repair {gameObject.name} using Circuits: {requiredCircuits}, Energy Cores: {requiredEnergyCores}, Scrap Metal: {requiredScrapMetal}");
            // Call to scan resources dynamically would occur here
        }
    }

    public void Repair()
    {
        isRepaired = true;
        Debug.Log($"{gameObject.name} repaired successfully!");
    }
}
