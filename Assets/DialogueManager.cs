using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    public Dialogue dialogueComponent; // Reference to the Dialogue component
    private Queue<string> dialogueQueue = new Queue<string>(); // Queue to hold dialogue lines
    private bool isDisplayingDialogue = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps it active across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates if already created
        }
    }
    
    // Method to add a new line of dialogue to the queue
    public void QueueDialogue(string line)
    {
        dialogueQueue.Enqueue(line);
        if (!isDisplayingDialogue) StartCoroutine(DisplayNextLine());
    }

    // Coroutine to display each line in the queue
    private IEnumerator DisplayNextLine()
    {
        isDisplayingDialogue = true;

        while (dialogueQueue.Count > 0)
        {
            string nextLine = dialogueQueue.Dequeue();
            dialogueComponent.StartTypingDialogue(nextLine);

            // Wait until the Dialogue component finishes typing the current line
            yield return new WaitUntil(() => !dialogueComponent.IsTyping);

            // Small delay before displaying the next line
            yield return new WaitForSeconds(0.5f);
        }

        isDisplayingDialogue = false;
    }
}
