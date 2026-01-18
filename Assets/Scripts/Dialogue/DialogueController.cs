using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;
    public AudioClip statementEndSound;
    public Action OnDialogueEnd;
    bool busy = false;
    bool finished = false;

    TextMeshProUGUI dialogueText;
    TextMeshProUGUI dialogueName;
    Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Transform textPanel = transform.Find("Text");
        Transform namePanel = transform.Find("Name");

        dialogueText = textPanel.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        dialogueName = namePanel.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        canvas = transform.gameObject.GetComponent<Canvas>();
        canvas.enabled = false;
        //StartCoroutine(startDialogue("Hello there everyone, I'm a very cool guy!", "Very Cool Guy"));
    }

    public void sendDialogue(GameObject player, string[] dialogue, string name)
    {
        if (!busy)
        {
            canvas.enabled = true;
            StartCoroutine(startDialogue(player, dialogue, name));
        }
    }

    private IEnumerator startDialogue(GameObject player, string[] dialogue, string name)
    {
        busy = true;
        PlayerMovement mov = null;

        if (player != null)
        {
            mov = player.GetComponent<PlayerMovement>();
            if (mov != null) mov.stopMoving();
        }

        foreach (string message in dialogue)
        {
            dialogueName.text = name;
            dialogueText.text = message;
            dialogueText.maxVisibleCharacters = 0;

            for (int i = 0; i <= message.Length; i++)
            {
                dialogueText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(0.01f);
            }

            finished = true;
            SoundFXManager.instance.PlaySoundFXClip(statementEndSound, transform, 1.0f);
            while (finished)
                yield return new WaitForSeconds(0.01f);
        }

        busy = false;
        canvas.enabled = false;
        finished = false;
        if (mov != null) mov.startMoving();

        OnDialogueEnd?.Invoke();
        OnDialogueEnd = null;

        yield break;
    }

    public void continueDialogue()
    {
        finished = false;
    }

    public bool isBusy()
    {
        return busy;
    }
}
