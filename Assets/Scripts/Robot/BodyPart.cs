using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public bool isRepaired = false;
    public bool canRepair = false;
    public ResourceManager resourceManager;
    public string requiredResourceType;
    public int requiredResourceAmount;

    void Start()
    {
        isRepaired = true;
    }

    public void SetRandomResourceRequirement()
    {
        requiredResourceType = resourceManager.GetRandomResourceType();
        requiredResourceAmount = resourceManager.GetRandomResourceAmount(requiredResourceType);
        Debug.Log($"Required resource for {gameObject.name}: {requiredResourceType}, Amount: {requiredResourceAmount}");
        DialogueManager.Instance.QueueDialogue($"Required resource for {gameObject.name}: {requiredResourceType}, Amount: {requiredResourceAmount}");
    }

    public void AttemptRepair()
    {
        if (canRepair && !isRepaired)
        {
            Debug.Log($"Attempting to repair {gameObject.name} using {requiredResourceType}. Required amount: {requiredResourceAmount}");
            // Call to scan resources dynamically would occur here
        }
    }

    public void ScanResource(string resourceType)
    {
        resourceManager.ScanResource(resourceType);
        AttemptRepair(); // Try to repair after scanning
    }

    public void Repair()
    {
        isRepaired = true;
        Debug.Log($"{gameObject.name} repaired successfully!");
    }
}
