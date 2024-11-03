using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed;

    private bool isTyping;
    public bool IsTyping => isTyping; // Public property to check if typing is in progress

    [Header("Audio")]
    [SerializeField] private AudioClip[] dialogueTypingSoundClips;
    [Range(1, 5)] [SerializeField] private int frequencyLevel = 2;
    [Range(-3, 3)] [SerializeField] private float minPitch = 0.5f;
    [Range(-3, 3)] [SerializeField] private float maxPitch = 3f;
    [SerializeField] private bool stopAudioSource;

    private AudioSource audioSource;
    private int charCounter;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        textComponent.text = string.Empty;
    }

    public void StartTypingDialogue(string line)
    {
        StopAllCoroutines(); // Stop any ongoing typing
        textComponent.text = string.Empty; // Clear the text field
        StartCoroutine(TypeLine(line)); // Start typing the new line
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        charCounter = 0;

        foreach (char c in line.ToCharArray())
        {
            charCounter++;
            textComponent.text += c;

            if (charCounter % frequencyLevel == 0) // Play sound for every other character
            {
                PlayDialogueSound();
            }

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void PlayDialogueSound()
    {
        if (stopAudioSource)
        {
            audioSource.Stop();
        }
        
        int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
        AudioClip soundClip = dialogueTypingSoundClips[randomIndex];
        
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(soundClip);
    }
}
