using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;
    private bool isTyping;
    private bool clicked;

    // Start is called before the first frame update

    [Header("Audio")]

    [SerializeField] private AudioClip[] dialogueTypingSoundClips;
    [Range(1, 5)]

    [SerializeField] private int frequencyLevel = 2;
    [Range(-3, 3)]

    [SerializeField] private float minPitch = 0.5f;
    [Range(-3, 3)]

    [SerializeField] private float maxPitch = 3f;

    [SerializeField] private bool stopAudioSource;

    private AudioSource audioSource;

    private int charCounter = 0; // New character counter for audio

    // Update is called once per frame

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !clicked)
        {
            clicked = true;

            if (!isTyping && textComponent.text == lines[index])
            {
                NextLine();
            }
            else if (isTyping)
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                isTyping = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            clicked = false;
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        charCounter = 0; // Reset the counter at the start of each line

        foreach (char c in lines[index].ToCharArray())
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

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    private void PlayDialogueSound()
    {
        if (stopAudioSource)
        {
            audioSource.Stop();
        }
        // sound clip
        int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
        AudioClip soundClip = dialogueTypingSoundClips[randomIndex];
        //pitch
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        // play sound
        audioSource.PlayOneShot(soundClip);
    }
}
