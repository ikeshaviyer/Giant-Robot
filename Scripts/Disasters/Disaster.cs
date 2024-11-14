using UnityEngine;

[System.Serializable]
public class Disaster
{
    public string name; // Name of the disaster
    public float probability; // Probability of this disaster occurring based on difficulty
    public string message; // Message to display when the disaster occurs

    // Method to apply the disaster effect
    public virtual void ApplyEffect(ref int roundsBeforeDeadline)
    {
        // Default effect can be overridden by subclasses
        Debug.Log($"{name} occurred! No specific effect defined.");
    }
}

[System.Serializable]
public class RoundReductionDisaster : Disaster
{
    public override void ApplyEffect(ref int roundsBeforeDeadline)
    {
        roundsBeforeDeadline = Mathf.Max(0, roundsBeforeDeadline - 1);
        Debug.Log($"{name} has occurred! Rounds remaining reduced by 1.");
        DialogueManager.Instance.QueueDialogue(message);
    }
}

// Additional disaster types can be defined here
[System.Serializable]
public class ResourceDrainDisaster : Disaster
{
    public override void ApplyEffect(ref int roundsBeforeDeadline)
    {
        // Example effect that could drain resources
        Debug.Log($"{name} has occurred! Resources are reduced by 1.");
        // Implement resource reduction logic here
        DialogueManager.Instance.QueueDialogue(message);
    }
}

[System.Serializable]
public class ActionDrainDisaster : Disaster
{
    public override void ApplyEffect(ref int roundsBeforeDeadline)
    {
        // Example effect that could drain resources
        Debug.Log($"{name} has occurred! Actions are reduced by 1.");
        // Implement resource reduction logic here
        DialogueManager.Instance.QueueDialogue(message);
    }
}
