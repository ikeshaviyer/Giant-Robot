using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

public class BodyPart : MonoBehaviour
{
    [Header("Repair Logic")]
    public bool isRepaired = false;
    public bool canRepair = false;
    public bool attemptedToRepair = false;

    [Header("Resource Requirements")]
    public int requiredCircuits;
    public int requiredEnergyCores;
    public int requiredScrapMetal;

    [Header("UI Elements")]
    public TextMeshProUGUI circuitsText;
    public TextMeshProUGUI energyCoresText;
    public TextMeshProUGUI scrapMetalText;

    [Header("References")]
    public RobotBrainLogic robotBrain;

    void Start()
    {
        isRepaired = true;
        robotBrain = FindObjectOfType<RobotBrainLogic>(); // Find RobotBrainLogic in the scene
    }

    void Update()
    {
        CheckRepairSpot();
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

        // Display the resource requirements on the UI
        if (circuitsText != null)
        {
            circuitsText.text = requiredCircuits.ToString();
        }
        if (energyCoresText != null)
        {
            energyCoresText.text = requiredEnergyCores.ToString();
        }
        if (scrapMetalText != null)
        {
            scrapMetalText.text = requiredScrapMetal.ToString();
        }

        // Can also display the resource requirement using DialogueManager
        DialogueManager.Instance.QueueDialogue($"Required resources for {gameObject.name}: Circuits = {requiredCircuits}, Energy Cores = {requiredEnergyCores}, Scrap Metal = {requiredScrapMetal}");
    }

    public async void AttemptRepair()
    {
        HashSet<string> processedUIDs = new HashSet<string>(); // Track UIDs that have been used

        while (!isRepaired)
        {
            Debug.Log($"Scan required resources for {gameObject.name}");

            MifareCardReader.StartReading();

            // Continuously check for a new scanned UID
            while (!isRepaired)
            {
                string lastScannedDescription = MifareCardReader.Instance.GetLastScannedUIDDescription();
                string lastScannedUID = MifareCardReader.Instance.GetLastScannedUID();

                // Ensure UID and description are not null and that UID hasn't been processed yet
                if (!string.IsNullOrEmpty(lastScannedDescription) && !string.IsNullOrEmpty(lastScannedUID) && !processedUIDs.Contains(lastScannedUID))
                {
                    // Check and process the resource type
                    if (lastScannedDescription == "Circuit" || lastScannedDescription == "Scrap Metal" || lastScannedDescription == "Energy Core")
                    {
                        // Subtract the resource and mark UID as processed
                        SubtractResource(lastScannedDescription);
                        processedUIDs.Add(lastScannedUID);

                        Debug.Log($"Processed resource: {lastScannedDescription} with UID {lastScannedUID}");

                        // Check if all requirements are fulfilled
                        if (requiredCircuits <= 0 && requiredScrapMetal <= 0 && requiredEnergyCores <= 0)
                        {
                            Repair();
                            break;
                        }
                    }
                }

                await Task.Delay(500); // Adjust delay as needed

                // Check if the repair process should stop
                if (Input.GetKeyDown(KeyCode.Space) || SerialReader.Instance.ButtonPressed)
                {
                    MifareCardReader.StopReading();
                    break;
                }
            }
        }
    }

    // Resource Subtraction
    private void SubtractResource(string resourceType)
    {
        switch (resourceType)
        {
            case "Circuit":
                requiredCircuits--;
                Debug.Log("Circuit resource used for repair.");
                break;
            case "Scrap Metal":
                requiredScrapMetal--;
                Debug.Log("Scrap Metal resource used for repair.");
                break;
            case "Energy Core":
                requiredEnergyCores--;
                Debug.Log("Energy Core resource used for repair.");
                break;
        }
    }

    public void Repair()
    {
        isRepaired = true;
        canRepair = false;
        DialogueManager.Instance.QueueDialogue($"{gameObject.name} repaired successfully!");
    }

    public void CheckRepairLogic()
    {
        if (!attemptedToRepair && !isRepaired)
        {
            // Check if no part can be repaired
            bool anyCanRepair = false;
            foreach (var part in robotBrain.GetSelectedParts())
            {
                if (part.canRepair)
                {
                    anyCanRepair = true;
                    break;
                }
            }

            // If no part is ready for repair, allow this part to start repairing
            if (!anyCanRepair)
            {
                canRepair = true;
                AttemptRepair();
                attemptedToRepair = true;
            }
        }
    }
        public virtual void CheckRepairSpot()
    {
        if (Input.GetKeyDown(KeyCode.R) && !attemptedToRepair && !isRepaired && !robotBrain.IsRepairInProgress) // Check if a repair is not already in progress
        {
            CheckRepairLogic();
        }
    }
}
